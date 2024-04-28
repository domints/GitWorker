using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitWorker.Git;
using GitWorker.Git.Objects;

namespace GitWorker.Download
{
    public class ObjectContents : BaseDownloadStep
    {
        public ObjectContents(Uri repoUrl, string outDir, Dictionary<string, DlObject> results) : base(repoUrl, outDir, results)
        {
        }

        public override async Task<bool> Download()
        {
            HashSet<string> parsed = new HashSet<string>();
            var objects = ObjectLoader.Load(_outDir);
            foreach (var o in objects.Values)
            {
                if (parsed.Contains(o.Hash)) continue;
                parsed.Add(o.Hash);
                switch (o.Type)
                {
                    case ObjectType.Commit:
                        await DownloadCommit((CommitObject)o, objects, parsed);
                        break;
                    case ObjectType.Tree:
                        await DownloadTree((TreeObject)o, objects, parsed);
                        break;
                }
            }

            return true;
        }

        private async Task DownloadCommit(CommitObject commit, Dictionary<string, GitObject> objects, HashSet<string> parsed)
        {
            if (commit.Parents != null)
            {
                foreach (var parent in commit.Parents)
                {
                    if (!parsed.Contains(parent))
                    {
                        parsed.Add(parent);
                        var parentObj = await LoadObject<CommitObject>(parent, objects);
                        if(parentObj.success)
                            await DownloadCommit(parentObj.obj, objects, parsed);
                    }
                }
            }

            if(!string.IsNullOrWhiteSpace(commit.Tree))
            {
                parsed.Add(commit.Tree);
                var tree = await LoadObject<TreeObject>(commit.Tree, objects);
                if(tree.success)
                    await DownloadTree(tree.obj, objects, parsed);
            }
        }

        private async Task DownloadTree(TreeObject tree, Dictionary<string, GitObject> objects, HashSet<string> parsed)
        {
            if(tree.Entries == null || tree.Entries.Count == 0)
                return;

            foreach(var e in tree.Entries)
            {
                parsed.Add(e.Hash);
                var entryObj = await LoadObject<GitObject>(e.Hash, objects);
                if(!entryObj.success) continue;
                switch (entryObj.obj.Type)
                {
                    case ObjectType.Commit:
                        await DownloadCommit((CommitObject)entryObj.obj, objects, parsed);
                        break;
                    case ObjectType.Tree:
                        await DownloadTree((TreeObject)entryObj.obj, objects, parsed);
                        break;
                }
            }
        }

        private async Task<(bool success, T obj)> LoadObject<T>(string hash, Dictionary<string, GitObject> objects)
            where T : GitObject
        {
            if(objects.ContainsKey(hash))
                return (true, (T)objects[hash]);
            else if(await TryDownloadObject(hash))
            {
                return (true, (T)ObjectLoader.GetObject(Path.Combine(_outDir, GetObjectPath(hash))));
            }

            return (false, null);
        }
    }
}