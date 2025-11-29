using System;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

//cyan -> just pressed (instant)
//green -> good (low delay)
//red -> way too early (high delay)

namespace Celeste.Mod.GooberHelper.Options.Miscellaneous {
    public static class BufferDelayVisualization {
        public class BufferOffsetIndicator : Entity {
            private Image image;
            private Color color;
            private float timer = 0f;

            public BufferOffsetIndicator(VirtualButton button, Player player) : base() {
                Position = player.Center;
                timer = 1f;
                Depth = Depths.Above;

                var counter = button.bufferCounter;

                if(button == Input.Dash)
                    counter = Math.Max(counter, Input.CrouchDash.bufferCounter);

                color = counter == button.BufferTime
                    ? Color.Turquoise
                    : Color.Lerp(Color.Red, Color.Lime, counter / button.BufferTime);

                image = new Image(button == Input.Jump ? GFX.Game["GooberHelper/jumpBufferIndicator"] : GFX.Game["GooberHelper/dashBufferIndicator"])
                    .SetColor(color)
                    .CenterOrigin();
                
                image.FlipX = player.Facing == Facings.Left;

                Add(image);
            }

            public override void Update() {
                base.Update();

                timer -= Engine.DeltaTime;
                image.Color = color * Math.Max(0.5f, timer);

                if(timer >= 0f)
                    return;
                    
                image.Scale = Vector2.One * Calc.LerpClamp(1f, 0f, Ease.BackIn(-timer * 4f));

                if(image.Scale.X <= 0f)
                    RemoveSelf();
            }
        }

        [ILHook(typeof(Player), "Jump")]
        [ILHook(typeof(Player), "SuperJump")]
        [ILHook(typeof(Player), "SuperWallJump")]
        [ILHook(typeof(Player), "StartDash")]
        [ILHook(typeof(Player), "SwimUpdate")]
        [ILHook(typeof(Player), "BoostUpdate")]
        [ILHook(typeof(Player), "HitSquashUpdate")]
        [ILHook(typeof(Player), "WallJump")]
        private static void hookConsumeBuffer(ILContext il) {
            var cursor = new ILCursor(il);
            var consumeBufferMethod = typeof(VirtualButton).GetMethod("ConsumeBuffer");
            var consumePressMethod = typeof(VirtualButton).GetMethod("ConsumePress"); //this is only used in boostupdate 😭

            if(
                cursor.TryGotoNext(MoveType.AfterLabel, instr => instr.MatchCallOrCallvirt(consumePressMethod)) ||
                cursor.TryGotoNext(MoveType.AfterLabel, instr => instr.MatchCallOrCallvirt(consumeBufferMethod))
            ) {
                cursor.EmitDup();
                cursor.EmitLdarg0();
                cursor.EmitDelegate(addIndicator);
            }
        }

        private static void addIndicator(VirtualButton button, Player player) {
            if(!GetOptionBool(Option.BufferDelayVisualization))
                return;

            if(Engine.Scene is Level)
                Engine.Scene.Add(new BufferOffsetIndicator(button, player));
        }
    }
}