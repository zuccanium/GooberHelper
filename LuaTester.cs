using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.GooberHelper {
    public class LuaTester {
        public static void OverloadedMethod(int a) {
            Console.WriteLine("int overload called");
        }

        public static void OverloadedMethod(bool a) {
            Console.WriteLine("bool overload called");
        }

        public static void OverloadedMethod(string a) {
            Console.WriteLine("string overload called");
        }

        public static int OverloadedMethod(object a) {
            Console.WriteLine("object returning int overload called");

            return 1;
        }

        public static void OverloadedGenericMethod<T>(T tuh) {
            Console.WriteLine($"void <T> T called with type argument {typeof(T)}");
        }

        public static int OverloadedGenericMethod<T>(bool b) {
            Console.WriteLine($"int <T> bool called with type argument {typeof(T)}");

            return 1;
        }

        public static int OverloadedGenericMethod<T, U>(bool b) {
            Console.WriteLine($"int <T, U> bool called with type arguments {typeof(T)} and {typeof(U)}");

            return 1;
        }

        //chatgpt testers
        public static List<T> MakeList<T>(params T[] args) => [.. args];
        public static T Echo<T>(T value) => value;
        
        public class Nested {
            public static Dictionary<K, List<V>> MakeMap<K, V>(K key, V value)
                => new() { [key] = new List<V> { value } };
        }
        
        public class Constrained {
            public static T MakeDefault<T>() where T : new() => new T();
            public static T First<T>(IEnumerable<T> source) where T : class => source.First();
        }

        public class Container<T> {
            public U Convert<U>(Func<T, U> f, T val) => f(val);

            public Container() {}
        }

        public class Weird {
            public static string Do<T>(T a) => $"Generic {a}";
            public static string Do(int a) => $"Non-generic {a}";
        }

        public class Types {
            public static Type GetOpenList() => typeof(List<>);
            public static Type CloseList<T>() => typeof(List<T>);
        }

        public class DelegateTest {
            public static T Id<T>(T x) => x;
            public static Func<T, T> GetId<T>() => Id;
        }

        public class Box<T> {
            public T Value;
            public T Diff<U>(T a, U b) => a;

            public Box(T value) => Value = value;
        }
    }
}