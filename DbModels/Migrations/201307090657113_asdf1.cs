namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdf1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShAVRItems", "AVRItemId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShAVRItems", "AVRItemId", c => c.Int(nullable: false, identity: true));
        }
    }
}
