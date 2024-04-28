using System.Collections.Generic;

namespace GitWorker.Git.Objects
{
    public class CommitObject : GitObject
    {
        public override ObjectType Type => ObjectType.Commit;

        public string Tree { get; set; }
        public TreeObject TreeObject { get; set; }
        public List<string> Parents { get; set; }
        public List<CommitObject> ParentObjects { get; set; }
        public string Author { get; set; }
        public string Commiter { get; set; }
        public long Authored { get; set; }
        public string AuthoredTimeZone { get; set; }
        public long Commited { get; set; }
        public string CommitedTimeZone { get; set; }
        public string Message { get; set; }
    }
}