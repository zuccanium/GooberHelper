using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings.Toggles.DebugMapPhysics {
    [GooberHelperSetting]
    public class DebugMapPhysics : AbstractToggle {
        private ExplodingDescription description;

        public override void OnValueChange(bool value) {
            base.OnValueChange(value);

            if(value)
                description.Explode();
            else
                description.Unexplode();
        }

        public override void CreateEntry(object container, bool inGame) {
            base.CreateEntry(container, inGame);

            if(container is not TextMenu menu)
                return;

            Entry.AddExplodingDescription(menu, Dialog.Clean("menu_gooberhelper_setting_description_DebugMapPhysics"));

            description = menu.items[^1] as ExplodingDescription;
        }
    }
}