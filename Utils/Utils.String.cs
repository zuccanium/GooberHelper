using System.Collections.Generic;
using System.Text.RegularExpressions;
using Celeste.Mod.GooberHelper.Components;
using Microsoft.Xna.Framework;
using static Celeste.Mod.GooberHelper.OptionsManager;

namespace Celeste.Mod.GooberHelper {
    public static partial class Utils {
        public static readonly string NumberCaptureRegex = @"(?<num>(\d+(\.\d+)?))";
        private static readonly Regex zingleRegex = new(@$"\s\((?<num>{NumberCaptureRegex})\)$");
        private static readonly Regex bingleRegex = new(@$"\(copy( (?<num>{NumberCaptureRegex}))?\)$");
        

        //this can probably be improved with something similar to a binary search 
        public static string PreventNameCollision<T>(string name, Dictionary<string, T> dict) {
            var newName = name;

            while(dict.ContainsKey(newName)) {                
                var match = zingleRegex.Match(newName);
                
                if(match.Success) {
                    match.Groups.TryGetValue("num", out var num);

                    newName = newName[0..num.Index] + $"{float.Parse(num.Value) + 1})";
                } else {
                    newName += " (2)";
                }
            }

            return newName;
        }

        public static string CreateCopyName<T>(string name, Dictionary<string, T> dict, out string lastNameCollision) {
            var newName = name;

            lastNameCollision = newName;

            while(dict.ContainsKey(newName)) {    
                lastNameCollision = newName;

                var match = bingleRegex.Match(newName);
                
                if(match.Success) {
                    if(match.Groups.TryGetValue("num", out var num) && num.Success) {
                        newName = newName[0..num.Index] + $"{float.Parse(num.Value) + 1})";
                    } else {
                        newName = newName[0..(newName.Length - 1)] + " 2)";
                    }
                } else {
                    newName += " (copy)";
                }
            }

            return newName;
        }

        public static Vector2 GetConservedSpeed(this Player self) {
            var c = GooberPlayerExtensions.Instance;
            var conserveBeforeDashSpeed = GetOptionBool(Option.ConserveBeforeDashSpeed) && self.StateMachine.State == 2;

            return new Vector2(
                SignedAbsMax(
                    self.Speed.X,
                    self.wallSpeedRetentionTimer > 0f
                        ? self.wallSpeedRetained
                        : 0f,
                    conserveBeforeDashSpeed
                        ? self.beforeDashSpeed.X
                        : 0f,
                    conserveBeforeDashSpeed && c.DashStickyRetentionExists
                        ? c.DashStickyRetentionSpeed.X
                        : 0f
                ),
                SignedAbsMax(
                    self.Speed.Y,
                    conserveBeforeDashSpeed
                        ? self.beforeDashSpeed.Y
                        : 0f,
                    conserveBeforeDashSpeed && c.DashStickyRetentionExists
                        ? c.DashStickyRetentionSpeed.Y
                        : 0f
                )
            );
        }
    }
}