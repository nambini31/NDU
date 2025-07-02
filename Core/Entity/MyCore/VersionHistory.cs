using System;
using System.Collections.Generic;

namespace Core.Entity.MyCore
{
    public class VersionHistory
    {
        public VersionHistory()
        {
            VersionHistoryQueries = new HashSet<VersionHistoryQueries>();
        }

        public string Id { get; set; }
        public long? MyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? VersionNumber { get; set; }
        public string ExecutedBy { get; set; }
        public DateTime? InsertedOn { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<VersionHistoryQueries> VersionHistoryQueries { get; set; }
    }
}
