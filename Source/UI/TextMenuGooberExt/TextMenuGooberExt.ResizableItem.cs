using System;

namespace Celeste.Mod.GooberHelper.UI.TextMenuGooberExt {
    public static partial class TextMenuGooberExt {
        public abstract class ResizableItem : TextMenu.Item {
            public Action ResizeContainer;
        }
    }
}