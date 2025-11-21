using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Celeste.Mod.GooberHelper.Helpers;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

#nullable enable

namespace Celeste.Mod.GooberHelper.Attributes.Hooks {
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ILHookAttribute : BaseHookAttribute {
        public ILHookAttribute()
            => Targets = [];
    
        //whatever you do DO NOT LOOK AT THE BOTTOM OF THIS FILE
        //please im begging you
        //c# wouldnt let me use tuple parameters for this method because the parameters had to be compile time constant or something
        //once again, PLEASE DO NOT LOOK AT THE BOTTOM OF THIS FILE
        //thank you for not looking :>
        //have a good time browing the rest of this file BUT NOT THE BOTTOM
        public ILHookAttribute(Type declaringType, string methodName)
            => Targets = [(declaringType, methodName)];

        public override void ApplyHooks(MethodInfo method) {
            foreach(var target in ResolvedTargets) {
                Utils.Log($"applying {method} to target {target}...");

                AppliedHooks.Add(new ILHook(target, method.CreateDelegate<ILContext.Manipulator>()));
            }
        }

        //god damn you
        //i warned you
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6, Type declaringType7, string methodName7) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6), (declaringType7, methodName7)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6, Type declaringType7, string methodName7, Type declaringType8, string methodName8) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6), (declaringType7, methodName7), (declaringType8, methodName8)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6, Type declaringType7, string methodName7, Type declaringType8, string methodName8, Type declaringType9, string methodName9) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6), (declaringType7, methodName7), (declaringType8, methodName8), (declaringType9, methodName9)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6, Type declaringType7, string methodName7, Type declaringType8, string methodName8, Type declaringType9, string methodName9, Type declaringType10, string methodName10) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6), (declaringType7, methodName7), (declaringType8, methodName8), (declaringType9, methodName9), (declaringType10, methodName10)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6, Type declaringType7, string methodName7, Type declaringType8, string methodName8, Type declaringType9, string methodName9, Type declaringType10, string methodName10, Type declaringType11, string methodName11) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6), (declaringType7, methodName7), (declaringType8, methodName8), (declaringType9, methodName9), (declaringType10, methodName10), (declaringType11, methodName11)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6, Type declaringType7, string methodName7, Type declaringType8, string methodName8, Type declaringType9, string methodName9, Type declaringType10, string methodName10, Type declaringType11, string methodName11, Type declaringType12, string methodName12) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6), (declaringType7, methodName7), (declaringType8, methodName8), (declaringType9, methodName9), (declaringType10, methodName10), (declaringType11, methodName11), (declaringType12, methodName12)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6, Type declaringType7, string methodName7, Type declaringType8, string methodName8, Type declaringType9, string methodName9, Type declaringType10, string methodName10, Type declaringType11, string methodName11, Type declaringType12, string methodName12, Type declaringType13, string methodName13) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6), (declaringType7, methodName7), (declaringType8, methodName8), (declaringType9, methodName9), (declaringType10, methodName10), (declaringType11, methodName11), (declaringType12, methodName12), (declaringType13, methodName13)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6, Type declaringType7, string methodName7, Type declaringType8, string methodName8, Type declaringType9, string methodName9, Type declaringType10, string methodName10, Type declaringType11, string methodName11, Type declaringType12, string methodName12, Type declaringType13, string methodName13, Type declaringType14, string methodName14) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6), (declaringType7, methodName7), (declaringType8, methodName8), (declaringType9, methodName9), (declaringType10, methodName10), (declaringType11, methodName11), (declaringType12, methodName12), (declaringType13, methodName13), (declaringType14, methodName14)];
        public ILHookAttribute(Type declaringType1, string methodName1, Type declaringType2, string methodName2, Type declaringType3, string methodName3, Type declaringType4, string methodName4, Type declaringType5, string methodName5, Type declaringType6, string methodName6, Type declaringType7, string methodName7, Type declaringType8, string methodName8, Type declaringType9, string methodName9, Type declaringType10, string methodName10, Type declaringType11, string methodName11, Type declaringType12, string methodName12, Type declaringType13, string methodName13, Type declaringType14, string methodName14, Type declaringType15, string methodName15) => Targets = [(declaringType1, methodName1), (declaringType2, methodName2), (declaringType3, methodName3), (declaringType4, methodName4), (declaringType5, methodName5), (declaringType6, methodName6), (declaringType7, methodName7), (declaringType8, methodName8), (declaringType9, methodName9), (declaringType10, methodName10), (declaringType11, methodName11), (declaringType12, methodName12), (declaringType13, methodName13), (declaringType14, methodName14), (declaringType15, methodName15)];
    }
}

//js script to make more stupid ass constructors 
/*
let str = "";

for(let i = 1; i < 16; i++) {
    let params = new Array(i)
        .fill(0)
        .map((a, i) => `Type declaringType${i + 1}, string methodName${i + 1}`)
        .join(", ");

    let body = new Array(i)
        .fill(0)
        .map((a, i) => `(declaringType${i + 1}, methodName${i + 1})`)
        .join(", ");
    
    str += `public ILHookAttribute(${params}) => Targets = [${body}];\n`
}
*/