using Celeste.Mod.Entities;
using System;

namespace Celeste.Mod.GooberHelper.Triggers {
    [CustomEntity("GooberHelper/ZeroFrameProtector")]
    [Tracked(false)]
    public class ZeroFrameProtector : Trigger {
        public enum Mode {
            Left,
            Right,
            Up,
            Down
        }

        private Mode mode;
        private string flag;
        private string notFlag;
        
        private Vector2 lastPlayerPosition;
        private bool disabled = false;
        private Hitbox actualHitbox;

        public ZeroFrameProtector(EntityData data, Vector2 offset) : base(data, offset) {
            mode = Enum.Parse<Mode>(data.Attr("mode", "Left"));
            flag = data.Attr("flag", "");
            notFlag = data.Attr("notFlag", "");

            actualHitbox = new Hitbox(Width, Height);

            Collider.Position -= Vector2.One;
            Collider.Height += 2;
            Collider.Width += 2;
        }

        public override void Update() {
            base.Update();

            lastPlayerPosition = Engine.Scene.Tracker.GetEntity<Player>()?.TopLeft ?? Vector2.Zero;
        }

        public override void DebugRender(Camera camera) {            
            base.DebugRender(camera);

            var collider = Collider;
            Collider = actualHitbox;

            Draw.HollowRect(Collider, Color.Red * (disabled ? 0.5f : 1f));

            Collider = collider;
        }

        public override void OnStay(Player player) {
            base.OnStay(player);

            if(disabled) return;

            if(flag != "" && !player.level.Session.GetFlag(flag))
                return;

            if(notFlag != "" && player.level.Session.GetFlag(flag))
                return;

            var playerCollider = player.Collider;
            player.Collider = player.hurtbox;

            var left = Left - player.Right + 1;
            var right = Right - player.Left - 1;
            var top = Top - player.Bottom + 1;
            var bottom = Bottom - player.Top - 1;

            var leftEntry = lastPlayerPosition.X + player.Collider.Width <= Left;
            var rightEntry = lastPlayerPosition.X >= Right;
            var topEntry = lastPlayerPosition.Y - player.Collider.Height <= Top;
            var bottomEntry = lastPlayerPosition.Y >= Bottom;

            var collides = player.CollideCheck(this);

            player.Collider = playerCollider;

            if(!collides) return;

            if(mode == Mode.Left && leftEntry)
                player.MoveH(left);

            else if(mode == Mode.Right && rightEntry)
                player.MoveH(right);
            
            else if(mode == Mode.Up && topEntry)
                player.MoveV(top);
            
            else if(mode == Mode.Down && bottomEntry)
                player.MoveV(bottom);

            disabled = true;
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);

            disabled = false;
        }
    }
}