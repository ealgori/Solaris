namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRItems", "Unit", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRItems", "Unit");
        }
    }
}
