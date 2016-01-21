namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfadf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRSs", "MSIPApprove", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShAVRSs", "AVRType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRSs", "AVRType");
            DropColumn("dbo.ShAVRSs", "MSIPApprove");
        }
    }
}
