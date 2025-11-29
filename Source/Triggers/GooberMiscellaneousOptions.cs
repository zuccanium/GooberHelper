using Celeste.Mod.Entities;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Triggers {

    [CustomEntity("GooberHelper/GooberMiscellaneousOptions")]
    [Tracked(false)]
    public class GooberMiscellaneousOptions : AbstractTrigger<GooberMiscellaneousOptions> {
        public GooberMiscellaneousOptions(EntityData data, Vector2 offset) : base(data, offset, OptionType.Boolean, [
            "AlwaysExplodeSpinners",
            "GoldenBlocksAlwaysLoad",
            "ShowActiveSettings",
        ], []) {}

        [OnLoad]
        public static new void Load()
            => AbstractTrigger<GooberMiscellaneousOptions>.Load();

        [OnUnload]
        public static new void Unload()
            => AbstractTrigger<GooberMiscellaneousOptions>.Unload();
    }
}