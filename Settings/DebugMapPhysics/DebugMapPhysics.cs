using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Infrastructure.Modes;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings.DebugMapPhysics {
    [GooberHelperSetting]
    public class DebugMapPhysics : SettingToggle {
        private ExplodingDescription description;

        public override void OnValueChange(bool value) {
            base.OnValueChange(value);

            if(value)
                description.Explode();
            else
                description.Unexplode();
        }

        public override void CreateEntry(TextMenu menu, bool inGame) {
            base.CreateEntry(menu, inGame);

            Entry.AddExplodingDescription(menu, Dialog.Clean("menu_gooberhelper_setting_description_DebugMapPhysics"));

            description = menu.items[^1] as ExplodingDescription;
        }
    }
}