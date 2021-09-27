using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AutoAnalytics.Upload.PostgresConnector
{
    public class PgConnector
    {
        private dreamscape_portal_dbContext _dbContext;

        public PgConnector(string connStr)
        {
            _dbContext = new dreamscape_portal_dbContext(connStr);
        }

        public TGroup FillGroupId(TGroup group)
        {
            var found = _dbContext.TGroups.FirstOrDefault(g => g.CName == group.CName);

            if (found != null)
            {
                group.Id = found.Id;
                return group;
            }

            EntityEntry<TGroup> addResult = _dbContext.Add(group);
            _dbContext.SaveChanges();

            return addResult.Entity;
        }

        public TSubgroup FillSubgroupId(TSubgroup subgroup)
        {
            var found = _dbContext.TSubgroups.FirstOrDefault(g => g.CName == subgroup.CName && g.GroupId == subgroup.GroupId);

            if (found != null)
            {
                subgroup.Id = found.Id;
                return subgroup;
            }

            EntityEntry<TSubgroup> addResult = _dbContext.Add(subgroup);
            _dbContext.SaveChanges();

            return addResult.Entity;
        }

        public TDetail FillDetailId(TDetail detail)
        {
            var found = _dbContext.TDetails.FirstOrDefault(g => g.CName == detail.CName && g.SubgroupId == detail.SubgroupId);

            if (found != null)
            {
                detail.Id = found.Id;
                return detail;
            }

            EntityEntry<TDetail> addResult = _dbContext.Add(detail);
            _dbContext.SaveChanges();

            return addResult.Entity;
        }

        public TCrash FillCrashId(TCrash crash)
        {
            var found = _dbContext.TCrashes.FirstOrDefault(g => g.CrashId == crash.CrashId && g.DamageDetailId == crash.DamageDetailId);

            if (found != null)
            {
                crash.Id = found.Id;
                return crash;
            }

            EntityEntry<TCrash> addResult = _dbContext.Add(crash);
            _dbContext.SaveChanges();

            return addResult.Entity;
        }
    }
}
