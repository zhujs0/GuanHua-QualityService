using Microsoft.EntityFrameworkCore;
using QualityWebApi.Filter.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityWebApi.Filter.ef
{
    public class GhDbContext:DbContext
    {
        public GhDbContext(DbContextOptions<GhDbContext> options):base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
            //string connectionString = "";//链接字符串
            //builder.UseSqlServer(connectionString);//建立链接

        }
        public DbSet<Audience> Bas_OauthJwtAudience { get; set; }
    }
}
