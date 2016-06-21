using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using DbModels.DomainModels.ShClone;
using DbModels.DomainModels.DbTasks;
using DbModels.DomainModels.HeadersMap;
using DbModels.DomainModels.Base;
using System.ComponentModel.DataAnnotations.Schema;
using DbModels.Models;
using DbModels.Models.ImportFilesModels;
using DbModels.Models.Pors;
using DbModels.DomainModels.Solaris.Pors;
using DbModels.DomainModels.WIH;
using DbModels.DomainModels.Solaris;
using DbModels.DomainModels.SAT;
using DbModels.DomainModels;
using EFCache;
//using Intranet.Models.DomainClasses.WO;
//using Intranet.Models.DomainClasses.ShClone;
//using Intranet.Models.DomainClasses.Tasks;
//using Intranet.Models.DomainClasses.ImportToSH;
//using Intranet.Models.DomainClasses.OpenPosted;

namespace DbModels.DataContext
{
    [DbConfigurationType(typeof(Configuration))]
    public class Context:DbContext
    {
        
        public Context():base("LocalDb")
        {
            Database.SetInitializer<Context>(
    new MigrateDatabaseToLatestVersion<Context, Migrations.Configuration>());
            Database.Initialize(false); 
        }
       

        #region Service Data

        //public DbSet<MacroRegion> MacroRegions { get; set; }
        //public DbSet<Region> Regions { get; set; }
        //public DbSet<WBSLvl1> WBSLvl1s { get; set; }
        //public DbSet<WBSLvl2> WBSLvl2s { get; set; }
        //public DbSet<PrintOutSowDataRow> PrintOutSowData { get; set; }
        //public DbSet<MasterDataRow> MasterData { get; set; }
        //public DbSet<SOWMappingRow> SOWMapping { get; set; }
        //public DbSet<RegionPrice> RegionPrices { get; set; }
        //public DbSet<ItemsCost> ItemsCosts { get; set; }
        //public DbSet<SOWCode> SOWCodes { get; set; }
        
        //public DbSet<Holiday> Holidays { get; set; }

       // public DbSet<Activity> Activities { get; set; }
        #endregion

        #region SHCLONE
        public DbSet<ShAVRItem> ShAVRItems { get; set; }
       // public DbSet<ShAVRf> ShAVRf { get; set; }
        public DbSet<ShAVRs> ShAVRs { get; set; }
        public DbSet<ShFIX> ShFIXs { get; set; }
        public DbSet<ShFOL> ShFOLs { get; set; }
        public DbSet<ShSITE> ShSITEs { get; set; }
        public DbSet<ShInvoice> ShInvoices { get; set; }
      //  public DbSet<ShSite> ShSites { get; set; }
      ////  public DbSet<Sh> ShUpdateLogs { get; set; }
      //  public DbSet<ShWBS> ShWBSs { get; set; }
      //  public DbSet<ShWO> ShWOs { get; set; }
      //  public DbSet<ShWOItem> ShWOItems { get; set; }
        public DbSet<ShFiles> ShFiles { get; set; }
        public DbSet<ShPriceListItem> ShPriceListItems { get; set; }
      //  public DbSet<ShElement> ShElements { get; set; }
      //  public DbSet<ShEarlyStart> ShEarlyStarts { get; set; }
      //  public DbSet<ShSOW> ShSOWs { get; set; }
      //  public DbSet<ShSOWItem> ShSOWItems { get; set; }
      //  public DbSet<ShPOR> ShPORs { get; set; }
      //  public DbSet<ShPORItem> ShPORItems { get; set; }
        public DbSet<ShContact> ShContacts { get; set; }

        public DbSet<ShTO> ShTOes { get; set; }
        public DbSet<ShTOItem> ShTOItems { get; set; }
        public DbSet<ShMatToItem> ShMatTOItems { get; set; }

        public DbSet<ShCloneUpdateLog> ShCloneUpdateLogs { get; set; }
        //public DbSet<ShVidTO> ShVidTOes { get; set; }
        //public DbSet<ShVidTOItem> ShVidTOItems { get; set; }
        //public DbSet<ShNode> ShNodes { get; set; }
        //public DbSet<ShMUS> ShMUSs { get; set; }
        //public DbSet<ShAvrFull> ShAvrFulls { get; set; }
        public DbSet<ShWIHRequest> ShWIHRequests { get; set; }

        public DbSet<ShSostavRabotTOItem> ShSostavRabotTOItems { get; set; }
        public DbSet<ShSostavRabotTO> ShSostavRabotTOs { get; set; }
        public DbSet<ShAct> ShActs { get; set; }

        public DbSet<ShAddAgreement> ShAddAgreements { get; set; }

        public DbSet<ShVCRequest> ShVCRequests { get; set; }
        public DbSet<ShLimit> ShLimits { get; set; }

        public DbSet<ShFilialStruct> ShFilialStruct { get; set; }

        #endregion

        //#region WO
        //public DbSet<RANContract> RANContracts { get; set; }
        //public DbSet<RANEarlyStart> RANEarlyStarts { get; set; }
        //public DbSet<RANRegion> RANRegions { get; set; }
        //public DbSet<RANWOFor> RANWOFors { get; set; }
        //public DbSet<RANWOMask> RANWOMasks { get; set; }
        //public DbSet<RANWOType> RANWOType { get; set; }
        //public DbSet<RANContractMasterData> RANContractMasterDatas { get; set; }
        //#endregion
        #region Tasks
        public DbSet<DbTask> DbTasks { get; set; }
        public DbSet<TaskLog> TaskLogs { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<DbTaskParameters> DbTaskParameters { get; set; }

        #endregion

        #region OpenPosted
        //public DbSet<InputFile> InputFiles { get; set; }
        //public DbSet<DbHeader> DbHeaders { get; set; }
        #endregion

        #region Imports
        public DbSet<DbModels.DomainModels.DbTasks.SHCImport> SHCImports { get; set; }
        #endregion

        #region Tables

        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<PriceListRevision> PriceListRevisions { get; set; }
        public DbSet<PriceListRevisionItem> PriceListRevisionItems { get; set; }

        public DbSet<PriceListMap> PriceListMaps { get; set; }
        public DbSet<Project> Projects { get; set; }
       
        public DbSet<SubContractor> SubContractors { get; set; }
        #endregion

        #region ImportsSOL
        public DbSet<DbModels.Models.ImportFilesModels.Import> Imports { get; set; }
        public DbSet<ImportFile> ImportFiles { get; set; }
        public DbSet<ImportFileLog> ImportFileLogs { get; set; }
        #endregion

        #region POR
        public DbSet<POR> PORs { get; set; }
        public DbSet<AVRPOR> AVRPORs { get; set; }
        public DbSet<PORItem> PORItems { get; set; }
        public DbSet<PORNetwork> PORNetworks { get; set; }
        public DbSet<PORStatus> PorStatuses { get; set; }
        //public DbSet<DbModels.DomainModels.ShClone.Status> Statuses { get; set; }
        public DbSet<SAPCode> SAPCodes { get; set; }
        public DbSet<PORActivity> PORActivities { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        #endregion

        #region WIH
        /// <summary>
        /// Насколько помню, используется только для отслеживания запросов сап кодов.
        /// </summary>
        public DbSet<WIHRequest> WIHRequests { get; set; }
        #endregion

        #region
        public DbSet<SATTO> SATTOs { get; set; }
        public DbSet<SATTOItem> SATTOItems { get; set; }

        public DbSet<SATAct> SATActs { get; set; }
        public DbSet<SATActItem> SATActItems { get; set; }

        public DbSet<SATSubregion> SATSubregions { get; set; }
        public DbSet<SATActFile> SATActFiles { get; set; }
        public DbSet<SatMusItem> SatMusItems { get; set; }
        public DbSet<VCRequestToCreate> VCRequestsToCreate { get; set; }

        #endregion
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<ShAVRf>().ToTable("ShAVRFs");
            modelBuilder.Entity<ShAVRs>().ToTable("ShAVRSs");
            modelBuilder.Entity<ShAVRItem>().Property(m => m.AVRItemId)
             .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<ShAVRItem>().Property(x => x.Price).HasPrecision(18, 4);
            modelBuilder.Entity<ShAVRItem>().Property(x => x.Quantity).HasPrecision(18, 4);
            modelBuilder.Entity<PORItem>().Property(x => x.NetQty).HasPrecision(18, 4);
            modelBuilder.Entity<PORItem>().Property(x => x.Price).HasPrecision(18, 4);
           // Database.SetInitializer(new DBInit());
        }

    }
}
