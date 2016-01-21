namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class artert : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATActItems", "Unit", c => c.String());
            AddColumn("dbo.SATActItems", "SiteAddress", c => c.String());
            AddColumn("dbo.SATActs", "WOVAT", c => c.Boolean(nullable: false));
            AddColumn("dbo.SATActs", "NomerDogovora", c => c.String());
            AddColumn("dbo.SATActs", "DataDogovora", c => c.DateTime());
            AddColumn("dbo.SATActs", "WorkDescription", c => c.String());
            AddColumn("dbo.SATActs", "Region", c => c.String());
            AddColumn("dbo.SATActs", "Branch", c => c.String());
            AddColumn("dbo.SATActs", "Network", c => c.String());
            AddColumn("dbo.SATActs", "SubContractor", c => c.String());
            AddColumn("dbo.SATActs", "SubContractorSapNumber", c => c.String());
            AddColumn("dbo.SATActs", "SubContractorAddress", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATActs", "SubContractorAddress");
            DropColumn("dbo.SATActs", "SubContractorSapNumber");
            DropColumn("dbo.SATActs", "SubContractor");
            DropColumn("dbo.SATActs", "Network");
            DropColumn("dbo.SATActs", "Branch");
            DropColumn("dbo.SATActs", "Region");
            DropColumn("dbo.SATActs", "WorkDescription");
            DropColumn("dbo.SATActs", "DataDogovora");
            DropColumn("dbo.SATActs", "NomerDogovora");
            DropColumn("dbo.SATActs", "WOVAT");
            DropColumn("dbo.SATActItems", "SiteAddress");
            DropColumn("dbo.SATActItems", "Unit");
        }
    }
}
