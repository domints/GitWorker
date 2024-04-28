using System;
using System.IO;
using System.Linq;
using GitWorker.Git;
using GitWorker.Git.Objects;

namespace GitWorker.Reset
{
    public static class Reset
    {
        public static void ResetTo(this string[] args)
        {
            if(args.Length < 3)
            {
                Console.WriteLine("Too little arguments! Usage: dotnet run -- reset <repoPath> <commitHash>");
                return;
            }

            var commit = args[args.Length - 1];
            var repo = FindRepoPath(args[args.Length - 2]);
            var commitPath = Path.Combine(repo, GetObjectPath(commit));
            if(!File.Exists(commitPath))
            {
                Console.WriteLine("Either this is not a valid git repo, or you don't have that commit on disk.");
                return;
            }

            var obj = ObjectLoader.GetObject(commitPath);
            if(obj.Type != ObjectType.Commit)
            {
                Console.WriteLine("Given hash does not point to a commit object!");
                return;
            }

            var outPath = Path.Combine(Path.GetDirectoryName(repo), commit);
            Directory.CreateDirectory(outPath);
            var comm = (CommitObject)obj;
            var rootTree = (TreeObject)ObjectLoader.GetObject(Path.Combine(repo, GetObjectPath(comm.Tree)));
            Console.WriteLine(string.Join("\r\n", rootTree.Entries.Select(e => $"{e.Hash} {e.Name}")));
            ResetTree(rootTree, outPath, repo);
        }

        private static void ResetTree(TreeObject tree, string currentPath, string repoPath)
        {
            foreach(var e in tree.Entries)
            {
                var entryTargetPath = Path.Combine(repoPath, GetObjectPath(e.Hash));
                if(!File.Exists(entryTargetPath)) continue;
                var entryObj = ObjectLoader.GetObject(entryTargetPath);
                var entryPath = Path.Combine(currentPath, e.Name);
                if(entryObj.Type == ObjectType.Tree)
                {
                    if(!Directory.Exists(entryPath)) Directory.CreateDirectory(entryPath);

                    ResetTree((TreeObject)entryObj, entryPath, repoPath);
                }
                else if(entryObj.Type == ObjectType.Blob)
                {
                    using FileStream fs = new FileStream(entryPath, FileMode.Create);
                    using var blob = ObjectLoader.GetBlobStream(entryTargetPath, ((BlobObject)entryObj).DataOffset);
                        blob.CopyTo(fs);
                }
            }
        }

        private static string GetObjectPath(string hash)
        {
            return Path.Combine(FilePaths.Objects, hash.Substring(0, 2), hash.Substring(2));
        }

        private static string FindRepoPath(this string originalUrl)
        {
            if(originalUrl.Contains(".git"))
                return originalUrl.Substring(0, originalUrl.IndexOf(".git") + 4) + "/";

            return Path.Combine(originalUrl, ".git") + "/";
        }
    }
}