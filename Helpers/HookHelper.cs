using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace Celeste.Mod.GooberHelper.Helpers {
    public static class HookHelper {
        //monomod is weird
        //thank you snip (https://discord.com/channels/403698615446536203/908809001834274887/1442308183304441877)
        public static string ToStringSafe(this Instruction instr) {
            if(instr.OpCode.OperandType is not (OperandType.InlineBrTarget or OperandType.ShortInlineBrTarget))
                return instr.ToString();

            var fakeInstr = Instruction.Create(instr.OpCode, ((ILLabel)instr.Operand).Target);
            
            fakeInstr.Offset = instr.Offset;

            return fakeInstr.ToString();
        }
        
        private class QueueItem {
            public Delegate Action;
            public Instruction CursorTarget;
            public bool AfterLabels;
            public object[] Args;

            public QueueItem(Delegate action, params object[] args) {
                Action = action;
                CursorTarget = cursor.Next;
                AfterLabels = (bool)f_ILCursor_afterHandlerEnds.GetValue(cursor);
                Args = args;
            }

            public void Run() {
                cursor.Goto(CursorTarget);
                
                if(AfterLabels)
                    cursor.MoveAfterLabels();

                if(debug)
                    Utils.Log($"setting cursor index to ({cursor.Next.ToStringSafe()}) and running something...");

                Action.Method.Invoke(Action.Target, Args);
            }
        }

        private static FieldInfo f_ILCursor_afterHandlerEnds = typeof(ILCursor).GetField("_afterHandlerEnds", Utils.BindingFlagsAll);

        private static ILCursor cursor;
        private static string taskName;
        private static bool aborted;
        private static List<QueueItem> queue = [];
        private static bool debug;
        private static string originalILString;

        private static Dictionary<string, object> queuedState;

        public static void Begin(ILCursor cursor, string taskName, bool debug = false) {
            HookHelper.cursor = cursor;
            HookHelper.taskName = taskName;
            HookHelper.debug = debug;

            aborted = false;

            if(debug) {
                Utils.Log($"======= BEGINNING HOOK THING =======");
                Utils.Log($"task name: {taskName}");

                originalILString = cursor.Context.ToString();
            }
        }

        public static void End() {
            if(!aborted) {
                foreach(var item in queue)
                    item.Run();
            }

            if(debug) {
                Utils.Log("FINISHED");
                Console.WriteLine(Utils.CreateDiff(originalILString, cursor.Context.ToString()));
                Utils.Log($"======= ENDING HOOK THING =======");
            }
            
            cursor = null;
            taskName = null;
            aborted = false;
            debug = false;
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
                Utils.Log($"moved from ({previousInstruction.ToStringSafe()}) to ({cursor.Next.ToStringSafe()}) while {name}");
        }

        public static void Do(Action action)
            => actuallyDo(action);

        public static void Do<T1>(Action<T1> action, T1 stateValue1)
            => actuallyDo(action, stateValue1);

        public static void Do<T1, T2>(Action<T1, T2> action, T1 stateValue1, T2 stateValue2)
            => actuallyDo(action, stateValue1, stateValue2);

        private static void actuallyDo(Delegate action, params object[] stateValues) {
            if(aborted)
                return;

            queue.Add(new QueueItem(action, stateValues));
        }

        public static void Abort(string name) {
            Logger.Error("GooberHelper", $"failed to hook {cursor.Method.Name} while {taskName} (at {name})");

            if(debug)
                Utils.Log($"ABORTED!!! during {name}");
            
            aborted = true;
        }
    }
}