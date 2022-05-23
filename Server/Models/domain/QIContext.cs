using Microsoft.EntityFrameworkCore;

namespace Server.Models.Domain
{
    public class QIContext : DbContext
    {
        public QIContext(DbContextOptions<QIContext> options)
            : base(options)
        {

        }

        public virtual DbSet<ConsumablesInMaster> ConsumablesInMasters { get; set; }
        public virtual DbSet<ConsumablesInMasterModel> ConsumablesInMasterModels { get; set; }
        public virtual DbSet<ConsumablesInDetail> ConsumablesInDetails { get; set; }
        public virtual DbSet<ConsumablesInDetailModel> ConsumablesInDetailModels { get; set; }
        public virtual DbSet<ConsumablesOutMaster> ConsumablesOutMasters { get; set; }
        public virtual DbSet<ConsumablesOutMasterModel> ConsumablesOutMasterModels { get; set; }
        public virtual DbSet<ConsumablesOutDetail> ConsumablesOutDetails { get; set; }
        public virtual DbSet<ConsumablesOutDetailModel> ConsumablesOutDetailModels { get; set; }
    }
}