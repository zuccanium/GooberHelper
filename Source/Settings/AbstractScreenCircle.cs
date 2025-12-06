using Celeste.Mod.GooberHelper.DataStructures;
using Celeste.Mod.GooberHelper.UI;

namespace Celeste.Mod.GooberHelper.Settings {
    public abstract class AbstractScreenCircle : AbstractSetting {
        public virtual void OnValueChange(Circle value) {
            Utils.Log($"value changed to {value.Position}, r={value.Radius}");
            
            SettingProperty.SetValue(ContainerObject, new DummyCircle(value));
        }

        public override void CreateEntry(object container, bool inGame) {
            base.CreateEntry(container, inGame);

            var value = SettingProperty.GetValue(ContainerObject);

            if(value is not DummyCircle dummyCircle) {
                Utils.Log("not valid value");

                return;
            }

            Utils.Log($"the dummy is <{dummyCircle.X}, {dummyCircle.Y}>, r={dummyCircle.Radius}");
            Utils.Log($"the converted dummy is {dummyCircle.ToCircle().Position}, r={dummyCircle.ToCircle().Radius}");

            var screenCircle = new TextMenuGooberExt.ScreenCircle(GetName(), dummyCircle.ToCircle());

            screenCircle.OnValueChange += OnValueChange;

            Entry = screenCircle;

            AddToContainer();
            AddDescription();
        }
    }
}