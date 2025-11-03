using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Celeste.Mod.GooberHelper.ModIntegration;
using Celeste.Mod.GooberHelper.UI;
using Celeste.Mod.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;
using NLua;

#nullable enable

//look i know this is really stupid
//this the BulletTemplate class should definitely exist as a nested class in Bullet
//but for SOME REASON lua really doesnt like it when i try to do that
//it doesnt even give me a specific error; the coroutine just doesnt give a successful
//result and then it fails to even call the error function in lua because message
//argument passed into it is null or something
//idk ill just work with this until i find a convenient #code_modding post from someone
//with a similar issue and a really helpful response from snip probably
namespace Celeste.Mod.GooberHelper {
    //sorry for all the constant code repetition
    //i know any logical programmer should only have to declare these things once or twice
    //and while i had it like that before, the performance cost of having to grab data
    //from luatables at most twice whenever you create a bullet really really sucked
    //it was like 0.0004ms for a single constructor
    //10 OF THOSE TAKES UP 1/4TH OF THE ENTIRE UPDATE TIME!!! WHAT THE FUCK!!!!
    //the rest of this code will probably not have as many comments
    //enjoy my post-debugging-terrible-shit messages of nincompoopery
    public class BulletTemplate {
        public Vector2? Velocity;
        public Vector2? Acceleration;
        public Color? Color;
        public string? Texture;
        public float? Scale;
        public string? Effect;
        public bool? Additive;
        public bool? LowResolution;
        public float? Rotation;
        public float? ColliderRadius;
        public int? CullDistance = 10;
        public Vector2? RotationCenter = Vector2.Zero;
        public float? PositionRotationSpeed = 0f;
        public float? VelocityRotationSpeed = 0f;
        public float? Friction = 0f;

        public void ApplyToBullet(Entities.Bullet bullet) {
            if(Velocity is not null) bullet.Velocity = (Vector2)Velocity;
            if(Acceleration is not null) bullet.Acceleration = (Vector2)Acceleration;
            if(Color is not null) bullet.Color = (Color)Color;
            if(Texture is not null) bullet.Texture = Texture;
            if(Scale is not null) bullet.Scale = (float)Scale;
            if(Effect is not null) bullet.Effect = Effect;
            if(Additive is not null) bullet.Additive = (bool)Additive;
            if(LowResolution is not null) bullet.LowResolution = (bool)LowResolution;
            if(Rotation is not null) bullet.Rotation = (float)Rotation;
            if(ColliderRadius is not null) bullet.ColliderRadius = (float)ColliderRadius;
            if(CullDistance is not null) bullet.CullDistance = (int)CullDistance;
            if(RotationCenter is not null) bullet.RotationCenter = (Vector2)RotationCenter;
            if(PositionRotationSpeed is not null) bullet.PositionRotationSpeed = (float)PositionRotationSpeed;
            if(VelocityRotationSpeed is not null) bullet.VelocityRotationSpeed = (float)VelocityRotationSpeed;
            if(Friction is not null) bullet.Friction = (float)Friction;
        }

        //i am so sorry
        //i am so so sorry
        //please forgive me

        public void ApplyToBulletTemplate(BulletTemplate template) {
            if(Velocity is not null) template.Velocity = Velocity;
            if(Acceleration is not null) template.Acceleration = Acceleration;
            if(Color is not null) template.Color = Color;
            if(Texture is not null) template.Texture = Texture;
            if(Scale is not null) template.Scale = Scale;
            if(Effect is not null) template.Effect = Effect;
            if(Additive is not null) template.Additive = Additive;
            if(LowResolution is not null) template.LowResolution = LowResolution;
            if(Rotation is not null) template.Rotation = Rotation;
            if(ColliderRadius is not null) template.ColliderRadius = ColliderRadius;
            if(CullDistance is not null) template.CullDistance = CullDistance;
            if(RotationCenter is not null) template.RotationCenter = RotationCenter;
            if(PositionRotationSpeed is not null) template.PositionRotationSpeed = PositionRotationSpeed;
            if(VelocityRotationSpeed is not null) template.VelocityRotationSpeed = VelocityRotationSpeed;
            if(Friction is not null) template.Friction = Friction;
        }

        public BulletTemplate(
            Vector2? velocity,
            Vector2? acceleration,
            Color? color,
            string? texture,
            object? scale,
            string? effect,
            object? additive,
            object? lowResolution,
            object? rotation,
            object? colliderRadius,
            object? cullDistance,
            Vector2? rotationCenter,
            object? positionRotationSpeed,
            object? velocityRotationSpeed,
            object? friction
        ) {
            if(velocity is not null) Velocity = (Vector2)velocity;
            if(acceleration is not null) Acceleration = (Vector2)acceleration;
            if(color is not null) Color = (Color)color;
            if(texture is not null) Texture = texture;
            if(scale is not null) Scale = (float)(double)scale;
            if(effect is not null) Effect = effect;
            if(additive is not null) Additive = (bool)additive;
            if(lowResolution is not null) LowResolution = (bool)lowResolution;
            if(rotation is not null) Rotation = (float)(double)rotation / 180f * MathF.PI;
            if(colliderRadius is not null) ColliderRadius = (float)(double)colliderRadius;
            if(cullDistance is not null) CullDistance = (int)(double)cullDistance;
            if(rotationCenter is not null) RotationCenter = (Vector2)rotationCenter;
            if(positionRotationSpeed is not null) PositionRotationSpeed = (float)(double)positionRotationSpeed;
            if(velocityRotationSpeed is not null) VelocityRotationSpeed = (float)(double)velocityRotationSpeed;
            if(friction is not null) Friction = (float)(double)friction;
        }

        public static BulletTemplate operator+(BulletTemplate a, BulletTemplate b) {
            var clone = (BulletTemplate)a.MemberwiseClone(); //thank you c#

            b.ApplyToBulletTemplate(clone);

            return clone;
        }
    }
}


namespace Celeste.Mod.GooberHelper.Entities {
    [Tracked(false)]
    public class Bullet : Entity {
        public enum BulletRotationMode {
            None,
            Velocity,
            PositionChange
        }

        public Level level;

        public BulletActivator Parent;
        public Vector2 Velocity = Vector2.Zero;
        public Vector2 Acceleration = Vector2.Zero;
        public Color Color = Color.White;
        public string Texture = "bullets/GooberHelper/arrow";
        public float Scale = 1f;
        public string Effect = "coloredBullet";
        public bool Additive = false;
        public bool LowResolution = false;
        public float Rotation = 0f;
        public BulletRotationMode RotationMode = BulletRotationMode.PositionChange;
        public int CullDistance = 10;
        public Vector2 RotationCenter = Vector2.Zero;
        public float PositionRotationSpeed = 0f;
        public float VelocityRotationSpeed = 0f;
        public float Friction = 0f;

        //evil
        public new Vector2 Position {
            get => base.Position - Parent.BulletFieldCenter;
            set => base.Position = value + Parent.BulletFieldCenter;
        }

        public float ColliderRadius {
            get => (PlayerCollider.Collider as Circle)!.Radius;
            set => (PlayerCollider.Collider as Circle)!.Radius = value;
        }

        public Vector2 ActualPosition {
            get => base.Position;
            set => base.Position = value;
        }

        public PlayerCollider PlayerCollider;

        public Bullet(
            BulletActivator parent,
            BulletTemplate? template,
            Vector2? position,
            Vector2? velocity,
            Vector2? acceleration,
            Color? color,
            string? texture,
            object? scale,
            string? effect,
            object? additive,
            object? lowResolution,
            object? rotation,
            object? colliderRadius,
            object? cullDistance,
            Vector2? rotationCenter,
            object? positionRotationSpeed,
            object? velocityRotationSpeed,
            object? friction
        ) : base(parent.BulletFieldCenter + position ?? Vector2.Zero) {
            parent.Scene.Add(this);
            Parent = parent;
            level = (parent.Scene as Level)!;

            template?.ApplyToBullet(this);

            Add(PlayerCollider = new PlayerCollider(onCollidePlayer, new Circle(2)));

            if(velocity is not null) Velocity = (Vector2)velocity;
            if(acceleration is not null) Acceleration = (Vector2)acceleration;
            if(color is not null) Color = (Color)color;
            if(texture is not null) Texture = texture;
            if(scale is not null) Scale = (float)(double)scale;
            if(effect is not null) Effect = effect;
            if(additive is not null) Additive = (bool)additive;
            if(lowResolution is not null) LowResolution = (bool)lowResolution;
            if(rotation is not null) Rotation = (float)(double)rotation / 180f * MathF.PI;
            if(colliderRadius is not null) ColliderRadius = (float)(double)colliderRadius;
            if(cullDistance is not null) CullDistance = (int)(double)cullDistance;
            if(rotationCenter is not null) RotationCenter = (Vector2)rotationCenter;
            if(positionRotationSpeed is not null) PositionRotationSpeed = (float)(double)positionRotationSpeed;
            if(velocityRotationSpeed is not null) VelocityRotationSpeed = (float)(double)velocityRotationSpeed;
            if(friction is not null) Friction = (float)(double)friction;
        }

        public void InterpolateValue(string key, object to, float time = 1f, Ease.Easer? easer = null) {
            if(typeof(Bullet).GetField(key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) is not FieldInfo fieldInfo) 
                throw new Exception("failed to find fieldinfo of value to interpolate");

            if(key == "Position" && to is Vector2 toVector)
                to = toVector + Parent.BulletFieldCenter;

            var from = fieldInfo.GetValue(this);

            //this has to be my favorite type of syntax ever
            object difference = (from, to) switch {
                (Vector2 fromValue, Vector2 toValue) => toValue - fromValue,
                (Color fromValue, Color toValue) => new Color(toValue.ToVector4() - fromValue.ToVector4()),
                (float fromValue, float toValue) => toValue - fromValue,
                (float fromValue, double toValue) => (float)(toValue - fromValue),
                (float fromValue, int toValue) => toValue - fromValue,
                _ => throw new Exception("from and to values in interpolation dont match types or they arent valid types to interpolate"),
            };

            Add(new Coroutine(interpolateCoroutine(fieldInfo, difference, time, easer ?? Ease.SineInOut), true));
        }

        private IEnumerator interpolateCoroutine<T>(FieldInfo fieldInfo, T difference, float time, Ease.Easer easer) {
            var dx = 0.001f;
            var progress = 0f;

            while(progress < 1f) {
                var derivative = (easer(progress + dx) - easer(progress - dx))/dx * Engine.DeltaTime;
                var current = fieldInfo.GetValue(this);

                //super attractive pattern matching
                object newValue = (difference, current) switch {
                    (Vector2 diff, Vector2 curr) => curr + diff * derivative,
                    (Color diff, Color curr) => new Color(curr.ToVector4() + (diff * derivative).ToVector4()),
                    (float diff, float curr) => curr + diff * derivative,
                    _ => throw new Exception("interpolation difference doesnt match value type"),
                };

                fieldInfo.SetValue(this, newValue);
                
                progress += Engine.DeltaTime / time;
                yield return null;
            }

            yield break;
        }

        public override void Update() {
            Vector2 oldPosition = Position;

            base.Update();

            base.Position += Velocity * Engine.DeltaTime;
            Velocity += Acceleration * Engine.DeltaTime;
            Velocity *= 1 - Friction * Engine.DeltaTime;

            var RotationCenteredPosition = Position - RotationCenter;
            
            if(PositionRotationSpeed != 0) {
                Position = new Vector2(
                    RotationCenteredPosition.X * MathF.Cos(PositionRotationSpeed * Engine.DeltaTime) + RotationCenteredPosition.Y * MathF.Sin(PositionRotationSpeed * Engine.DeltaTime),
                    -RotationCenteredPosition.X * MathF.Sin(PositionRotationSpeed * Engine.DeltaTime) + RotationCenteredPosition.Y * MathF.Cos(PositionRotationSpeed * Engine.DeltaTime)
                ) + RotationCenter;
            }
            if(VelocityRotationSpeed != 0) {
                Velocity = new Vector2(
                    Velocity.X * MathF.Cos(VelocityRotationSpeed * Engine.DeltaTime) + Velocity.Y * MathF.Sin(VelocityRotationSpeed * Engine.DeltaTime),
                    -Velocity.X * MathF.Sin(VelocityRotationSpeed * Engine.DeltaTime) + Velocity.Y * MathF.Cos(VelocityRotationSpeed * Engine.DeltaTime)
                );
            }

            Rotation = RotationMode switch {
                BulletRotationMode.Velocity => Rotation = Velocity.Angle() + MathF.PI / 2,
                BulletRotationMode.PositionChange => Rotation = (Position - oldPosition).Angle() + MathF.PI / 2,
                _ => Rotation,
            };

            if(
                base.Position.X < level.Bounds.Left - CullDistance ||
                base.Position.X > level.Bounds.Right + CullDistance ||
                base.Position.Y > level.Bounds.Bottom + CullDistance ||
                base.Position.Y < level.Bounds.Top - CullDistance
            ) {
                RemoveSelf();
            }
        }

        private void onCollidePlayer(Player player) {
            // player.Die((player.Position - Position).SafeNormalize());

            player.Play("event:/char/madeline/death");

            RemoveSelf();
        }

        public static void BeginRender(bool lowResolution, string effectName, bool additive) {
            if(Engine.Scene is not Level level)
                return;
            
            if(lowResolution)
                GameplayRenderer.End();
            else
                Draw.SpriteBatch.End();

            var camera = level.GameplayRenderer.Camera;

            var effect = effectName != "" ? FrostHelperAPI.GetEffectOrNull.Invoke(effectName) : null;
            var matrix = camera.Matrix;

            if(!lowResolution)
                matrix *= Matrix.CreateScale(6);

            Draw.SpriteBatch.Begin(
                SpriteSortMode.Deferred,
                additive ? BlendState.Additive : BlendState.AlphaBlend,
                SamplerState.PointWrap,
                DepthStencilState.None,
                RasterizerState.CullNone,
                effect,
                matrix
            );

            if(effect is not null) {
                FrostHelperAPI.ApplyStandardParameters(effect, matrix);
                
                effect.CurrentTechnique = effect.Techniques["Shader"];
            }

            HighResolutionBulletRenderer.RenderState_Additive = additive;
            HighResolutionBulletRenderer.RenderState_Effect = effectName;
        }

        public static void EndRender(bool lowResolution) {
            Draw.SpriteBatch.End();
            
            if(lowResolution)
                GameplayRenderer.Begin();
            else
                HighResolutionBulletRenderer.BeginRender();
        }

        public override void Render() {
            if(Engine.Scene is not Level || (HighResolutionBulletRenderer.DontRender && !LowResolution))
                return;

            //the high resolution bullets are all drawn together
            //in that situation, only change buffer stuff if the "render state" is different from the last render
            if(
                LowResolution ||
                HighResolutionBulletRenderer.RenderState_Additive != Additive ||
                HighResolutionBulletRenderer.RenderState_Effect != Effect
            ) BeginRender(LowResolution, Effect, Additive); 

            //actual rendering
            base.Render();
            
            GFX.Game[Texture].DrawCentered(this.ActualPosition, this.Color, this.Scale, this.Rotation);
            
            if(LowResolution)
                EndRender(true);
        }

        public override void DebugRender(Camera camera) {
            if(Engine.Scene is not Level || (HighResolutionBulletRenderer.DontRender && !LowResolution))
                return;

            // Console.WriteLine("f");

            base.DebugRender(camera);

            // GFX.Game[Texture].DrawCentered(this.ActualPosition, this.Color, this.Scale * 10, this.Rotation + MathF.PI);
        }
    }
}