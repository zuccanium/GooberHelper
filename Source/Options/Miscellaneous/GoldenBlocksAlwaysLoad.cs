using Celeste.Mod.GooberHelper.Attributes;
using Celeste.Mod.GooberHelper.Attributes.Hooks;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

//todo: make this bgettert ioaejiojaeogaze si mtired

namespace Celeste.Mod.GooberHelper.Options.Miscellaneous {
    [GooberHelperOption]
    public class GoldenBlocksAlwaysLoad : AbstractOption {
        [ILHook(typeof(GoldenBlock), "Awake")]
        [ILHook("PlatinumStrawberry", "Celeste.Mod.PlatinumStrawberry.Entities.PlatinumBlock", "Awake")]
        [ILHook("CollabUtils2", "Celeste.Mod.CollabUtils2.Entities.SilverBlock", "Awake")]
        private static void makeGoldenBlocksOrSimilarEntitiesAlwaysLoad(ILContext il) {
            var cursor = new ILCursor(il);

            //iteration #1 is for the flag local boolean on golden and silver blocks
            //iteration #2 is for the flag2 local boolean on platinum blocks
            for(var i = 0; i < 2; i++) {
                if(cursor.TryGotoNext(MoveType.After,
                    instr => instr.MatchLdcI4(0),
                    instr => instr.MatchStloc(i)
                )) {
                    cursor.Index--;
                    cursor.EmitDelegate(overrideLoadThing);
                }
            }
        }

        private static float overrideLoadThing(int orig)
            => GetOptionBool(Option.GoldenBlocksAlwaysLoad)
                ? 1
                : orig;
    }
}