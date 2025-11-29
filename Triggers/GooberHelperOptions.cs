using System.Linq;
using Celeste.Mod.Entities;
using Celeste.Mod.GooberHelper.Attributes.Hooks;

namespace Celeste.Mod.GooberHelper.Triggers {
    [CustomEntity("GooberHelper/GooberHelperOptions")]
    [Tracked(false)]
    public class GooberHelperOptions : Trigger {
        public OptionChanges Changes;
        private bool revertOnLeave;
        private bool revertOnDeath;

        public GooberHelperOptions(EntityData data, Vector2 offset) : base(data, offset) {
            Changes = new OptionChanges(data);

            revertOnLeave = data.Bool("revertOnLeave");
            revertOnDeath = data.Bool("revertOnDeath");
        }

        public bool RemoveFromStack(bool update = true) {
            var countBefore = GooberHelperModule.Session.Stack.Count; 

            GooberHelperModule.Session.Stack.RemoveAll(item => item.ID.Key == Changes.ID.Key);

            var changed = countBefore != GooberHelperModule.Session.Stack.Count;

            if(update && changed)
                OptionChanges.UpdateStack();

            return changed;
        }

        public void AddToStack() {
            if(!revertOnLeave && !revertOnDeath) GooberHelperModule.Session.Stack.Clear();

            GooberHelperModule.Session.Stack.Add(Changes);
            Changes.Apply();
        }

        public override void Removed(Scene scene) {
            base.Removed(scene);
            
            if(revertOnDeath)
                RemoveFromStack();
        }

        public override void SceneEnd(Scene scene) {
            base.SceneEnd(scene);

            if(revertOnDeath)
                RemoveFromStack();
        }

        [OnHook]
        private static PlayerDeadBody patch_Player_Die(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats) {
            var didAnything = false;

            foreach(GooberHelperOptions item in Engine.Scene.Tracker.GetEntities<GooberHelperOptions>())
                if(item.revertOnDeath && item.RemoveFromStack(false))
                    didAnything = true;

            if(didAnything)
                OptionChanges.UpdateStack();

            return orig(self, direction, evenIfInvincible, registerDeathInStats);
        }

        public override void OnEnter(Player player) {
            base.OnEnter(player);

            if(GooberHelperModule.Session.Stack.Count > 0 && GooberHelperModule.Session.Stack.Last() == Changes)
                return;

            AddToStack();
        }

        public override void OnLeave(Player player) {
            base.OnLeave(player);
            
            if(!revertOnLeave || player.Dead)
                return;

            RemoveFromStack();
        }
    }
}