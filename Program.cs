using System;
using System.Threading.Tasks;
using GitWorker.Download;
using GitWorker.Reset;

namespace GitWorker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if(args.Length > 0)
            {
                if(args[0] == "download")
                {
                    await args.ProcessDownload();
                }
                else if(args[0] == "unpack")
                {

                }
                else if(args[0] == "reset")
                {
                    args.ResetTo();
                }
            }
        }
    }
}
