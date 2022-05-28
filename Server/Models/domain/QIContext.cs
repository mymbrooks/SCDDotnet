﻿using Microsoft.EntityFrameworkCore;

namespace Server.Models.Domain
{
    public class QIContext : DbContext
    {
        public QIContext(DbContextOptions<QIContext> options)
            : base(options)
        {

        }

        public virtual DbSet<InspectionAbilityCategory> InspectionAbilityCategorys { get; set; }
        public virtual DbSet<InspectionAbilityItem> InspectionAbilityItems { get; set; }
        public virtual DbSet<InspectionItem> InspectionItems { get; set; }
        public virtual DbSet<InspectionAbilityItemInspectionStandard> InspectionAbilityItemInspectionStandards { get; set; }
        public virtual DbSet<InspectionItemInspectionStandard> InspectionItemInspectionStandards { get; set; }
        public virtual DbSet<InspectionItemDeterminationStandard> InspectionItemDeterminationStandards { get; set; }
        public virtual DbSet<InspectionItemInspectionStandardInstrument> InspectionItemInspectionStandardInstruments { get; set; }
        public virtual DbSet<InspectionStandardCredentials> InspectionStandardCredentialss { get; set; }
        public virtual DbSet<Standard> Standards { get; set; }
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