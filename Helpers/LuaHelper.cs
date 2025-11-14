//THIS IS ALL STOLEN FROM LUACUTSCENES https://github.com/Cruor/LuaCutscenes/blob/master/Helpers/LuaHelper.cs
//you can tell which code is mine by looking at the lack of newlines before brackets

using Celeste.Mod.GooberHelper.Entities;
using Celeste.Mod.Helpers;
using FMOD;
using Iced.Intel;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using NLua;
using NLua.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Celeste.Mod.GooberHelper
{
    public static class LuaHelper
    {
        private static ILHook? objectTranslatorThrowErrorHook;
        public static bool UserCodeIsRunning;

        public static void Load() {
            MethodInfo? objectTranslatorThrowError = typeof(ObjectTranslator).GetMethod("ThrowError", BindingFlags.NonPublic | BindingFlags.Instance);

            if(objectTranslatorThrowError is null) throw new Exception("ef");

            objectTranslatorThrowErrorHook = new ILHook(objectTranslatorThrowError, modifyObjectTranslatorThrowError);
        }

        public static void Unload() {
            objectTranslatorThrowErrorHook?.Dispose();
        }

        public static void modifyObjectTranslatorThrowError(ILContext il) {
            var cursor = new ILCursor(il);

            if(cursor.TryGotoNextBestFit(MoveType.After,
                instr => instr.MatchCall<ObjectTranslator>("Push"),
                instr => instr.MatchRet()
            )) {
                cursor.Index--;

                cursor.EmitLdarg1();
                cursor.EmitLdarg2();
                cursor.EmitDelegate((KeraLua.Lua luaState, object e) => {                    
                    if(!UserCodeIsRunning) return;

                    if(e is LuaScriptException exception) {
                        Logger.LogDetailed(LogLevel.Error, "GooberHelper", exception.ToString() + "\n" + exception.InnerException);

                        luaState.Error(exception.Message);
                    } else {
                        Logger.LogDetailed(LogLevel.Error, "GooberHelper", $"Error of unknown type {e.GetType()} occurred! {e}");

                        luaState.Error();
                    }
                });
            }
        }

        public static void PrintTable(object obj) {
            if(obj is not LuaTable table) {
                Console.WriteLine("not a table!");

                return;
            }

            var str = "LuaTable\n".Color(Utils.SyntaxColors.Type);
            var keyTablesToExpand = new Dictionary<string, LuaTable>();
            var keyTableIndex = 0;
            var indentStr = "  ";

            void recur(LuaTable table, string indent) {
                var enumerator = table.GetEnumerator();

                while(enumerator.MoveNext()) {
                    var key = enumerator.Key;
                    var value = enumerator.Value;

                    str += indent;

                    if(key is LuaTable keyTable) {
                        string name = $"LuaTable #{++keyTableIndex}".Color(Utils.SyntaxColors.Error);

                        str += $"[{name}]: ";

                        keyTablesToExpand[name] = keyTable;
                    } else {
                        str += Utils.FormatValue(key) + ": ";
                    }

                    if(value is LuaTable valueTable) {
                        str += "\n";

                        recur(valueTable, indent + indentStr);
                    } else if(value is KeraLua.LuaFunction valueKeraLuaFunction){
                        str += $"KeraLua.LuaFunction for {valueKeraLuaFunction.Method}\n".Color(Utils.SyntaxColors.Type);
                    } else if(value is LuaFunction luaFunction) {
                        str += $"LuaFunction {DynamicData.For(luaFunction).Get<KeraLua.LuaFunction>("function")?.Method}\n".Color(Utils.SyntaxColors.Type);
                    } else {
                        str += Utils.FormatValue(value) + "\n";
                    }
                }
            }

            recur(table, indentStr);

            foreach(var pair in keyTablesToExpand) {
                str += $"\n{pair.Key}: \n";
                
                recur(pair.Value, indentStr);
            } 
            
            Console.WriteLine(str);
        }

        public static LuaTable CreateTable() =>
            Everest.LuaLoader.Context.DoString("return {}").FirstOrDefault() as LuaTable
                ?? throw new Exception("??? what the fuck why doesnt the table exist");

        public static string? GetFileContent(string path)
        {
            Stream? stream = Everest.Content.Get(path)?.Stream;

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
            UserCodeIsRunning = true;

            try
            {
                return enumerator.MoveNext();
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "Lua Cutscenes", $"Failed to resume coroutine");
                Logger.LogDetailed(e);

                return false;
            } finally {
                UserCodeIsRunning = false;
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
            LuaTable table = CreateTable();

            foreach (KeyValuePair<object, object> pair in dict)
            {
                table[pair.Key] = pair.Value;
            }

            return table;
        }

        public static LuaTable ListToLuaTable(IList list)
        {
            LuaTable table = CreateTable();

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
                    object? result = results.FirstOrDefault();

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

        public static Everest.LuaLoader.CachedType GenericizeCachedType(Everest.LuaLoader.CachedType type, params Type[] types) {
            var genericizedTypeMaybe = type.Type.MakeGenericType(types); //this Should always return the same value even if run twice with the same arguments

            if(genericizedTypeMaybe is not Type genericizedType)
                throw new Exception("??? how is the genericized type null (?)?");

            Everest.LuaLoader.CachedType genericizedCachedType = type.Parent == null ?
                new Everest.LuaLoader.CachedType(type.Namespace, genericizedType) :
                new Everest.LuaLoader.CachedType(type.Parent, genericizedType);

            return genericizedCachedType;
        }

        public static Type GetProxiedType(ProxyType proxyType)
            => proxyType.UnderlyingSystemType;

        //i wish i could just pass both of them in as params parameters to leverage unpacking but it is what it is
        public static LuaTable GenericizeMethodOverloads(LuaTable typeArgumentsTable, params MethodInfo[] overloads) {            
            var typeArgumentsValues = typeArgumentsTable.Values;
            var typeArguments = new Type[typeArgumentsValues.Count]; 
            var i = 0;

            foreach(var type in typeArgumentsValues) {
                typeArguments[i++] = (Type)type;
            }

            var genericizedOverloads = new List<MethodInfo>();

            foreach(var overload in overloads) {
                if(!overload.IsGenericMethodDefinition)
                    continue;

                var overloadTypeArguments = overload.GetGenericArguments();

                if(overloadTypeArguments.Length != typeArguments.Length)
                    continue;

                var genericizedOverload = overload.MakeGenericMethod(typeArguments);

                genericizedOverloads.Add(genericizedOverload);
                Console.WriteLine("added overload " + genericizedOverload);
            }

            if(genericizedOverloads.Count == 0)
                throw new Exception("no generic overloads found!");

            //create the method wrapper
            var lua = Everest.LuaLoader.Context;

            var translator = typeof(NLua.Lua)
                .GetField("_translator", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(lua)
                ?? throw new Exception("translator is null");

            var luaMethodWrapperType = typeof(NLua.Lua).Assembly
                .GetTypes()
                .Where(type => type.Name == "LuaMethodWrapper")
                .First()
                ?? throw new Exception("wrapper type is null");

            var luaMethodWrapperInstance = luaMethodWrapperType
                .GetConstructors()
                .Where(constructor => constructor.GetParameters()[2].ParameterType == typeof(NLua.ProxyType))
                .First()
                .Invoke([translator, null, new ProxyType(genericizedOverloads.First().DeclaringType), genericizedOverloads.First()]) //the last parameter will be overwritten soon; it doesnt matter
                ?? throw new Exception("wrapper instance is null");

            //make it try to match the correct arguments like normal methods invoked from lua
            luaMethodWrapperType
                .GetField("_method", BindingFlags.NonPublic | BindingFlags.Instance)?
                .SetValue(luaMethodWrapperInstance, null);
            
            luaMethodWrapperType
                .GetField("_members", BindingFlags.NonPublic | BindingFlags.Instance)?
                .SetValue(luaMethodWrapperInstance, genericizedOverloads.ToArray());

            //grab the delegate lua actually uses to call methods
            var invokeFunction = luaMethodWrapperType
                .GetField("InvokeFunction", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(luaMethodWrapperInstance)
                as KeraLua.LuaFunction
                ?? throw new Exception("wtf man");

            var node = ListToLuaTable(genericizedOverloads);
            var proxy = invokeFunction;

            // Console.WriteLine(node);
            // PrintTable(node);
            // Console.WriteLine(proxy);

            return ListToLuaTable(new List<object>() { node, proxy });
        }
        
        public static KeraLua.LuaFunction CreateMethodWrapper(MethodInfo methodInfo) {
            var lua = Everest.LuaLoader.Context;

            var translator = typeof(NLua.Lua)
                .GetField("_translator", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(lua)
                ?? throw new Exception("translator is null");

            var luaMethodWrapperType = typeof(NLua.Lua).Assembly
                .GetTypes()
                .Where(type => type.Name == "LuaMethodWrapper")
                .First()
                ?? throw new Exception("wrapper type is null");

            var luaMethodWrapperInstance = luaMethodWrapperType
                .GetConstructors()
                .Where(constructor => constructor.GetParameters()[2].ParameterType == typeof(NLua.ProxyType))
                .First()
                .Invoke([translator, null, new ProxyType(methodInfo.DeclaringType), methodInfo])
                ?? throw new Exception("wrapper instance is null");

            var invokeFunction = luaMethodWrapperType
                .GetField("InvokeFunction", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(luaMethodWrapperInstance)
                as KeraLua.LuaFunction
                ?? throw new Exception("wtf man");

            return invokeFunction;
        }

        //everything after this is not stolen from luacutscenes
        public static LuaTable MakeGeneric(LuaTable outerTable, params Type[] types) {
            var enumerator = outerTable.GetEnumerator();
            object? nodeKey = null;
            object? nodeValue = null;

            object? proxyKey = null;
            object? proxyValue = null;

            LuaTable newTable = CreateTable();

            while(enumerator.MoveNext()) {
                if(enumerator.Key is not LuaTable table) throw new Exception("iwejofweij ???");

                var id = table["_id"];

                switch(id) {
                    case "node":
                        nodeKey = enumerator.Key;
                        nodeValue = enumerator.Value;
                    break;

                    case "proxy":
                        proxyKey = enumerator.Key;
                        proxyValue = enumerator.Value;
                    break;

                    default: throw new Exception("wjat the fuck");
                };
            }

            Console.WriteLine(nodeKey);
            Console.WriteLine(nodeValue);
            Console.WriteLine(proxyKey);

            if(nodeValue is LuaTable nodeTable) {
                var newNodeTable = CreateTable();
                var nodeEnumerator = nodeTable.GetEnumerator();

                while(nodeEnumerator.MoveNext()) {
                    if(nodeEnumerator.Value is not MethodInfo methodInfo) throw new Exception("what the hell why is it not a method??? ?      ?");

                    var genericMethod = methodInfo.MakeGenericMethod(types);
                    
                    newNodeTable[nodeEnumerator.Key] = genericMethod;
                }

                newTable[nodeKey] = newNodeTable;
                newTable[proxyKey] = proxyValue;

                Console.WriteLine("c");
            }

            return newTable;


            // if(Utils.GetEnumeratorIndex(outerTable.Values.GetEnumerator(), 0) is not LuaTable innerTable) {
            //     throw new Exception($"couldnt make the method generic (weird input)");
            // }

            // if(Utils.GetEnumeratorIndex(innerTable.Values.GetEnumerator(), 0) is MethodInfo methodInfo) {

            // }

            // var genericMethod = methodInfo.MakeGenericMethod(types);
            
            // return Everest.LuaLoader.Context.RegisterFunction("generic_method", genericMethod);
        }

        public static Type GetCSharpType(string name) {
            if(Everest.LuaLoader.AllTypes.TryGetValue(name, out var cachedType)) {
                return cachedType.Type;
            }

            throw new Exception($"couldnt find type {name}. please make sure to include the parent namespaces and use PascalCase");
        }

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

        str += $"---@operator {luaOperatorName}({(methodInfo.GetParameters().Length > 1 ? adaptToLua(methodInfo.GetParameters().ElementAtOrDefault(1)!.ParameterType) : "")}): {adaptToLua(methodInfo.ReturnType)}\n";
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