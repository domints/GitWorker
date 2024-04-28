namespace GitWorker.Git
{
    public class GitFile
    {
        public int Ctime { get; set; }
        public int Mtime { get; set; }
        public int Dev { get; set; }
        public int Ino { get; set; }
        public int Mode { get; set; }
        public int Uid { get; set; }
        public int Gid { get; set; }
        public int Size { get; set; }
        public string Hash { get; set; }
        public bool AssumeValid { get; set; }
        public bool MergeStage1 { get; set; }
        public bool MergeStage2 { get; set; }
        public string Name { get; set; }
    }
}