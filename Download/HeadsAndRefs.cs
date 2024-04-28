using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GitWorker.Git;

namespace GitWorker.Download
{
    public class HeadsAndRefs : BaseDownloadStep
    {
        public HeadsAndRefs(Uri repoUrl, string outDir, Dictionary<string, DlObject> results) : base(repoUrl, outDir, results)
        {
        }

        public override async Task<bool> Download()
        {
            var gotMainHead = await GetHead(FilePaths.Head);
            await GetHead(FilePaths.OrigHead);
            await GetHead(FilePaths.FetchHead);

            foreach (var head in Directory.EnumerateFiles(Path.Combine(_outDir, FilePaths.RefsHeads)))
            {
                var contents = File.ReadAllText(head).Trim();
                if(!string.IsNullOrWhiteSpace(contents) && contents.Length == 40)
                    await TryDownloadObject(contents);
            }

            return gotMainHead;
        }

        private async Task<bool> GetHead(string headFile)
        {
            if(!await TryDownload(headFile)) return false;
            var head = File.ReadAllText(Path.Combine(_outDir, FilePaths.Head)).Replace("ref:", "").Trim();
            if(string.IsNullOrWhiteSpace(head))
                return true;
            return await TryDownload(head);
        }
    }
}