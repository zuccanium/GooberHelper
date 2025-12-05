using System;

namespace Celeste.Mod.GooberHelper.Settings.Toggles.DebugMapPhysics {
    public static class DebugMapThings {
        public static MouseSmoother MouseSmoother = new();

        public static float AttractStrength;

        public static Color FluidStaticColor;
        public static Color FluidMovingColor;
        public static float FluidBlobSize;

        public static void RandomizeFluidColor()
            => FluidStaticColor = Calc.HsvToColor(Random.Shared.NextFloat(), Random.Shared.NextFloat(), Random.Shared.NextFloat()) * 0.5f;

        public static void ResetThings() {
            AttractStrength = 0;

            FluidStaticColor = new Color(0, 0.5f, 1f, 1f) * 0.5f;
            FluidMovingColor = new Color(1f, 1f, 1f, 1f) * 0.5f;
            FluidBlobSize = 5;
        }
    }
}