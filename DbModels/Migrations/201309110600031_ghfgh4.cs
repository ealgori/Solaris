namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ghfgh4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRs", "PurchaseOrderNumber", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRs", "PurchaseOrderNumber");
        }
    }
}
