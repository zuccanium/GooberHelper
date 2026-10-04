using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class HyperAndSuperWithDashAttackJump : AbstractOption {
        public static bool BeforeJump(Player self) {
            if (!GetOptionBool(Option.HyperAndSuperWithDashAttackJump) || !self.DashAttacking)
                return false;
            
            self.SuperJump();
                
            return true;
        }
    }
}