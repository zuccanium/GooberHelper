using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Settings.Toggles;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings.Root {
    [GooberHelperSetting]
    public class DebugMapPhysics : AbstractToggle {
        private TextMenuGooberExt.ExplodingDescription description;

        public override void OnValueChange(bool value) {
            base.OnValueChange(value);

            if(value)
                description.Explode();
            else
                description.Unexplode();
        }

        public override void CreateEntry(object container, bool inGame) {
            if(container is not TextMenu menu)
                return;

            Entry = CreateItem(GetValue());

            AddToContainer();

            Entry.AddExplodingDescription(menu, GetDescription());

            description = menu.items[^1] as TextMenuGooberExt.ExplodingDescription;
        }
    }
}