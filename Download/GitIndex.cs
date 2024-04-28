using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GitWorker.Git;

namespace GitWorker.Download
{
    public class GitIndex : BaseDownloadStep
    {
        public GitIndex(Uri repoUrl, string outDir, Dictionary<string, DlObject> results)
            : base(repoUrl, outDir, results)
        {
        }

        public override async Task<bool> Download()
        {
            if(!await TryDownload(FilePaths.Index))
            {
                Console.WriteLine("Can't download git index file. That's not the method to use.");
                return false;
            }
            
            var indexFile = new IndexFile(_outDir);
            try
            {
                indexFile.Load();
            }
            catch 
            {
                Console.WriteLine("Downloaded index file is broken. That's not the method to use.");
                return false;
            }

            Console.WriteLine($"Found {indexFile.Entries.Count} entries in index.");
            Console.WriteLine($"Found {indexFile.Entries.Select(e => e.Hash).Distinct().Count()} distinct entries in index.");

            var done = 0;
            foreach(var entry in indexFile.Entries)
            {
                await TryDownloadObject(entry.Hash);
                Console.CursorLeft = 0;
                Console.Write($"{++done}/{indexFile.Entries.Count}");
            }

            Console.WriteLine();

            return true;
        }
    }
}