using System.Collections.Generic;

namespace GitWorker.Git.Objects
{
    public class TreeObject : GitObject
    {
        public override ObjectType Type => ObjectType.Tree;
        public List<TreeEntry> Entries { get; set; }
    }
}