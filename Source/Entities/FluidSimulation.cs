using Celeste.Mod.Entities;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using Celeste.Mod.GooberHelper.ModIntegration;
using Celeste.Mod.GooberHelper.Attributes.Hooks;

//absolutely atrocious code
//dont reference for anything
//this sucks

namespace Celeste.Mod.GooberHelper.Entities {
    [CustomEntity("GooberHelper/FluidSimulation")]
    [Tracked(false)]
    public class FluidSimulation : Entity {
		public class RenderTargetSet {
			public DoubleRenderTarget2D Source;
			public DoubleRenderTarget2D Velocity;
			public DoubleRenderTarget2D Pressure;
			public RenderTarget2D DivergenceCurl;
			public RenderTarget2D Display;

			public RenderTargetSet(Rectangle bounds) {
				Source = new DoubleRenderTarget2D(bounds.Width, bounds.Height);
				Velocity = new DoubleRenderTarget2D(bounds.Width, bounds.Height);
				Pressure = new DoubleRenderTarget2D(bounds.Width, bounds.Height);
				DivergenceCurl = new RenderTarget2D(Engine.Instance.GraphicsDevice, bounds.Width, bounds.Height, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
				Display = new RenderTarget2D(Engine.Instance.GraphicsDevice, bounds.Width, bounds.Height, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
			}

			public void Dispose() {
				Source.Read.Dispose();
				Source.Write.Dispose();
				Velocity.Read.Dispose();
				Velocity.Write.Dispose();
				Pressure.Read.Dispose();
				Pressure.Write.Dispose();
				DivergenceCurl.Dispose();
				Display.Dispose();
			}
		}

		public static Dictionary<EntityID, RenderTargetSet> RenderTargetSets = [];
		public static Effect DisplayShader = null;
		public static Effect AdvectionShader = null;
		public static Effect BaseVelocityShader = null;
		public static Effect JacobiShader = null;
		public static Effect DivergenceCurlShader = null;
		public static Effect GradientShader = null;
		public static Effect DiffuseShader = null;
		public static Effect VorticityShader = null;

        private Rectangle bounds;

		private MeshData plane;

        private float playerVelocityInfluence;
        private float playerSizeInfluence;
        private string textureName;
        private float velocityDiffusion;
        private float colorDiffusion;
        private float playerHairDyeFactor;
        private List<Color> dyeColors;
        private float dyeBrightness;
		private float dyeCycleSpeed;
		private bool onlyDyeWhileDashing;
		private bool onlyInfluenceWhileDashing;
		private float playerSpeedForFullBrightness;
		private int pressureIterations;
		private float vorticity;
		private bool doExplosionShockwave;
		private float shockwaveSize;
		private float shockwaveForce;

		private float dyeCycleTime = 0;

		private bool duplicate = false;	

		public EntityID EntityId;

        public FluidSimulation(EntityData data, Vector2 offset) : base(data.Position + offset) {
            Tag = Tags.TransitionUpdate;
			EntityId = new EntityID(data.Level.Name, data.ID);
            Depth = data.Int("depth", 10001);

            bounds = new Rectangle((int)(data.Position.X + offset.X), (int)(data.Position.Y + offset.Y), data.Width, data.Height);
            plane = MeshData.CreatePlane(data.Width, data.Height);

            playerVelocityInfluence = data.Float("playerVelocityInfluence", 0.1f);
            playerSizeInfluence = data.Float("playerSizeInfluence", 15.0f);
            textureName = data.Attr("texture", "");
            velocityDiffusion = data.Float("velocityDiffusion", 0.95f);
            colorDiffusion = data.Float("colorDiffusion", 0.95f);
            playerHairDyeFactor = data.Float("playerHairDyeFactor", 0.0f);
        	dyeBrightness = data.Attr("dyeColor", "00ffff,ffffff,ff44ff|0.5").Contains('|') ? float.Parse(data.Attr("dyeColor", "000000").Split("|")[1]) : 1f; //code programming glumbsup
        	dyeColors = [];
            dyeCycleSpeed = data.Float("dyeCycleSpeed", 4.0f);
            onlyDyeWhileDashing = data.Bool("onlyDyeWhileDashing", false);
            onlyInfluenceWhileDashing = data.Bool("onlyInfluenceWhileDashing", false);
            playerSpeedForFullBrightness = data.Float("playerSpeedForFullBrightness", 90);
            pressureIterations = Math.Clamp(data.Int("pressureIterations", 50), 0, 100);
            vorticity = data.Float("vorticity", 0f);
            doExplosionShockwave = data.Bool("doExplosionShockwave", false);
            shockwaveSize = data.Float("shockwaveSize", 20);
            shockwaveForce = data.Float("shockwaveForce", 10);
			
			foreach(var str in data.Attr("dyeColor", "00ffff,ffffff,ff44ff|0.5").Split('|')[0].Split(","))
				dyeColors.Add(Calc.HexToColor(str));

			DisplayShader          = FrostHelperAPI.GetEffectOrNull.Invoke("display");
			AdvectionShader        = FrostHelperAPI.GetEffectOrNull.Invoke("advection");
			BaseVelocityShader     = FrostHelperAPI.GetEffectOrNull.Invoke("baseVelocity");
			JacobiShader           = FrostHelperAPI.GetEffectOrNull.Invoke("jacobi");
			DivergenceCurlShader   = FrostHelperAPI.GetEffectOrNull.Invoke("divergenceCurl");
			GradientShader         = FrostHelperAPI.GetEffectOrNull.Invoke("gradient");
			DiffuseShader          = FrostHelperAPI.GetEffectOrNull.Invoke("diffuse");
			VorticityShader        = FrostHelperAPI.GetEffectOrNull.Invoke("vorticity");

			AttemptAddSet();

			if(GetSet().Source != null && textureName != "") {
				Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().Source.Read);
				Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
				BeginSpriteBatch();
				var tex = GFX.Game[textureName];
				tex.Draw(Vector2.Zero, Vector2.Zero, Color.White, new Vector2(GetSet().Source.Read.Width/(float)tex.Width, GetSet().Source.Read.Height/(float)tex.Height));
				EndSpriteBatch();
			}

			Add(new BeforeRenderHook(BeforeRender));
        }

        public RenderTargetSet GetSet()
			=> RenderTargetSets[EntityId];

        public void AttemptAddSet() {
			if(RenderTargetSets.ContainsKey(EntityId))
				return;

			RenderTargetSets.Add(EntityId, new RenderTargetSet(bounds));
		}


		[OnHook]
		private static void patch_Puffer_Explode(On.Celeste.Puffer.orig_Explode orig, Puffer self) {
			orig(self);

			foreach(FluidSimulation sim in Engine.Scene.Tracker.GetEntities<FluidSimulation>())
				sim.Shockwave(self.Center);
		}

		[OnHook]
		private static IEnumerator patch_Seeker_RegenerateCoroutine(On.Celeste.Seeker.orig_RegenerateCoroutine orig, Seeker self) {
			var originalEnumerator = orig(self);

            while(originalEnumerator.MoveNext())
                yield return originalEnumerator.Current;

			foreach(FluidSimulation sim in Engine.Scene.Tracker.GetEntities<FluidSimulation>())
				sim.Shockwave(self.Center);
		}

		[OnHook]
		private static void patch_LevelExit_ctor(On.Celeste.LevelExit.orig_ctor orig, LevelExit self, LevelExit.Mode mode, Session session, HiresSnow snow = null) {
			orig(self, mode, session, snow);

			foreach(var id in RenderTargetSets.Keys) {
				RenderTargetSets[id].Dispose();
				RenderTargetSets.Remove(id);
			}
		}


		[OnHook]
		private static void patch_Level_UnloadLevel(On.Celeste.Level.orig_UnloadLevel orig, Level level) {
			orig(level);

			foreach(var id in RenderTargetSets.Keys) {
				RenderTargetSets[id].Dispose();
				RenderTargetSets.Remove(id);
			}
		}

        public static void BeginSpriteBatch()
			=> Draw.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null);

        public static void EndSpriteBatch()
			=> Draw.SpriteBatch.End();

        public void ClearDoubleRenderTarget2D(ref DoubleRenderTarget2D renderTarget) {
			if(renderTarget != null) {
				Engine.Graphics.GraphicsDevice.SetRenderTarget(renderTarget.Write);
				Engine.Instance.GraphicsDevice.Clear(Color.Transparent);

				Engine.Graphics.GraphicsDevice.SetRenderTarget(renderTarget.Read);
				Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			}
		}

		public void ClearRenderTarget2D(ref RenderTarget2D renderTarget) {
			Engine.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
		}

        public override void Update() {
            base.Update();

			if(duplicate)
				return;

			dyeCycleTime += Engine.DeltaTime * dyeCycleSpeed;
        }

		public void RenderEffect(Effect effect) {
			var viewport = Engine.Graphics.GraphicsDevice.Viewport;
			var projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, 1);
			
			effect.Parameters["TransformMatrix"]?.SetValue(projection);
        	effect.Parameters["ViewMatrix"]?.SetValue(Matrix.Identity);

			foreach(var pass in effect.CurrentTechnique.Passes) {
				pass.Apply();

				Engine.Instance.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, plane.Positions, 0, 4, plane.Indices, 0, 2);
			}
		}
		
		public Color GetDyeColor() {
			var cur = dyeColors[(int)Math.Floor(dyeCycleTime) % dyeColors.Count];
			var next = dyeColors[(int)Math.Ceiling(dyeCycleTime) % dyeColors.Count];

			return Color.Lerp(cur, next, dyeCycleTime % 1);
		}

		public void Splat(DoubleRenderTarget2D target, Vector3 color, Vector2 position, float size, bool shockwave = false) {
			Engine.Graphics.GraphicsDevice.SetRenderTarget(target.Write);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			Engine.Graphics.GraphicsDevice.Textures[0] = target.Read;
			BaseVelocityShader.Parameters["splatPosition"].SetValue(position - new Vector2(bounds.X, bounds.Y));
			BaseVelocityShader.Parameters["splatColor"].SetValue(color);
			BaseVelocityShader.Parameters["screenSize"].SetValue(new Vector2(bounds.Width, bounds.Height));
			BaseVelocityShader.Parameters["splatSize"].SetValue(size);
			BaseVelocityShader.Parameters["shockwave"].SetValue(shockwave);
			RenderEffect(BaseVelocityShader);
			target.Swap();
		}

		public void Shockwave(Vector2 position) {
			if(!doExplosionShockwave)
				return;

			Splat(
				GetSet().Velocity,
				new Vector3(shockwaveForce, 0, 0),
				position,
				shockwaveSize,
				true
			);
		}

		public void UpdateTextures() {
			Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().Velocity.Write);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			DiffuseShader.Parameters["amount"].SetValue(velocityDiffusion);
			Engine.Graphics.GraphicsDevice.Textures[0] = GetSet().Velocity.Read;
			RenderEffect(DiffuseShader);
			GetSet().Velocity.Swap();

			Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().Source.Write);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			DiffuseShader.Parameters["amount"].SetValue(colorDiffusion);
			Engine.Graphics.GraphicsDevice.Textures[0] = GetSet().Source.Read;
			RenderEffect(DiffuseShader);
			GetSet().Source.Swap();

			var player = Engine.Scene.Tracker.GetEntity<Player>();

			if(player != null && !(player.Scene as Level).Transitioning) {
				if(!onlyInfluenceWhileDashing || player.StateMachine.State == Player.StDash) {
					Splat(
						GetSet().Velocity,
						new Vector3(player.Speed * Engine.DeltaTime * playerVelocityInfluence, 0),
						player.Center,
						playerSizeInfluence
					);
				}

				if(!onlyDyeWhileDashing || player.StateMachine.State == Player.StDash) {
					Splat(
						GetSet().Source,
						(player.Hair.GetHairColor(0).ToVector3() * playerHairDyeFactor + GetDyeColor().ToVector3() * dyeBrightness) * Math.Min(1, player.Speed.Length()/playerSpeedForFullBrightness),
						player.Center,
						playerSizeInfluence
					);
				}
			}


			Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().Source.Write);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			Engine.Graphics.GraphicsDevice.Textures[0] = GetSet().Velocity.Read;
			Engine.Graphics.GraphicsDevice.Textures[1] = GetSet().Source.Read;
			AdvectionShader.Parameters["timestep"].SetValue(1000 * Engine.DeltaTime);
			AdvectionShader.Parameters["pixelSize"].SetValue(new Vector2(1f/bounds.Width, 1f/bounds.Height));
			RenderEffect(AdvectionShader);
			GetSet().Source.Swap(); 

			Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().Velocity.Write);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			Engine.Graphics.GraphicsDevice.Textures[0] = GetSet().Velocity.Read;
			Engine.Graphics.GraphicsDevice.Textures[1] = GetSet().Velocity.Read;
			AdvectionShader.Parameters["timestep"].SetValue(1000 * Engine.DeltaTime);
			AdvectionShader.Parameters["pixelSize"].SetValue(new Vector2(1f/bounds.Width, 1f/bounds.Height));
			RenderEffect(AdvectionShader);
			GetSet().Velocity.Swap(); 

			Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().DivergenceCurl);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			Engine.Graphics.GraphicsDevice.Textures[0] = GetSet().Velocity.Read;
			DivergenceCurlShader.Parameters["textureSize"].SetValue(new Vector2(bounds.Width, bounds.Height));
			RenderEffect(DivergenceCurlShader);

			Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().Velocity.Write);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			VorticityShader.Parameters["textureSize"].SetValue(new Vector2(bounds.Width, bounds.Height));
			Engine.Graphics.GraphicsDevice.Textures[0] = GetSet().Velocity.Read;
			Engine.Graphics.GraphicsDevice.Textures[1] = GetSet().DivergenceCurl;
			VorticityShader.Parameters["timestep"].SetValue(1000 * Engine.DeltaTime);
			VorticityShader.Parameters["curl"].SetValue(vorticity);
			RenderEffect(VorticityShader);
			GetSet().Velocity.Swap();

			for(var i = 0; i < pressureIterations; i++) {
				Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().Pressure.Write);
				Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
				JacobiShader.Parameters["textureSize"].SetValue(new Vector2(bounds.Width, bounds.Height));
				Engine.Graphics.GraphicsDevice.Textures[0] = GetSet().Pressure.Read;
				Engine.Graphics.GraphicsDevice.Textures[1] = GetSet().DivergenceCurl;
				RenderEffect(JacobiShader);
				GetSet().Pressure.Swap();
			}

			Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().Velocity.Write);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			GradientShader.Parameters["textureSize"].SetValue(new Vector2(bounds.Width, bounds.Height));
			Engine.Graphics.GraphicsDevice.Textures[0] = GetSet().Pressure.Read;
			Engine.Graphics.GraphicsDevice.Textures[1] = GetSet().Velocity.Read;
			RenderEffect(GradientShader);
			GetSet().Velocity.Swap();

			Engine.Graphics.GraphicsDevice.SetRenderTarget(GetSet().Display);
			Engine.Instance.GraphicsDevice.Clear(Color.Transparent);
			Engine.Graphics.GraphicsDevice.Textures[0] = GetSet().Source.Read;
			Engine.Graphics.GraphicsDevice.Textures[1] = GetSet().Velocity.Read;
			RenderEffect(DisplayShader);
		}

		public void BeforeRender() {
			if(
				(Engine.Scene as Level) != null &&
				!(Engine.Scene as Level).FrozenOrPaused &&
				(Engine.Scene as Level).unpauseTimer <= 0 &&
				!duplicate
			) {
				BeginSpriteBatch();
				UpdateTextures();
				EndSpriteBatch();
			}
		}

        public override void Render()
		{
			if(!RenderTargetSets.ContainsKey(EntityId))
				return;

			if(GetSet().Display != null) {
				Draw.SpriteBatch.Draw(GetSet().Display, Position, Color.White);
			}
		}

		public class DoubleRenderTarget2D {
			public RenderTarget2D Read;
			public RenderTarget2D Write;

			public DoubleRenderTarget2D(int width, int height) {
				Read  = new RenderTarget2D(Engine.Instance.GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
				Write = new RenderTarget2D(Engine.Instance.GraphicsDevice, width, height, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
			}

            public void Swap()
				=> (Write, Read) = (Read, Write);
        }

		public class MeshData {
			public VertexPositionTexture[] Positions;
			public short[] Indices;

			public MeshData(VertexPositionTexture[] positions, short[] indices) {
				Positions = positions;
				Indices = indices;
			}

			public static MeshData CreatePlane(float width, float height) {
				var vertices = new VertexPositionTexture[4];

				vertices[0].Position = new Vector3(0,     0,      0);
				vertices[1].Position = new Vector3(width, 0,      0);
				vertices[2].Position = new Vector3(0,     height, 0);
				vertices[3].Position = new Vector3(width, height, 0);

				vertices[0].TextureCoordinate = new Vector2(0, 0);
				vertices[1].TextureCoordinate = new Vector2(1, 0);
				vertices[2].TextureCoordinate = new Vector2(0, 1);
				vertices[3].TextureCoordinate = new Vector2(1, 1);

				return new MeshData(vertices, [0, 1, 2, 3, 2, 1]);
			}
		}
    }
}