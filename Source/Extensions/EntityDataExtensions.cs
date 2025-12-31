namespace Celeste.Mod.GooberHelper.Extensions {
    public static class EntityDataExtensions {
        public static Color HexColorSafe(this EntityData data, string key, Color defaultValue = default) {
            if(data.Values != null)
                return data.HexColor(key, defaultValue);

            return defaultValue;
        }
    }
}