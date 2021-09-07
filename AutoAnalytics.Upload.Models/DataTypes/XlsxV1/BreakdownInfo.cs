using System;

namespace AutoAnalytics.Upload.Model.DataTypes.XlsxV1
{
    [Serializable]
    public class BreakdownInfo
    {
        public string GroupName { get; set; }
        public string SubgroupName { get; set; }
        public string DetailName { get; set; }

        public string CarModel { get; set; }

//Complaing id = complaint id + engine num
        public string ComplaintId { get; set; }
        public string EngineNum { get; set; }

        public string Country { get; set; }
        public string Dealer { get; set; }

        public string Culprit { get; set; }

        public string Description { get; set; }

        public DateTime? Date { get; set; }
    }
}
