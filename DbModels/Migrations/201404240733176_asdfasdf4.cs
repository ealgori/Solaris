namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfasdf4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATActs", "PODate", c => c.DateTime());
            AddColumn("dbo.SATActs", "PrintReadyToUpload", c => c.Boolean(nullable: false));
            AddColumn("dbo.SATActs", "PrintUploadDate", c => c.DateTime());
            AddColumn("dbo.SATActs", "PrintUploadUser", c => c.String());
            AddColumn("dbo.ShActs", "TOId", c => c.String());
            AddColumn("dbo.ShActs", "ActLink", c => c.String());
            AddColumn("dbo.ShActs", "GetActLink", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShTOes", "POIssueDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOes", "POIssueDate");
            DropColumn("dbo.ShActs", "GetActLink");
            DropColumn("dbo.ShActs", "ActLink");
            DropColumn("dbo.ShActs", "TOId");
            DropColumn("dbo.SATActs", "PrintUploadUser");
            DropColumn("dbo.SATActs", "PrintUploadDate");
            DropColumn("dbo.SATActs", "PrintReadyToUpload");
            DropColumn("dbo.SATActs", "PODate");
        }
    }
}
