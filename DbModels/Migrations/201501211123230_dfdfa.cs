namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRSs", "PaymentDate", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "ClearingDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRSs", "ClearingDate");
            DropColumn("dbo.ShAVRSs", "PaymentDate");
        }
    }
}
