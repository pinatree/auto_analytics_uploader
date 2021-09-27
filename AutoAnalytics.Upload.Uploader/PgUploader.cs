using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AutoAnalytics.Upload.ExcelReader;
using AutoAnalytics.Upload.Model.DataTypes.XlsxV1;
using AutoAnalytics.Upload.PostgresConnector;

namespace AutoAnalytics.Upload.Uploader
{
    public class PgUploader
    {
        private string _connStr;

        public PgUploader(string connStr)
        {
            this._connStr = connStr;
        }

        public List<BreakdownInfo> Read(string path)
        {
            return ExcelReader.ExcelReader.Read(path).ToList();
        }

        public List<TGroup> UploadGroups(List<TGroup> groups)
        {
            var pgConnector = new PgConnector(_connStr);

            var result = new List<TGroup>();

            foreach(var group in groups)
            {
                TGroup filledGroup = pgConnector.FillGroupId(group);
                result.Add(filledGroup);
            }

            return result;
        }

        public List<TSubgroup> UploadSubgroups(List<TSubgroup> subgroups)
        {
            var pgConnector = new PgConnector(_connStr);

            var result = new List<TSubgroup>();

            foreach (var subgroup in subgroups)
            {
                TSubgroup filledGroup = pgConnector.FillSubgroupId(subgroup);
                result.Add(filledGroup);
            }

            return result;
        }

        public List<TDetail> UploadDetails(List<TDetail> details)
        {
            var pgConnector = new PgConnector(_connStr);

            var result = new List<TDetail>();

            foreach (var detail in details)
            {
                TDetail filledGroup = pgConnector.FillDetailId(detail);
                result.Add(filledGroup);
            }

            return result;
        }

        public List<TGroup> GetGroups(List<BreakdownInfo> brdnInfo)
        {
            return brdnInfo.GroupBy(x => x.GroupName)
                .Select(g => g.First())
                .Select(x => new TGroup() { CName = x.GroupName })
                .ToList();
        }

        public List<TSubgroup> GetSubgroups(List<TGroup> groupIds, List<BreakdownInfo> brdnInfo)
        {
            List<ShortSubgroup> shortSubgroups = brdnInfo
                .Select(x => new ShortSubgroup()
                {
                    GroupName = x.GroupName,
                    SubgroupName = x.SubgroupName
                })
                .Distinct().ToList();

            List<TSubgroup> result = new List<TSubgroup>();

            foreach (var subgroup in shortSubgroups)
            {
                TSubgroup addSubgroup = new TSubgroup();
                addSubgroup.CName = subgroup.SubgroupName;

                //long foundId = 
                TGroup found = groupIds.FirstOrDefault(x => x.CName == subgroup.GroupName);

                if (found == null)
                    throw new KeyNotFoundException("Group for subgroup not found");

                addSubgroup.GroupId = found.Id;
                result.Add(addSubgroup);
            }

            return result;
        }

        public List<TDetail> GetDetails(List<TSubgroup> subgroupIds, List<BreakdownInfo> brdnInfo)
        {
            List<ShortDetail> shortDetails = brdnInfo
                .Select(x => new ShortDetail()
                {
                    SubgroupName = x.SubgroupName,
                    DetailName = x.DetailName
                })
                .Distinct().ToList();

            List<TDetail> result = new List<TDetail>();

            foreach (var detail in shortDetails)
            {
                TDetail addDetail = new TDetail();
                addDetail.CName = detail.DetailName;

                //long foundId = 
                TSubgroup found = subgroupIds.FirstOrDefault(x => x.CName == detail.SubgroupName);

                if (found == null)
                    throw new KeyNotFoundException("Subgroup for detail not found");

                addDetail.SubgroupId = found.Id;

                result.Add(addDetail);
            }

            return result;
        }

        public List<TCrash> GetCrashes(List<TDetail> details, List<BreakdownInfo> brdnInfo)
        {
            List<ShortCrash> shortCrashes = brdnInfo
                .Select(x => new ShortCrash()
                {
                    Description = x.Description,
                    CrashId = x.ComplaintId + " - " + x.EngineNum,
                    DetailName = x.DetailName
                }).ToList();
            
            var pgConnector = new PgConnector(_connStr);

            var result = new List<TCrash>();

            foreach (var crash in shortCrashes)
            {
                //Fill detail id

                TDetail found = details.FirstOrDefault(x => x.CName == crash.DetailName);

                if (found == null)
                    throw new KeyNotFoundException("Detail for crash not found");

                TCrash srcCrash = new TCrash()
                {
                    CDescription = crash.Description,
                    CrashId = crash.CrashId,
                    DamageDetailId = found.Id
                };

                TCrash filledCrash = pgConnector.FillCrashId(srcCrash);
                result.Add(filledCrash);
            }

            return result;
        }

        struct ShortSubgroup
        {
            public string GroupName;
            public string SubgroupName;
        }

        struct ShortDetail
        {
            public string SubgroupName;
            public string DetailName;
        }

        struct ShortCrash
        {
            public string DetailName;
            public string Description;
            public string CrashId;
        }
    }
}
