using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.Extensions;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace Celeste.Mod.GooberHelper.Options.Physics.Jumping {
    [GooberHelperOption]
    public class BounceHelperBounceSpeedPreservation : AbstractOption {
        private static Vector2 beforeUpdateSpeed;

        [ILHook("BounceHelper", "Celeste.Mod.BounceHelper.BounceHelperModule", "bounce")]
        private static void boyoyoyoing(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg(1);
            cursor.EmitLdarg(2);
            cursor.EmitDelegate(overrideBounceSpeed);
            cursor.EmitStarg(2);
        }
        
        [ILHook]
        private static void patch_Player_Update(ILContext il) {
            var cursor = new ILCursor(il);

            cursor.EmitLdarg0();
            cursor.EmitDelegate(setOriginalSpeed);
        }

        private static Vector2 overrideBounceSpeed(Player player, Vector2 orig) {
            if(!GetOptionBool(Option.BounceHelperBounceSpeedPreservation))
                return orig;

            var sign = orig.Sign();
            var ext = player.GetExtensionFields();

            orig = orig.Abs();

            //down and vertical bounces
            if(orig.Y == 210 || orig.Y == 200)
                orig.Y = Utils.UnsignedAbsMax(
                    ext.DashStickyRetentionExists
                        ? ext.DashStickyRetentionSpeed.Y
                        : 0f,
                    beforeUpdateSpeed.Y,
                    orig.Y
                );

            //horizontal bounces
            else if(orig.X == 320)
                orig.X = Utils.UnsignedAbsMax(
                    ext.DashStickyRetentionExists
                        ? ext.DashStickyRetentionSpeed.X 
                        : 0f,
                    beforeUpdateSpeed.X,
                    orig.X
                );
            
            //diagonal bounces
            else
                orig = orig.MaxLengthInclusive(
                    new Vector2(
                        Utils.UnsignedAbsMax(
                            ext.DashStickyRetentionExists
                                ? ext.DashStickyRetentionSpeed.X 
                                : 0f,
                            beforeUpdateSpeed.X
                        ),
                        Utils.UnsignedAbsMax(
                            ext.DashStickyRetentionExists
                                ? ext.DashStickyRetentionSpeed.Y
                                : 0f,
                            beforeUpdateSpeed.Y
                        )
                    ).Length()
                );

            return orig * sign;
        }

        private static void setOriginalSpeed(Player player)
            => beforeUpdateSpeed = player.GetConservedSpeed();
    }
}