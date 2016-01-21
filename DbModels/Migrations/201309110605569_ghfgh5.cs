namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ghfgh5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShAVRs", "PurchaseOrderNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShAVRs", "PurchaseOrderNumber", c => c.Int());
        }
    }
}
