using Microsoft.EntityFrameworkCore;
using QualityWebApi.Filter.Domain;
using QualityWebApi.Filter.ef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace QualityWebApi.Filter.DLL
{
    public class AudienceRepository
    {
        private GhDbContext _dbContext;
        public AudienceRepository(GhDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Audience GetByName(string name)
        {
            return _dbContext.Bas_OauthJwtAudience.Where(t => t.Name == name).FirstOrDefault();
        }

        public Permission GetPermissingById(string authId, string userId)
        {
            string sql = string.Format(" select rtrim(chrAuth) as AuthName,t1.chrAuthId as AuthId,chrUserName as UserId from ( " +
                 "  select  '+' + chrAuthId as chrAuthId, chrAuth from dbo.tau_Auth " +
                 " where chrAuthId = '{0}' " +
                " )t1 join v_AuthId t2 on t1.chrAuthId = t2.chrAuthId " +
                " where t2.chrUserName = '{1}'", authId, userId);
            return _dbContext.Set<Permission>().FromSql(sql).FirstOrDefault();//针对对上下文和基础存储中给定类型的实体的访问返回一个非泛型 DbSet 实例。
        }
    }
}
