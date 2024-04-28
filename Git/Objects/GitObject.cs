using System;

namespace GitWorker.Git.Objects
{
    public abstract class GitObject : IComparable<GitObject>, IEquatable<GitObject>
    {
        public string Hash { get; set; }
        public bool Used { get; set; }
        public abstract ObjectType Type { get; }

        public int CompareTo(GitObject other)
        {
            if(other == null) return -1;
            return this.Hash.CompareTo(other.Hash);
        }

        public bool Equals(GitObject other)
        {
            return other != null && Hash.Equals(other.Hash);
        }

        public override int GetHashCode()
        {
            return Hash.GetHashCode();
        }
    }
}