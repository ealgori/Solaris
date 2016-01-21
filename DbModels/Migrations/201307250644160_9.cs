namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRItems", "Subregion", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRItems", "Subregion");
        }
    }
}
