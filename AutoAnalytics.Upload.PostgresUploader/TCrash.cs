using System;
using System.Collections.Generic;

#nullable disable

namespace AutoAnalytics.Upload.PostgresConnector
{
    public partial class TCrash
    {
        public long Id { get; set; }
        public string CrashId { get; set; }
        public long DamageDetailId { get; set; }
        public DateTime? CDate { get; set; }
        public string CDescription { get; set; }

        public virtual TDetail DamageDetail { get; set; }
    }
}
