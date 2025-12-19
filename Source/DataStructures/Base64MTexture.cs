using System;
using System.IO;
using System.IO.Compression;

//THIS DOESNT WORK BECAUSE I CANT GRAB IMAGES FROM THE CLIPBOARD
//SHIT
namespace Celeste.Mod.GooberHelper.DataStructures {
    public class Base64MTexture {
        public string String { get; set; }
        public MTexture ToMTexture() {
            var stream = new MemoryStream();
            
            using(var compressedStream = new MemoryStream(Convert.FromBase64String(String))) {
                using(var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress)) {
                    gzipStream.CopyTo(stream);
                    gzipStream.Close();
                }
            }
            
            var reader = new BinaryReader(stream);

            var width = reader.ReadInt16();
            var height = reader.ReadInt16();
            var pixels = new Color[width * height];

            for(var i = 0; stream.Position < stream.Length; i++)
                pixels[i] = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

            var virtualTexture = new VirtualTexture("gooberhelper-relative-cursor", width, height, Color.White);

            virtualTexture.Texture.SetData(
                0,
                new Rectangle(0, 0, width, height),
                pixels,
                0,
                pixels.Length
            );

            stream.Dispose();

            return new MTexture(virtualTexture);
        }
    }
}