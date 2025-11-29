using System;

namespace Celeste.Mod.GooberHelper.UI {
    public class TextMenuCombo : TextMenu.Item {
        public int Counter;
        public float TimeSinceLastIncrease;
        public float ExpirationTime;
        public int Minimum;
        private float rainbowValue = 0;

        public static Action IncreaseCombo;

        public TextMenuCombo(float expirationTime = 2, int minimum = 1) : base() {
            ExpirationTime = expirationTime;
            Minimum = minimum;
        }

        public override void Update() {
            base.Update();

            if(Counter == 0) return;

            TimeSinceLastIncrease += Engine.DeltaTime;
            rainbowValue = (rainbowValue + Engine.DeltaTime * 2f) % 1f;

            if(TimeSinceLastIncrease > ExpirationTime) {
                Counter = 0;

                Visible = false;
            }
        }

        public override void Render(Vector2 position, bool highlighted) {
            base.Render(position, highlighted);

            if(!Visible || Counter < Minimum) return;

            var fade = 1 - (TimeSinceLastIncrease > ExpirationTime / 2 ? 2 * (TimeSinceLastIncrease - ExpirationTime / 2) / ExpirationTime : 0);

            var pop = MathHelper.Lerp(1.5f, 1f, Ease.ExpoOut(TimeSinceLastIncrease));

            var rainbow = Calc.HsvToColor(rainbowValue, 0.5f, 1f);
            var rainbow2 = Calc.HsvToColor(rainbowValue, 0.0f, 1f);

            ActiveFont.DrawEdgeOutline(
                text: "x" + this.Counter,
                position: position + Vector2.UnitX.Rotate(Random.Shared.NextAngle()) * (pop - 1f) * 5f,
                justify: new Vector2(0.5f, 0.5f),
                scale: Vector2.One * 2f * pop,
                color: rainbow2 * fade,
                edgeDepth: 4f * pop,
                edgeColor: rainbow * fade * fade,
                stroke: 2f * pop,
                strokeColor: rainbow * fade * fade
            );
        }

        // public static void CreateComboModal(TextMenu menu, float expireTime = 1) {
        //     var label = new TextMenu.Header("") { Container = menu };
        //     var modal = new TextMenuExt.Modal(label, 85, 500) { Visible = false };

        //     var timeSinceLastInput = 0f;
        //     var counter = 0;

        //     menu.Add(modal);

        //     modal.OnUpdate = () => {
        //         if(counter == 0) return;

        //         timeSinceLastInput += Engine.DeltaTime;

        //         if(timeSinceLastInput > expireTime) {
        //             counter = 0;

        //             modal.Visible = false;
        //         }
        //     };

        //     IncreaseCombo = () => {
        //         counter++;
        //         modal.Visible = true;
        //         label.Title = "x" + counter;
        //     };
        // }

        public void Increase() {
            Counter++;
            Visible = true;
            TimeSinceLastIncrease = 0;
        }
    }
}