using System;

//this doesnt do anything at all
//i just thought i might need it later and i didnt want to have to go through every single option class in the future

namespace Celeste.Mod.GooberHelper.Attributes {
    [AttributeUsage(AttributeTargets.Class)]
    public class GooberHelperOptionAttribute : Attribute {
        public readonly Option Option;
        
        public GooberHelperOptionAttribute(Option option)
            => Option = option;

        public GooberHelperOptionAttribute() {}
    }
}