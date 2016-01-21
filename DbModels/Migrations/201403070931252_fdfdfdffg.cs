namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdfdffg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PORActivities", "TOType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PORActivities", "TOType");
        }
    }
}
