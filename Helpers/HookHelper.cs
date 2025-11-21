using System;
using System.Collections.Generic;
using Celeste.Mod;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Helpers {
    public static class HookHelper {
        private class QueueItem {
            public Mono.Cecil.Cil.Instruction CursorTarget;
            public Action Action;

            public QueueItem(Action action) {
                Action = action;
                CursorTarget = cursor.Next;
            }

            public void Run() {
                cursor.Goto(CursorTarget);

                if(debug)
                    Console.WriteLine($"setting cursor index to ({cursor.Next}) and running something...");

                Action();
            }
        }

        private static ILCursor cursor;
        private static string taskName;
        private static bool aborted = false;
        private static bool debug;
        private static List<QueueItem> queue = [];

        public static void Begin(ILCursor cursor, string taskName, bool debug = false) {
            HookHelper.cursor = cursor;
            HookHelper.taskName = taskName;
            HookHelper.debug = debug;

            if(debug)
                Console.WriteLine("======= BEGINNING HOOK THING =======");
        }

        public static void End() {
            if(!aborted) {
                foreach(var item in queue)
                    item.Run();
            }

            if(debug)
                Console.WriteLine($"""
                FINISHED DOING STUFF
                
                body:
                {cursor.Context}
                ======= ENDING HOOK THING =======
                """);
            
            cursor = null;
            taskName = null;
            aborted = false;
            queue.Clear();
        }

        public static void Move(string name, Action action) {
            if(aborted)
                return;

            var previousInstruction = cursor.Next;

            try {
                action();
            } catch {
                Abort(name);
            }
            
            if(debug)
                Console.WriteLine($"moved from ({previousInstruction}) to ({cursor.Next}) while {name}");
        }

        public static void Do(Action action) {
            if(aborted)
                return;

            queue.Add(new QueueItem(action));
        }

        public static void Abort(string name) {
            Logger.Error("GooberHelper", $"failed to hook ${cursor.Method.Name} while {taskName} (at {name})");
            
            aborted = true;
        }
    }
}