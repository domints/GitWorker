using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitWorker.Extensions;

namespace GitWorker.Git
{
    public class IndexFile
    {
        private byte[] MagicString = new byte[] { 0x44, 0x49, 0x52, 0x43 }; //DIRC string
        private readonly string repositoryRoot;

        public string FilePath { get; private set; }
        public int IndexVersion { get; private set; }

        public IReadOnlyCollection<GitFile> Entries { get; private set; }
        
        public IndexFile(string repositoryRoot)
        {
            FilePath = System.IO.Path.Combine(repositoryRoot, "index");
            this.repositoryRoot = repositoryRoot;
        }

        public void Load()
        {
            using(var file = File.OpenRead(FilePath))
            {
                var wordBuffer = new byte[4];
                file.Read(wordBuffer, 0, 4);
                if(!wordBuffer.Select((v, ix) => MagicString[ix] == v).All(v => v))
                    throw new InvalidDataException("Provided file is not git index file.");

                IndexVersion = file.ReadInt();

                var entryCount = file.ReadInt();
                var entries = new List<GitFile>(entryCount);
                for(int i = 0; i < entryCount; i++)
                {
                    var f = new GitFile();
                    f.Ctime = file.ReadInt();
                    file.ReadInt();
                    f.Mtime = file.ReadInt();
                    file.ReadInt();
                    f.Dev = file.ReadInt();
                    f.Ino = file.ReadInt();
                    f.Mode = file.ReadInt();
                    f.Uid = file.ReadInt();
                    f.Gid = file.ReadInt();
                    f.Size = file.ReadInt();
                    f.Hash = file.ReadSha();
                    var flags = file.ReadShort();
                    f.AssumeValid = (flags & 0x8000) > 0;
                    f.MergeStage1 = (flags & 0x2000) > 0;
                    f.MergeStage2 = (flags & 0x1000) > 0;
                    var nameSize = (flags & 0x0FFF) + 1;
                    var headerSize = 62 + nameSize;
                    if(headerSize % 8 != 0)
                    {
                        nameSize += 8 - (headerSize % 8);
                    }
                    
                    f.Name = file.ReadString(nameSize);

                    entries.Add(f);
                }

                Entries = entries;
            }
        }
    }
}