using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitWorker.Download
{
    public class FileScrapper : BaseDownloadStep
    {
        public FileScrapper(Uri repoUrl, string outDir, Dictionary<string, DlObject> results) : base(repoUrl, outDir, results)
        {
        }

        public override async Task<bool> Download()
        {
            await Task.Delay(1);
            return true;
        }
    }
}