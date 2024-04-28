using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GitWorker.Git;

namespace GitWorker.Download
{
    public abstract class BaseDownloadStep
    {
        private readonly GitHttpClient _hc;
        protected readonly string _outDir;
        private readonly Dictionary<string, DlObject> _results;

        public BaseDownloadStep(Uri repoUrl, string outDir, Dictionary<string, DlObject> results)
        {
            _hc = new GitHttpClient(repoUrl, outDir);
            _outDir = outDir;
            _results = results;
        }

        public abstract Task<bool> Download();

        protected async Task<bool> TryDownloadObject(string hash)
        {
            var objectPath = GetObjectPath(hash);
            return await TryDownload(objectPath);
        }

        protected string GetObjectPath(string hash)
        {
            return Path.Combine(FilePaths.Objects, hash.Substring(0, 2), hash.Substring(2));
        }
        
        protected async Task<bool> TryDownload(string path)
        {
            if(_results.ContainsKey(path))
            {
                return _results[path].IsDownloaded;
            }
            else
            {
                var result = await _hc.TryDownloadFileAsync(path);
                _results.Add(path, new DlObject
                {
                    Path = path,
                    IsDownloaded = result.success,
                    DlResultCode = result.statusCode
                });
                
                return result.success;
            }
        }
    }
}