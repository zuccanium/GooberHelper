using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GooberHelper.Options;
using Celeste.Mod.GooberHelper.UI.OptionSliderContent;

namespace Celeste.Mod.GooberHelper.UI {
    // public class OuiGooberHelperOptions : Oui {
    //     private TextMenu menu = null;
    //     private static TextMenu backgroundMenu = null;
    //     private static bool inGame = true;
    //     private static Dictionary<string, TextMenuExt.ButtonExt> stringToCategoryButton = [];
    //     private static Dictionary<TextMenuExt.EnumerableSlider<float>, Option> sliderToOption = [];

    //     private static string queuedOptionsProfileName;
    //     private static bool wasAllowingHudHide = true;
    //     private static bool wasPauseMainMenuOpen = true;
    //     private static int optionsProfileStartIndex;
    //     private static TextMenuGooberExt.Combo combo;
    //     private static TextMenuExt.Modal comboModal;

    //     public OuiGooberHelperOptions() : base() {}

    //     private static void createOnPauseAction(TextMenu menu)
    //         => menu.OnPause = () => {
    //             menu.Close();

    //             GooberHelperModule.Instance.SaveSettings();

    //             (Engine.Scene as Level).Paused = false;
    //             (Engine.Scene as Level).AllowHudHide = wasAllowingHudHide;
    //             (Engine.Scene as Level).unpauseTimer = 0.15f;

    //             Audio.Play(SFX.ui_main_button_back);
    //         };
        

    //     public override IEnumerator Enter(Oui from) {
    //         menu = CreateMenu();
    //         Visible = true;
    //         menu.Focused = true;

    //         yield break;
    //     }

    //     public override IEnumerator Leave(Oui from) {
    //         Visible = false;
    //         menu.RemoveSelf();
    //         menu = null;

    //         yield break;
    //     }
    // } 
}