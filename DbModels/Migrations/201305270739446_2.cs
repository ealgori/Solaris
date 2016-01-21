namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShAVRs", "WorkStart", c => c.DateTime());
            AlterColumn("dbo.ShAVRs", "WorkEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShAVRs", "WorkEnd", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ShAVRs", "WorkStart", c => c.DateTime(nullable: false));
        }
    }
}
