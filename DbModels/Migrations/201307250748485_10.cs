namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRItems", "Region", c => c.String());
            AddColumn("dbo.ShAVRs", "Region", c => c.String());
            DropColumn("dbo.ShAVRItems", "Subregion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShAVRItems", "Subregion", c => c.String());
            DropColumn("dbo.ShAVRs", "Region");
            DropColumn("dbo.ShAVRItems", "Region");
        }
    }
}
