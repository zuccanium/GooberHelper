using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Settings.Root {
    [GooberHelperSetting]
    public class FastMenuing : AbstractToggle {
        public override void OnValueChange(bool value) {
            base.OnValueChange(value);

            UpdateFastMenuing();
        }

        [SubscribeToEvent(typeof(Everest.Events.Input), "OnInitialize")]
        public static void UpdateFastMenuing() {
            //menuleft isnt special but everything else will be null if it is
            if(Input.MenuLeft == null)
                return;

            var firstRepeatTime = GooberHelperModule.Settings.FastMenuing ? 0.2f : 0.4f;
            var multiRepeatTime = GooberHelperModule.Settings.FastMenuing ? 0.05f : 0.1f;

            Input.MenuLeft.SetRepeat(firstRepeatTime, multiRepeatTime);
            Input.MenuRight.SetRepeat(firstRepeatTime, multiRepeatTime);
            Input.MenuUp.SetRepeat(firstRepeatTime, multiRepeatTime);
            Input.MenuDown.SetRepeat(firstRepeatTime, multiRepeatTime);
        }

        [ILHook]
        private static void patch_Postcard_EaseIn(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.8f))) {
                cursor.EmitDelegate(getMultiplier);
                cursor.EmitMul();
            }
        }

        //this isnt a typo on my part; the method is actually just called that 😭
        [ILHook]
        private static void patch_Postcard_EaseButtinIn(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.75f)))
                cursor.EmitDelegate(overrideWithZero);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(2f))) {
                cursor.EmitDelegate(getMultiplier);
                cursor.EmitMul();
            }
        }

        [ILHook]
        private static void patch_Postcard_EaseOut(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallOrCallvirt<Engine>("get_DeltaTime"))) {
                cursor.EmitDelegate(getMultiplier);
                cursor.EmitMul();
            }
        }

        [ILHook]
        private static void patch_Postcard_DisplayRoutine(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(0.75f)))
                cursor.EmitDelegate(overrideWithZero);

            if(cursor.TryGotoNext(MoveType.After, instr => instr.MatchLdcR4(1.2f)))
                cursor.EmitDelegate(overrideWithZero);
        }

        private static float overrideWithZero(float orig)
            => GooberHelperModule.Settings.FastMenuing ? 0f : orig;

        private static float getMultiplier()
            => GooberHelperModule.Settings.FastMenuing ? 2f : 1f;
    }
}