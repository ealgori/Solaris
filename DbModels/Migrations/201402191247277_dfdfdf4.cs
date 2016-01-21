namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfdf4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShAVRSs", "DataVipuskaPO", c => c.DateTime());
            AlterColumn("dbo.ShAVRSs", "PORotpravlenVOD", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShAVRSs", "PORotpravlenVOD", c => c.String());
            AlterColumn("dbo.ShAVRSs", "DataVipuskaPO", c => c.String());
        }
    }
}
