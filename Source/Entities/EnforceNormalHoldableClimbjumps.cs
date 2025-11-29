using Celeste.Mod.Entities;

//deprecated

namespace Celeste.Mod.GooberHelper.Entities {
    [CustomEntity("GooberHelper/EnforceNormalHoldableClimbjumps")]
    [Tracked(false)]
    public class EnforceNormalHoldableClimbjumps : Entity {
        public EnforceNormalHoldableClimbjumps(Vector2 position, int width, int height) : base(position)
            => Collider = new Hitbox(width, height, 0, 0);

        public EnforceNormalHoldableClimbjumps(EntityData data, Vector2 offset) : this(data.Position + offset, data.Width, data.Height) {}
    }
}