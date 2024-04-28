using System.IO;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace GitWorker.Git.Objects
{
    public class BlobObject : GitObject
    {
        public override ObjectType Type => ObjectType.Blob;

        public int DataOffset { get; set; }
        public int Size { get; set; }
        public string Path { get; set; }

        public MemoryStream GetContents()
        {
            var resultStream = new MemoryStream();
            using (var file = File.OpenRead(Path))
            using (var df = new InflaterInputStream(file))
            {
                df.Seek(DataOffset, SeekOrigin.Begin);
                df.CopyTo(resultStream);
                resultStream.Flush();
                resultStream.Seek(0, SeekOrigin.Begin);
                return resultStream;
            }
        }
    }
}