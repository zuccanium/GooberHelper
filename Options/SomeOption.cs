using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Celeste;
using Celeste.Mod;
using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using Celeste.Mod.GooberHelper.ModIntegration;
using IL.Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

#nullable enable

namespace Celeste.Mod.GooberHelper.GooberOptions {
    public class SomeOptionUsingTheAttributeThing : AbstractOption {
        // [ILHook(
        //     typeof(Player), "SuperWallJump",
        //     typeof(Player), "WallJump"
        // )]
        // private static void doStuffSingle(ILContext il) {}

        [ILHook(typeof(Player), "SuperWallJump")]
        [ILHook(typeof(Player), "WallJump")]
        private static void doStuffDouble(ILContext il) {
            Utils.Log($"HELLO FROM {il.Method}");
        }

        [OnHook]
        private static void patch_Player_WallJump(On.Celeste.Player.orig_WallJump orig, Player self, int dir) {
            Console.WriteLine("JWEFJIOWEJIF");

            orig(self, dir);
        }
    }
}