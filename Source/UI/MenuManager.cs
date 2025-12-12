using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Mod.GooberHelper.Extensions;

namespace Celeste.Mod.GooberHelper.UI {
    public static class MenuManager {
        public static TextMenu CurrentMenu => Engine.Scene.GetExtensionFields().CurrentMenu;

        public static void SetRootMenu(TextMenu menu)
            => Engine.Scene.GetExtensionFields().RootMenu = menu;
        
        //just trust that the user (literally only me) doesnt call it with an invalid type
        //its probably fine
        public static void OpenMenu(Type menuType, params object[] constructorArguments) {
            var ext = Engine.Scene.GetExtensionFields();

            var instance = menuType
                .GetConstructors()
                .First()
                .Invoke(constructorArguments);

            if(instance is not AbstractGooberMenu menuInstance)
                throw new Exception("qhar");

            Engine.Scene.Add(menuInstance);

            ext.CurrentMenu = menuInstance;
            menuInstance.Added();
            
            var lastMenu = ext.MenuStack.Count > 0
                ? ext.MenuStack.Peek()
                : ext.RootMenu;

            lastMenu.Visible = false;
            lastMenu.Active = false;
            lastMenu.Focused = false;            

            ext.MenuStack.Push(menuInstance);

            Utils.Log("SET CURRENT MENU");
        }

        public static void OpenMenu<T>(params object[] constructorArguments) where T : AbstractGooberMenu
            => OpenMenu(typeof(T), constructorArguments);

        public static void GoBack() {
            var ext = Engine.Scene.GetExtensionFields();

            if(ext.MenuStack.Pop() is not TextMenu topMenu)
                return;
            
            topMenu.Close();

            var newTop = ext.MenuStack.Count == 0
                ? ext.RootMenu
                : ext.MenuStack.Peek();

            newTop.Visible = true;
            newTop.Active = true;
            newTop.Focused = true;

            newTop.Position.Y = newTop.ScrollTargetY;

            ext.CurrentMenu = newTop;
        }

        public static void GotoRoot() {
            var ext = Engine.Scene.GetExtensionFields();

            while(ext.MenuStack.TryPop(out var top))
                top.Close();
            
            ext.RootMenu.Visible = true;
            ext.RootMenu.Active = true;
            ext.RootMenu.Focused = true;

            ext.CurrentMenu = null;
        }
    }
}