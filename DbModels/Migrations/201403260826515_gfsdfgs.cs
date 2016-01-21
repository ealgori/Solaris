namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gfsdfgs : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PORItems", "Plandate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PORItems", "Plandate", c => c.DateTime(nullable: false));
        }
    }
}
