using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Options.Miscellaneous {
    [GooberHelperOption]
    public class AlwaysExplodeSpinners : AbstractOption {
        [ILHook(typeof(CrystalStaticSpinner), "OnPlayer")]
        [ILHook("FrostTempleHelper", "FrostHelper.CustomSpinner", "OnPlayer")]
        [ILHook("VivHelper", "VivHelper.Entities.CustomSpinner", "OnPlayer")]
        private static void makeSpinnerMaybeExplode(ILContext il) {
            var cursor = new ILCursor(il);

            var afterReturnLabel = cursor.DefineLabel();

            cursor.EmitLdarg0();
            cursor.EmitDelegate(maybeDestroy);
            cursor.EmitBrfalse(afterReturnLabel);
            cursor.EmitRet();
            cursor.MarkLabel(afterReturnLabel);
        }

        private static bool maybeDestroy(Entity entity) {
            if(!GetOptionBool(Option.AlwaysExplodeSpinners))
                return false;

            entity.GetType()
                .GetMethod("Destroy", Utils.BindingFlagsAll)
                .Invoke(entity, [false]);
            
            return true;
        }
    }
}