using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public static string JoinList(IEnumerable<string> strings, string conjunction = "and") {
            var count = strings.Count();

            if(count == 1)
                return strings.First();
            
            if(count == 2)
                return $"{strings.First()} {conjunction} {strings.ElementAt(1)}";

            var builder = new StringBuilder();
            var enumerator = strings.GetEnumerator();

            for(var i = 0; i < count - 1; i++) {
                enumerator.MoveNext();

                builder.Append(enumerator.Current);
                builder.Append(", ");
            }

            enumerator.MoveNext();

            builder.Append(conjunction + " ");
            builder.Append(enumerator.Current);

            return builder.ToString();
        }

        public static string CreateDiff(string original, string changed) {
            var netural = AnsiReset + "  ";
            var positive = GetAnsiColorCode(new Color(0, 255, 0)) + "+ ";
            var negative = GetAnsiColorCode(new Color(255, 0, 0)) + "- ";

            var builder = new StringBuilder();

            var originalLines = original.Split('\n');
            var changedLines = changed.Split('\n');

            var originalLineIndex = 0;
            var changedLineIndex = 0;

            while(originalLineIndex < originalLines.Length || changedLineIndex < changedLines.Length) {
                for(var changedOffset = 0; changedOffset < 20; changedOffset++) {
                    for(var originalOffset = 0; originalOffset < 20; originalOffset++) {
                        var originalLine = originalLines.ElementAtOrDefault(originalLineIndex + originalOffset);
                        var changedLine = changedLines.ElementAtOrDefault(changedLineIndex + changedOffset);
                        
                        if(originalLine != changedLine)
                            continue;
                        
                        for(var i = 0; i < originalOffset; i++)
                            builder.Append(negative + originalLines.ElementAtOrDefault(originalLineIndex + i) + "\n");
                            
                        for(var i = 0; i < changedOffset; i++)
                            builder.Append(positive + changedLines.ElementAtOrDefault(changedLineIndex + i) + "\n");
                        
                        builder.Append(netural + originalLines.ElementAtOrDefault(originalLineIndex + originalOffset) + "\n");
                    
                        originalLineIndex += originalOffset + 1;
                        changedLineIndex += changedOffset + 1;
                    
                        goto getOut;    
                    }
                }
                
                //cursed
                getOut:;
            }

            return builder.ToString();
        }
    }
}