//THIS IS ALL STOLEN FROM LUACUTSCENES https://github.com/Cruor/LuaCutscenes/blob/master/Helpers/LuaHelper.cs

using Celeste.Mod.GooberHelper.Entities;
using FMOD;
using Microsoft.Xna.Framework;
using Monocle;
using NLua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.GooberHelper
{
    public static class LuaHelper
    {
        public static string GetFileContent(string path)
        {
            Stream stream = Everest.Content.Get(path)?.Stream;

            if (stream != null)
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }

            return null;
        }

        private static bool SafeMoveNext(this LuaCoroutine enumerator)
        {
            try
            {
                return enumerator.MoveNext();
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "Lua Cutscenes", $"Failed to resume coroutine");
                Logger.LogDetailed(e);

                return false;
            }
        }

        public static IEnumerator LuaCoroutineToIEnumerator(LuaCoroutine routine)
        {
            while (routine != null && routine.SafeMoveNext())
            {
                if (routine.Current is double || routine.Current is long)
                {
                    yield return Convert.ToSingle(routine.Current);
                }
                else
                {
                    yield return routine.Current;
                }
            }

            yield return null;
        }

        public static LuaTable DictionaryToLuaTable(IDictionary<object, object> dict)
        {
            Lua lua = Everest.LuaLoader.Context;
            LuaTable table = lua.DoString("return {}").FirstOrDefault() as LuaTable;

            foreach (KeyValuePair<object, object> pair in dict)
            {
                table[pair.Key] = pair.Value;
            }

            return table;
        }

        public static LuaTable ListToLuaTable(IList list)
        {
            Lua lua = Everest.LuaLoader.Context;
            LuaTable table = lua.DoString("return {}").FirstOrDefault() as LuaTable;

            int ptr = 1;

            foreach (var value in list)
            {
                table[ptr++] = value;
            }

            return table;
        }

        // Attempt to eval the string if possible
        // Returns eval result if possible, otherwise the input string
        public static object LoadArgumentsString(string arguments)
        {
            Lua lua = Everest.LuaLoader.Context;

            try
            {
                object[] results = lua.DoString("return " + arguments);

                if (results.Length == 1)
                {
                    object result = results.FirstOrDefault();

                    return result ?? arguments;
                }
                else
                {
                    return ListToLuaTable(results);
                }
            }
            catch
            {
                return arguments;
            }
        }

        public static LuaFunction MakeGeneric(LuaTable outerTable, params Type[] types) {
            Console.WriteLine("HI I AM HERE");

            // int top = Everest.LuaLoader.Context.State.GetTop();

            // Console.WriteLine(top);

            // Everest.LuaLoader.Context.State.GetType();

            if(
                Utils.GetEnumeratorIndex(outerTable.Values.GetEnumerator(), 0) is not LuaTable innerTable ||
                Utils.GetEnumeratorIndex(innerTable.Values.GetEnumerator(), 0) is not MethodInfo methodInfo
            ) {
                throw new Exception($"couldnt make the method generic");
            }

            var genericMethod = methodInfo.MakeGenericMethod(types);
            
            return Everest.LuaLoader.Context.RegisterFunction("generic_method", genericMethod);

            // Console.WriteLine(innerTable.Values);
            // Console.WriteLine(innerTable.Values.GetEnumerator());
            // var luaFunctionValue = Utils.GetEnumeratorIndex(innerTable.Values.GetEnumerator(), 0) as KeraLua.LuaFunction;
            // var luaFunctionKey = Utils.GetEnumeratorIndex(innerTable.Keys.GetEnumerator(), 0);
            // var methodInfo = luaFunctionValue.Method;

            // Console.WriteLine(innerTable[luaFunctionKey]);
            // Console.WriteLine(luaFunctionValue);

            // innerTable[luaFunctionKey] = methodInfo.MakeGenericMethod(types).CreateDelegate(typeof(KeraLua.LuaFunction));

            // var parameterTypes = methodInfo.GetParameters().Select(param => param.ParameterType).ToArray();

            // innerTable[innerTable.Keys.GetEnumerator()]

            // Utils.GetEnumeratorIndex(outerTable.GetEnumerator(), 1);

            // return outerTable;

            // return methodInfo.MakeGenericMethod(types).CreateDelegate(
            //     methodInfo.ReturnType == typeof(void) ?
            //         typeof(Action).MakeGenericType(parameterTypes) :
            //         typeof(Func<>).MakeGenericType([methodInfo.ReturnType, ..parameterTypes])
            // );

            // var outerTableEnumerator = outerTable.GetEnumerator();
            //     outerTableEnumerator.MoveNext();
            //     outerTableEnumerator.MoveNext();

            // var innerTable = outerTableEnumerator.Current as LuaTable;

            // var innerTableEnumerator = innerTable.GetEnumerator();
            //     innerTableEnumerator.MoveNext();

            // var methodInfo = innerTableEnumerator.Current as MethodInfo;
        }

        public static Type GetCSharpType(string name) {
            if(Everest.LuaLoader.AllTypes.TryGetValue(name, out var cachedType)) {
                return cachedType.Type;
            }

            throw new Exception($"couldnt find type {name}. please make sure to include the parent namespaces and use PascalCase");
        }

        //this is not stolen from lua cutscenes
        //paste this into CelesteREPL and change the interestingTypes local variable to whatever you want
        public static void GenerateLuaLsTypeAnnotations() {
//using Celeste.Mod.GooberHelper;
//using Celeste.Mod.GooberHelper.Entities;

var str = "-- the code to generate this can be found in Helpers/LuaHelper.cs\n\n";
var interestingTypes = new List<Type>() {
    typeof(Bullet),
    typeof(BulletActivator),
    typeof(BulletTemplate),
    typeof(Vector2),
    typeof(Vector3),
    typeof(Vector4),
    typeof(Color),
    typeof(Bullet.BulletRotationMode),
    typeof(Ease),
    typeof(Calc),
    typeof(Type),
};

var importantInheritedThings = new HashSet<string>() {
    "RemoveSelf",
    "Depth"
};

string adaptToLua(Type type) {
    var nullableUnderlyingType = Nullable.GetUnderlyingType(type);

    if(nullableUnderlyingType != null)
        type = type.GenericTypeArguments[0];

    var processedTypeName = type.Name
        .Split("`")[0]
        .Split("&")[0]
        .Split("[")[0];

    var luaName = processedTypeName switch {
        "Single" => "number",
        "Double" => "number",
        "Byte" => "number",
        "Int32" => "number",
        "UInt32" => "number",
        "Object" => "table",
        "String" => "string",
        "Boolean" => "boolean",
        "Void" => "nil",
        _ => processedTypeName
    };

    var generics = type.GetGenericArguments();

    if(generics.Length > 0) 
        luaName += $"<{generics.Select(generic => adaptToLua(generic))}>";

    if(type.IsArray) 
        luaName += string.Concat(Enumerable.Repeat("[]", type.GetArrayRank()));

    if(nullableUnderlyingType != null)
        luaName += "?";

    return luaName;
}

string getBasicReturnType(Type type) {
    string adapted = adaptToLua(type);

    return adapted switch {
        "number" => "0",
        "boolean" => "false",
        "string" => "\"\"",
        _ => "{}"
    };
}

string getUsingImportPath(Type type) {
    return type.AssemblyQualifiedName?.Split(",")[0].Replace("+", ".") ?? "";
}

var operatorNameToLua = new Dictionary<string, string>() {
    { "op_Addition", "add" },
    { "op_Subtraction", "sub" },
    { "op_Multiply", "mul" },
    { "op_Division", "div" },
    { "op_Modulus", "mod" },
    { "op_UnaryNegation", "unm" },
    { "get_Item", "index" },
};

foreach(var type in interestingTypes) {
    if(type.IsEnum) {
        str += $"---@enum {type.Name}\n";
        str += $"_G.{type.Name} = {{\n";
        
        foreach(var enumValue in type.GetEnumValues()) {
            str += $"\t{Enum.GetName(type, enumValue)} = {Convert.ChangeType(enumValue, Enum.GetUnderlyingType(type))},\n";
        }

        str += $"}}\n\n\n";

        continue;
    }

    var fakeName = type.Name + "_";

    str += $"---@type {type.Name}\n";
    str += $"{type.Name} = require(\"#{getUsingImportPath(type)}\")\n";
    str += "\n";
    str += $"---@class {type.Name}\n";

    foreach(var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
        if(fieldInfo.DeclaringType != type && !importantInheritedThings.Contains(fieldInfo.Name)) continue;

        str += $"---@field {fieldInfo.Name} {adaptToLua(fieldInfo.FieldType)}\n";
    }
    
    foreach(var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
        if(propertyInfo.DeclaringType != type && !importantInheritedThings.Contains(propertyInfo.Name)) continue;

        str += $"---@field {propertyInfo.Name} {adaptToLua(propertyInfo.PropertyType)}\n";
    }

    foreach(var constructor in type.GetConstructors()) {
        str += $"---@overload fun({string.Join(", ", constructor.GetParameters().Select(param => $"{param.Name}: {adaptToLua(param.ParameterType)}"))}): {type.Name}\n";
    }

    foreach(var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
        if(!operatorNameToLua.TryGetValue(methodInfo.Name, out var luaOperatorName)) continue;

        str += $"---@operator {luaOperatorName}({(methodInfo.GetParameters().Length > 1 ? adaptToLua(methodInfo.GetParameters().ElementAtOrDefault(1).ParameterType) : "")}): {adaptToLua(methodInfo.ReturnType)}\n";
    }

    str += $"local {fakeName} = {{}}\n";
    str += "\n";
    
    if(type.IsEnum) continue;

    var functionString = "";
    var lastMethodName = "";

    foreach(var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)) {
        if(methodInfo.DeclaringType != type && !importantInheritedThings.Contains(methodInfo.Name)) continue;
        if(methodInfo.GetBaseDefinition() != methodInfo) continue;

        if(methodInfo.Name == lastMethodName) {
            str += $"---@overload fun({string.Join(", ", methodInfo.GetParameters().Select(param => $"{param.Name}: {adaptToLua(param.ParameterType)}"))}): {adaptToLua(methodInfo.ReturnType)}\n";

            continue;
        } else {
            str += functionString;

            functionString = null;
        }

        lastMethodName = methodInfo.Name;
        
        if(
            methodInfo.Name.StartsWith("get_") ||
            methodInfo.Name.StartsWith("set_") ||
            methodInfo.Name.StartsWith("op_")
        ) continue;

        str += $"---@return {adaptToLua(methodInfo.ReturnType)}\n";

        foreach(var parameterInfo in methodInfo.GetParameters()) {
            str += $"---@param {parameterInfo.Name} {adaptToLua(parameterInfo.ParameterType)}\n";
        }

        functionString = $"function {fakeName}{(methodInfo.IsStatic ? "." : ":")}{methodInfo.Name}({string.Join(", ", methodInfo.GetParameters().Select(param => param.Name))}) return {getBasicReturnType(methodInfo.ReturnType)} end\n\n";
    }

    str += functionString;
    str += "\n";
}

TextInput.SetClipboardText(str);
        }
    }
}