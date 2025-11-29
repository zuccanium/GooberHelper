using Celeste.Mod.Entities;
using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Triggers.LegacyOptionTriggers {
    [CustomEntity("GooberHelper/RetentionFrames")]
    [Tracked(false)]
    public class RetentionFrames : AbstractTrigger<RetentionFrames> {
        public RetentionFrames(EntityData data, Vector2 offset) : base(data, offset, OptionType.Float, ["RetentionFrames"], new Dictionary<string, string>() {{ "RetentionFrames", "RetentionLength" }}) {}

        [OnLoad]
        public static new void Load()
            => AbstractTrigger<RetentionFrames>.Load();

        [OnUnload]
        public static new void Unload()
            => AbstractTrigger<RetentionFrames>.Unload();
    }
}