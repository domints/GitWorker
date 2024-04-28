using System.IO;

namespace GitWorker.Git
{
    public static class FilePaths
    {
        // Essential files
        public const string Index = "index";
        public const string Config = "config";
        public const string Description = "description";
        public const string Head = "HEAD";

        // non-essential files
        public const string CommitEditMsg = "COMMIT_EDITMSG";
        public const string OrigHead = "ORIG_HEAD";
        public const string FetchHead = "FETCH_HEAD";
        public static readonly string LogHead = Path.Combine("logs", "HEAD");

        // Directories
        public const string Objects = "objects";
        public static readonly string RefsHeads = Path.Combine("refs", "heads");

        // default branches
        public static readonly string RefHeadMaster = Path.Combine(RefsHeads, "master");
        public static readonly string RefHeadDev = Path.Combine(RefsHeads, "dev");
        public static readonly string RefHeadDevelop = Path.Combine(RefsHeads, "develop");
        public static readonly string RefHeadMain = Path.Combine(RefsHeads, "main");
        public static readonly string RefHeadRelease = Path.Combine(RefsHeads, "release");
        public static readonly string RefHeadProd = Path.Combine(RefsHeads, "prod");
        public static readonly string RefHeadProduction = Path.Combine(RefsHeads, "production");

    }
}