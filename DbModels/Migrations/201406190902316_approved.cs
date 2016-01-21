namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class approved : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PriceListRevisions", "Approved", c => c.Boolean(nullable: false));
            AddColumn("dbo.PriceListRevisions", "ApprovedBy", c => c.String());
            AddColumn("dbo.PriceListRevisions", "ApprovedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PriceListRevisions", "ApprovedDate");
            DropColumn("dbo.PriceListRevisions", "ApprovedBy");
            DropColumn("dbo.PriceListRevisions", "Approved");
        }
    }
}
