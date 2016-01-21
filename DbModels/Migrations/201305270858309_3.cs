namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRs", "Subcontractor", c => c.String());
            AddColumn("dbo.ShAVRs", "Project", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRs", "Project");
            DropColumn("dbo.ShAVRs", "Subcontractor");
        }
    }
}
