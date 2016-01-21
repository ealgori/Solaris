namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sdfgsdfgsd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATActs", "PONumber", c => c.String());
            AddColumn("dbo.ShAVRSs", "RukRegionApproval", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRSs", "RukRegionApproval");
            DropColumn("dbo.SATActs", "PONumber");
        }
    }
}
