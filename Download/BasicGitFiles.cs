using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GitWorker.Git;

namespace GitWorker.Download
{
    public class BasicGitFiles : BaseDownloadStep
    {
        public BasicGitFiles(Uri repoUrl, string outDir, Dictionary<string, DlObject> results) : base(repoUrl, outDir, results)
        {
        }

        public override async Task<bool> Download()
        {
            bool gotEssential = true;
            gotEssential &= await TryDownload(FilePaths.Index);
            gotEssential &= await TryDownload(FilePaths.Config);
            gotEssential &= await TryDownload(FilePaths.Description);
            gotEssential &= await TryDownload(FilePaths.Head);

            await TryDownload(FilePaths.CommitEditMsg);
            await TryDownload(FilePaths.OrigHead);
            await TryDownload(FilePaths.FetchHead);
            await TryDownload(FilePaths.LogHead);
            await TryDownload(FilePaths.RefHeadMaster);
            await TryDownload(FilePaths.RefHeadDev);
            await TryDownload(FilePaths.RefHeadDevelop);
            await TryDownload(FilePaths.RefHeadMain);
            await TryDownload(FilePaths.RefHeadProd);
            await TryDownload(FilePaths.RefHeadProduction);
            await TryDownload(FilePaths.RefHeadRelease);

            if(!gotEssential) 
                Console.WriteLine("Couldn't download essential git files. Recovering repo will be tad harder...");
            return gotEssential;
        }
    }
}