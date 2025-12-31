using System.Collections.Generic;
using Celeste.Mod.GooberHelper.Options;
using Celeste.Mod.GooberHelper.Triggers.LegacyOptionTriggers;

namespace Celeste.Mod.GooberHelper {
    public class GooberHelperModuleSession : EverestModuleSession {
        public Dictionary<string, List<StackItem>> Stacks { get; set; } = [];
        public List<OptionChanges> Stack { get; set; } = [];
        public Dictionary<Option, float> MapDefinedOptions { get; set; } = [];
    }
}