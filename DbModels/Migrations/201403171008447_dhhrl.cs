namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dhhrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShMatToItems", "SiteId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShMatToItems", "SiteId");
        }
    }
}
