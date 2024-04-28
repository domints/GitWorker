using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GitWorker.Download
{
    public static class Download
    {
        public static async Task ProcessDownload(this string[] args)
        {
            Dictionary<string, DlObject> downloadResults = new Dictionary<string, DlObject>();
            var url = args[args.Length - 1].FindRepoUrl();
            var outputDir = Path.Combine(".", url.Host.Replace(".", "_"));
            var ixO = -1;
            if((ixO = Array.FindIndex(args, a => a == "-o")) > -1 || (ixO = Array.FindIndex(args, a => a == "--output")) > -1)
            {
                outputDir = args[ixO + 1];
            }

            outputDir = Path.Combine(outputDir, ".git");

            if(!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            Console.WriteLine("Scraping files...");
            await new FileScrapper(url, outputDir, downloadResults).Download();
            Console.WriteLine("Downloading base git files...");
            await new BasicGitFiles(url, outputDir, downloadResults).Download();
            Console.WriteLine("Downloading files listed in git index...");
            await new GitIndex(url, outputDir, downloadResults).Download();
            Console.WriteLine("Downloading all found refs...");
            await new HeadsAndRefs(url, outputDir, downloadResults).Download();
            Console.WriteLine("Downloading objects referenced by other objects...");
            await new ObjectContents(url, outputDir, downloadResults).Download();
            var fine = downloadResults.Count(r => r.Value.IsDownloaded);
            var notFound = downloadResults.Count(r => r.Value.DlResultCode == 404);
            var broken = downloadResults.Count(r => r.Value.DlResultCode >= 300 && r.Value.DlResultCode != 404);
            var all = downloadResults.Count;
            Console.WriteLine($"{fine}/{notFound}/{broken}/{all}");
        }

        public static Uri FindRepoUrl(this string originalUrl)
        {
            if(originalUrl.Contains(".git"))
                return new Uri(originalUrl.Substring(0, originalUrl.IndexOf(".git") + 4) + "/");

            return new Uri(Path.Combine(originalUrl, ".git") + "/");
        }
    }
}