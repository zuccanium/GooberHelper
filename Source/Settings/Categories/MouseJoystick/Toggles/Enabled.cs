using System;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Toggles;

namespace Celeste.Mod.GooberHelper.Settings.Categories.MouseJoystick.Toggles {
    [GooberHelperSetting]
    public class Enabled : AbstractToggle {
        public override void CreateEntry(object container, bool inGame) {
            base.CreateEntry(container, inGame);

            Console.WriteLine("iuhwed");
        }
    }
}