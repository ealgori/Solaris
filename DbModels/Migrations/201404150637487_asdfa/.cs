namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShContacts", "WithOutVAT", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShTOItems", "FileReportTO1", c => c.String());
            AddColumn("dbo.ShTOItems", "FileReportTO2", c => c.String());
            AddColumn("dbo.ShTOItems", "FileReportTO3", c => c.String());
            AddColumn("dbo.ShTOItems", "FileReportTO4", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOItems", "FileReportTO4");
            DropColumn("dbo.ShTOItems", "FileReportTO3");
            DropColumn("dbo.ShTOItems", "FileReportTO2");
            DropColumn("dbo.ShTOItems", "FileReportTO1");
            DropColumn("dbo.ShContacts", "WithOutVAT");
        }
    }
}
