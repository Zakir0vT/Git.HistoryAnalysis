using LibGit2Sharp;
using System;
using System.Collections.Generic;

namespace Git.HistoryAnalysis
{
    public class Committer
    {
        public Committer(Signature comitter)
        {
            Signature = comitter;
            CommitsDate = new List<DateTimeOffset>();
        }
        public Signature Signature { get; }
        public List<DateTimeOffset> CommitsDate { get; }
    }
}
