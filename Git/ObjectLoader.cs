using System.Collections.Generic;
using System.IO;
using System.Linq;
using GitWorker.Extensions;
using GitWorker.Git.Objects;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace GitWorker.Git
{
    public static class ObjectLoader
    {
        public static Dictionary<string, GitObject> Load(string repositoryRoot)
        {
            var path = Path.Combine(repositoryRoot, "objects");
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                .Where(f => Path.GetFileName(f).Length == 38);

            var result = GetObjects(files).ToDictionary(k => k.Hash, v => (GitObject)v);
            ResolveLinks(result);
            return result;
        }

        private static void ResolveLinks(Dictionary<string, GitObject> objects)
        {
            foreach(var obj in objects.Values)
            {
                if(obj.Type == ObjectType.Commit)
                {
                    var o = obj as CommitObject;
                    if(objects.ContainsKey(o.Tree)) o.TreeObject = objects[o.Tree] as TreeObject;
                    if(o.Parents != null && o.Parents.Count > 0)
                    {
                        if(o.ParentObjects == null) o.ParentObjects = new List<CommitObject>();
                        foreach(var parent in o.Parents)
                        {
                            if(objects.ContainsKey(parent)) o.ParentObjects.Add(objects[parent] as CommitObject);
                        }
                    }
                }
                else if(obj.Type == ObjectType.Tree)
                {
                    var o = obj as TreeObject;
                    foreach(var e in o.Entries)
                    {
                        if(objects.ContainsKey(e.Hash))
                            e.Object = objects[e.Hash];
                    }
                }
            }
        }

        public static IEnumerable<GitObject> GetObjects(IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                bool isOk = false;
                GitObject obj = null;
                try
                {
                    obj = GetObject(path);
                    isOk = true;
                }
                catch
                {
                    File.Delete(path);
                }
                if (isOk)
                    yield return obj;
            }
        }

        public static GitObject GetObject(string path)
        {
            var hash = Directory.GetParent(path).Name + Path.GetFileName(path);
            using (var file = File.OpenRead(path))
            using (var df = new InflaterInputStream(file))
            {
                var header = df.ReadString().Split(" ");
                var size = int.Parse(header[1]);
                switch (header[0])
                {
                    case "commit":
                        return GetCommit(df, hash);
                    case "tree":
                        return GetTree(df, hash);
                    default:
                        return GetBlob(df, size, path, hash);
                }
            }
        }

        public static Stream GetBlobStream(string path, int offset)
        {
            var file = File.OpenRead(path);
            var df = new InflaterInputStream(file);
            return df;
        }

        private static CommitObject GetCommit(Stream stream, string hash)
        {
            var commitContent = stream.ReadString();
            var lines = commitContent.Split("\n");
            var result = new CommitObject
            {
                Hash = hash
            };

            bool hadEmpty = false;

            List<string> messageLines = new List<string>();
            foreach (var l in lines)
            {
                if (!hadEmpty)
                {
                    if (string.IsNullOrWhiteSpace(l))
                    {
                        hadEmpty = true;
                        continue;
                    }
                    var detail = l.Split(" ");
                    if (detail[0] == "tree") result.Tree = detail[1];
                    else if (detail[0] == "parent") 
                    {
                        if(result.Parents == null) result.Parents = new List<string>();
                        result.Parents.Add(detail[1]);
                    }
                    else if (detail[0] == "author")
                    {
                        result.Author = CommitPerson(detail);
                        result.Authored = long.Parse(detail[detail.Length - 2]);
                        result.AuthoredTimeZone = detail[detail.Length - 1];
                    }
                    else if (detail[0] == "committer")
                    {
                        result.Commiter = CommitPerson(detail);
                        result.Commited = long.Parse(detail[detail.Length - 2]);
                        result.CommitedTimeZone = detail[detail.Length - 1];
                    }
                }
                else messageLines.Add(l);
            }

            result.Message = string.Join('\n', messageLines).Trim();

            return result;
        }

        private static string CommitPerson(string[] values)
        {
            return string.Join(" ", values.Skip(1).Take(values.Length - 3));
        }

        private static TreeObject GetTree(Stream stream, string hash)
        {
            var result = new TreeObject
            {
                Hash = hash,
                Entries = new List<TreeEntry>()
            };
            
            while(true)
            {
                var head = stream.ReadString();
                if (string.IsNullOrWhiteSpace(head)) break;
                var sHead = head.Split(" ");
                var e = new TreeEntry
                {
                    AccessCode = sHead[0],
                    Name = sHead[1],
                    Hash = stream.ReadSha()
                };

                result.Entries.Add(e);
            }

            return result;
        }

        private static BlobObject GetBlob(Stream stream, int size, string path, string hash)
        {
            return new BlobObject
            {
                Path = path,
                Hash = hash,
                Size = size,
                DataOffset = (int)stream.Position
            };
        }
    }
}