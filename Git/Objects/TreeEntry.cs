namespace GitWorker.Git.Objects
{
    public class TreeEntry
    {
        public string AccessCode { get; set; }
        public string Name { get; set; }
        public string Hash { get; set; }
        public GitObject Object { get; set; }

        public override string ToString()
        {
            return $"{Hash.Substring(0, 7)} {Name}";
        }
    }
}