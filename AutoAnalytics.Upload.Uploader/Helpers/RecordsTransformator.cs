using AutoAnalytics.Upload.Model.DataTypes.XlsxV1;
using AutoAnalytics.Upload.PostgresConnector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoAnalytics.Upload.Uploader
{
    public static class RecordsTransformator
    {
        public static List<TGroup> GetGroupNames(List<BreakdownInfo> breakdowns)
        {
            //BreakdownInfo.GroupName to Group converter
            Func<BreakdownInfo, TGroup> selector =
                (bkdown) => new TGroup()
                {
                    CName = bkdown.GroupName
                };

            //Convert breakdowns to Group
            return breakdowns.Select(selector)
                //Sort groups
                .OrderBy(det => det.CName)
                //and remove dublicates
                .Distinct().ToList();
        }

        //public static List<TSubgroup> GetSubgroupNames(List<BreakdownInfo> breakdowns)
        //{
        //    //BreakdownInfo.SubgroupName to Subgroup converter
        //    Func<BreakdownInfo, TSubgroup> selector =
        //        (bkdown) => new TSubgroup()
        //        {
        //            CName = bkdown.SubgroupName,
        //            GroupName = bkdown.GroupName
        //        };

        //    //Convert breakdowns to Subgroup
        //    return breakdowns.Select(selector)
        //        //Sort subgroups by group - subgroup
        //        .OrderBy(det => det.GroupName).ThenBy(det => det.Name)
        //        //and remove dublicates
        //        .Distinct().ToList();
        //}

        //public static List<Detail> GetDetailNames(List<BreakdownInfo> breakdowns)
        //{
        //    //BreakdownInfo.SubgroupName to Detail converter
        //    Func<BreakdownInfo, Detail> selector =
        //        (bkdown) => new Detail()
        //        {
        //            Name = bkdown.DetailName,
        //            GroupName = bkdown.GroupName,
        //            SubgroupName = bkdown.SubgroupName
        //        };

        //    //Convert breakdowns to Detail
        //    return breakdowns.Select(selector)
        //        //Sort details by group - subgroup - detail
        //        .OrderBy(det => det.GroupName).ThenBy(det => det.SubgroupName).ThenBy(det => det.Name)
        //        //and remove dublicates
        //        .Distinct().ToList();
        //}
    }
}
