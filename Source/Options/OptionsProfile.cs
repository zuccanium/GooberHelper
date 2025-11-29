using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Celeste.Mod.GooberHelper.Options {
    public class OptionsProfile {
        public string Name;
        public Dictionary<Option, float> UserDefinedOptions;

        //this is required for modsettings to work
        //it will throw "Cannot dynamically create an instance of type '[this one]'. Reason: No parameterless constructor defined"
        public OptionsProfile() {}

        public OptionsProfile(string name, Dictionary<Option, float> userDefinedOptions) {
            Name = name;
            UserDefinedOptions = userDefinedOptions;
        }

        public static void Create(string name) {
            GooberHelperModule.Settings.OptionsProfiles[name] = new OptionsProfile(name, GooberHelperModule.Settings.UserDefinedOptions.ToDictionary());
            GooberHelperModule.Settings.OptionsProfileOrder.Add(name);
        }

        public static OptionsProfile CreateFromImport() {
            var deserializedProfile = Deserialize(TextInput.GetClipboardText());

            deserializedProfile.Name = Utils.PreventNameCollision(deserializedProfile.Name, GooberHelperModule.Settings.OptionsProfiles);

            GooberHelperModule.Settings.OptionsProfiles[deserializedProfile.Name] = deserializedProfile;
            GooberHelperModule.Settings.OptionsProfileOrder.Add(deserializedProfile.Name);

            return deserializedProfile;
        }

        public static void Load(string name)
            => GooberHelperModule.Settings.UserDefinedOptions = GooberHelperModule.Settings.OptionsProfiles[name].UserDefinedOptions.ToDictionary();

        public static void Save(string name)
            => GooberHelperModule.Settings.OptionsProfiles[name].UserDefinedOptions = GooberHelperModule.Settings.UserDefinedOptions.ToDictionary();

        public static void Rename(string from, string to) {
            if(from == to) return;

            GooberHelperModule.Settings.OptionsProfiles[to] = GooberHelperModule.Settings.OptionsProfiles[from];
            GooberHelperModule.Settings.OptionsProfiles.Remove(from);

            GooberHelperModule.Settings.OptionsProfiles[to].Name = to;

            GooberHelperModule.Settings.OptionsProfileOrder[GooberHelperModule.Settings.OptionsProfileOrder.IndexOf(from)] = to;
        }

        public static OptionsProfile Duplicate(string name, out int insertionIndex) {
            var duplicate = new OptionsProfile(
                name: Utils.CreateCopyName(name, GooberHelperModule.Settings.OptionsProfiles, out var lastNameCollision),
                userDefinedOptions: GooberHelperModule.Settings.OptionsProfiles[name].UserDefinedOptions.ToDictionary()
            );

            GooberHelperModule.Settings.OptionsProfiles[duplicate.Name] = duplicate;

            insertionIndex = GooberHelperModule.Settings.OptionsProfileOrder.IndexOf(lastNameCollision) + 1;
            GooberHelperModule.Settings.OptionsProfileOrder.Insert(insertionIndex, duplicate.Name);

            return duplicate;
        }

        public static void Export(string name)
            => TextInput.SetClipboardText(GooberHelperModule.Settings.OptionsProfiles[name].Serialize());

        public static OptionsProfile Import(string name) {
            var deserializedProfile = Deserialize(TextInput.GetClipboardText());

            GooberHelperModule.Settings.OptionsProfiles[name].UserDefinedOptions = deserializedProfile.UserDefinedOptions;

            return deserializedProfile;
        }

        public static void Delete(string name) {
            GooberHelperModule.Settings.OptionsProfiles.Remove(name);
            GooberHelperModule.Settings.OptionsProfileOrder.Remove(name);
        }

        //this oop stuff is getting ridiculous
        public static bool GetExists(string name)
            => GooberHelperModule.Settings.OptionsProfiles.ContainsKey(name);

        public string Serialize() {
            List<byte> data = [];
            var nameBytes = Encoding.UTF8.GetBytes(Name);

            for(var i = 0; i < nameBytes.Length; i++) {
                data.Add(nameBytes[i]);
            }

            data.Add(0); //null termination

            //i shouldve just used a binary writer for this but it works and i dont want to redo it
            //i didnt know those existed when writing this lmao
            foreach(var pair in UserDefinedOptions) {
                var keyBytes = BitConverter.GetBytes((ushort)pair.Key);
                var valueBytes = BitConverter.GetBytes(pair.Value);
                
                data.Add(keyBytes[0]);
                data.Add(keyBytes[1]);
                data.Add(valueBytes[0]);
                data.Add(valueBytes[1]);
                data.Add(valueBytes[2]);
                data.Add(valueBytes[3]);
            }

            using(var compressedStream = new MemoryStream()) {
                using(var gzipStream = new GZipStream(compressedStream, CompressionLevel.Fastest)) {
                    gzipStream.Write(data.ToArray());
                    gzipStream.Close();

                    return Convert.ToBase64String(compressedStream.ToArray());
                }
            }
        }

        public static OptionsProfile Deserialize(string str) {
            OptionsProfile profile = new("", new Dictionary<Option, float>());
            byte[] data;

            using(var compressedStream = new MemoryStream(Convert.FromBase64String(str))) {
                using(var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress)) {
                    using(var resultsStream = new MemoryStream()) {
                        gzipStream.CopyTo(resultsStream);
                        gzipStream.Close();

                        data = resultsStream.ToArray();
                    }
                }
            }

            var stringLength = Array.IndexOf(data, (byte)0);

            if(stringLength == -1) {
                throw new Exception("couldnt find the null termination of the options profile's name");
            }

            profile.Name = Encoding.UTF8.GetString(data, 0, stringLength);

            for(var i = stringLength + 1; i < data.Length; i += 6) {
                var key = (Option)BitConverter.ToUInt16(data, i);
                var value = BitConverter.ToSingle(data, i + 2);

                profile.UserDefinedOptions[key] = value;
            }

            return profile;
        }
    }
}