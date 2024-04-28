using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GitWorker.Download
{
    public class GitHttpClient : HttpClient
    {
        private readonly string baseDownloadPath;

        public GitHttpClient(Uri repoUrl, string baseDownloadPath)
        {
            BaseAddress = repoUrl;
            this.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Macintosh; Intel Mac OS X 11_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/88.0.4324.96 Safari/537.36");
            this.baseDownloadPath = baseDownloadPath;
        }

        public async Task<(bool success, int statusCode)> TryDownloadFileAsync(string filePath)
        {
            var retryCount = 5;
            while (retryCount-- > 0)
            {
                try
                {
                    var result = await this.GetAsync(filePath);
                    if (!result.IsSuccessStatusCode)
                    {
                        return (false, (int)result.StatusCode);
                    }


                    var outPath = Path.Combine(baseDownloadPath, filePath);
                    if (!Directory.Exists(Path.GetDirectoryName(outPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                    }

                    using var resultStream = await result.Content.ReadAsStreamAsync();
                    if (resultStream.Length == 0)
                    {
                        return (false, (int)System.Net.HttpStatusCode.NoContent);
                    }

                    using FileStream fs = new FileStream(outPath, FileMode.Create);
                    {
                        await resultStream.CopyToAsync(fs);
                    }

                    await Task.Delay(200);

                    return (true, (int)result.StatusCode);
                }
                catch { }

                Console.WriteLine($"Download of {filePath} crashed. Waiting 5 sec... {retryCount} retries left.");
                await Task.Delay(5000);
            }
            Console.WriteLine($"Failed to download file {filePath} after 5 retries.");
            return (false, 0);
        }
    }
}