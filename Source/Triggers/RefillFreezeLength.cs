using Celeste.Mod.Entities;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Triggers {
    [CustomEntity("GooberHelper/RefillFreezeLength")]
    [Tracked(false)]
    public class RefillFreezeLength : AbstractTrigger<RefillFreezeLength> {
        public RefillFreezeLength(EntityData data, Vector2 offset) : base(data, offset, OptionType.Float, ["RefillFreezeLength"], []) {}

        [OnLoad]
        public static new void Load()
            => AbstractTrigger<RefillFreezeLength>.Load();

        [OnUnload]
        public static new void Unload()
            => AbstractTrigger<RefillFreezeLength>.Unload();
    }
}