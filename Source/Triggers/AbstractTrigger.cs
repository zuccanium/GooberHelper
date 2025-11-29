using System;
using System.Collections.Generic;
using System.Linq;

//deprecated

namespace Celeste.Mod.GooberHelper.Triggers {
    //this should absolutely go into the trigger class
    //however the trigger class takes a generic argument and i really dont want to deal with any weird results from doing that
    //sorry
    public class StackItem {
        public Dictionary<Option, float> SettingValues;
        public string Type;
        public int ID;

        public StackItem() {}

        public StackItem(Dictionary<Option, float> SettingValues, string Type, int ID) {
            this.SettingValues = SettingValues;
            this.Type = Type;
            this.ID = ID;
        }
    }
    public abstract class AbstractTrigger<T> : Trigger where T : Trigger {
        public Dictionary<Option, float> SettingValues = [];
        private bool revertOnLeave = false;
        private bool revertOnDeath = false;
        private StackItem stackItem;
        private int id;
        private string flag;
        private string notFlag;

        public static List<StackItem> Stack {
            get {
                if(!GooberHelperModule.Session.Stacks.ContainsKey(typeof(T).Name)) GooberHelperModule.Session.Stacks.Add(typeof(T).Name, new List<StackItem>());

                return GooberHelperModule.Session.Stacks[typeof(T).Name];
            }
            set {}
        }

        public AbstractTrigger(EntityData data, Vector2 offset, OptionType type, List<string> optionNames, Dictionary<string, string> optionNameOverrides) : base(data, offset) {
            if(revertOnDeath) {
                Stack.RemoveAll(a => a == stackItem);

                UpdateStack();
            }

            id = data.ID;

            foreach(var item in Stack.Where(a => a.ID == id))
                stackItem = item;

            foreach(var optionName in optionNames) {
                if(Enum.TryParse(optionNameOverrides.TryGetValue(optionName, out var actualOptionName) ? actualOptionName : optionName, out Option option)) {
                    var id = optionName[..1].ToLower() + optionName[1..];

                    SettingValues[option] = type == OptionType.Float ? data.Int(id, (int)OptionsManager.Options[option].DefaultValue) : (data.Bool(id, false) ? 1 : 0);
                } else {
                    HandleWeirdOption(optionName);
                }
            }

            revertOnDeath = data.Bool("revertOnDeath", false);
            revertOnLeave = data.Bool("revertOnLeave", false);

            flag = data.Attr("flag", "");
            notFlag = data.Attr("notFlag", "");
        }

        public virtual void HandleWeirdOption(string optionName) {}

        //i would just use the normal hooking method but these need to be called from the inheriting classes to respect the type argument
        //GRAHHHH i hate backwards compatibility code
        public static void Load()
            => On.Celeste.Player.Die += modPlayerDie;

        public static void Unload()
            => On.Celeste.Player.Die -= modPlayerDie;

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            
            if(revertOnDeath) {
                Stack.RemoveAll(a => a == stackItem);

                UpdateStack();
            }
        }

        public override void SceneEnd(Scene scene)
        {
            base.SceneEnd(scene);

            if(revertOnDeath) {
                Stack.RemoveAll(a => a == stackItem);

                UpdateStack();
            }
        }

        private static PlayerDeadBody modPlayerDie(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats) {
            foreach(AbstractTrigger<T> item in Engine.Scene.Tracker.GetEntities<T>()) {
                if(item.revertOnDeath)
                    Stack.RemoveAll(a => a == item.stackItem);

                item.UpdateStack();
            }

            return orig(self, direction, evenIfInvincible, registerDeathInStats);
        }

        public void UpdateStack() {
            if(Stack.Count == 0) {
                foreach(var item in this.SettingValues)
                    ResetOptionValue(item.Key, OptionSetter.Map);
            } else {
                foreach(var item in Stack.Last().SettingValues)
                    SetOptionValue(item.Key, item.Value, OptionSetter.Map);
            }
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);

            if(revertOnLeave && !player.Dead) {
                Stack.RemoveAll(a => a == this.stackItem);

                UpdateStack();
            }
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);

            if(!(
                (flag    != "" &&  (Engine.Scene as Level).Session.GetFlag(flag))   || (flag == "") &&
                (notFlag != "" && !(Engine.Scene as Level).Session.GetFlag(notFlag) || notFlag == "")
            )) return;

            stackItem ??= new StackItem(SettingValues, typeof(T).Name, id);

            if(!revertOnLeave && !revertOnDeath)
                Stack.Clear();

            Stack.Add(stackItem);

            UpdateStack();
        }
    }
}