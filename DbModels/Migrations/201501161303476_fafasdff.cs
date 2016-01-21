namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fafasdff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRSs", "ActivityCode", c => c.String());
            DropColumn("dbo.ShAVRSs", "Activity");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShAVRSs", "Activity", c => c.String());
            DropColumn("dbo.ShAVRSs", "ActivityCode");
        }
    }
}
