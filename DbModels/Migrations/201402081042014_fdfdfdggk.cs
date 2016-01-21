namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdfdggk : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShAVRFs", "CreationPORdate", c => c.DateTime());
            AlterColumn("dbo.ShAVRSs", "CreationPORdate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShAVRSs", "CreationPORdate", c => c.DateTime());
            AlterColumn("dbo.ShAVRFs", "CreationPORdate", c => c.DateTime());
        }
    }
}
