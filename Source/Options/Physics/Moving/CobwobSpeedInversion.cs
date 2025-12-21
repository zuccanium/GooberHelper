using Celeste.Mod.GooberHelper.Attributes;

namespace Celeste.Mod.GooberHelper.Options.Physics.Moving {
    [GooberHelperOption]
    public class CobwobSpeedInversion : AbstractOption {
        public override OptionGroup HeadGroup { get; set; } = OptionGroup.SpeedPreservation;

        public enum Value {
            None,
            RequireSpeed,
            WorkWithRetention
        }
        
        //implemented in GeneralHooks/Cobwob.cs
    }
}