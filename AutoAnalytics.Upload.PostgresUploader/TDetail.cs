using System;
using System.Collections.Generic;

#nullable disable

namespace AutoAnalytics.Upload.PostgresConnector
{
    public partial class TDetail
    {
        public TDetail()
        {
            TCrashes = new HashSet<TCrash>();
        }

        public long Id { get; set; }
        public long SubgroupId { get; set; }
        public string CName { get; set; }

        public virtual TSubgroup Subgroup { get; set; }
        public virtual ICollection<TCrash> TCrashes { get; set; }
    }
}
