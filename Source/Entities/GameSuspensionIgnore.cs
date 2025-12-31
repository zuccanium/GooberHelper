using Celeste.Mod.Entities;
using Celeste.Mod.GooberHelper.ModImports;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.GooberHelper.Entities {
    [CustomEntity("GooberHelper/GameSuspensionIgnore")]
    [Tracked(false)]
    public class GameSuspensionIgnore : Entity {
        public List<Entity> Entities = [];
        public Dictionary<Type, bool> WhitelistedEntities = [];

        public GameSuspensionIgnore(EntityData data, Vector2 offset) : base(data.Position + offset) {
            foreach(var typeName in data.Attr("types", "").Split(",")) {
                if(typeName == "")
                    continue;

                var type = FrostHelper.EntityNameToType?.Invoke(typeName);

                if(type == null)
                    continue;

                WhitelistedEntities[type] = true;
            }

            Collider = new Hitbox(data.Width, data.Height);
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);

            foreach(var entity in Scene.Entities)
                if(Collider.Collide(entity.Position) && (WhitelistedEntities.Count == 0 || WhitelistedEntities.ContainsKey(entity.GetType())))
                    Entities.Add(entity);
        }

        public static void UpdateEntities() {
            foreach(GameSuspensionIgnore ignore in Engine.Scene.Tracker.GetEntities<GameSuspensionIgnore>())
                foreach(var entity in ignore.Entities)
                    entity.Update();
        }
    }
}