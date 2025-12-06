using System;
using Celeste.Mod.GooberHelper.UI.TextMenuGooberExt;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractColor : AbstractSetting {
        public virtual void OnValueChange(Color value)
            => SettingProperty.SetValue(ContainerObject, value);

        public override void CreateEntry(object container, bool inGame) {
            base.CreateEntry(container, inGame);

            Utils.Log($"creating color for {GetType()}");

            if(SettingProperty.GetValue(ContainerObject) is not Color value)
                throw new Exception("NOT THE CORRECT TYPE");

            var colorInput = new TextMenuGooberExt.ColorInput(GetName(), value);
            
            colorInput.OnValueChange += OnValueChange;
            
            Entry = colorInput;

            AddToContainer();
            AddDescription();
        }
    }
}