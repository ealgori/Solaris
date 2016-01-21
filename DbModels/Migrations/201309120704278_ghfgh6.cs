namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ghfgh6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShAvrFulls",
                c => new
                    {
                        AVRId = c.String(nullable: false, maxLength: 128),
                        Subregion = c.String(),
                        TotalAmount = c.Decimal(precision: 18, scale: 2),
                        WorkStart = c.DateTime(),
                        WorkEnd = c.DateTime(),
                        Subcontractor = c.String(),
                        Project = c.String(),
                        Region = c.String(),
                        PurchaseOrderNumber = c.String(),
                    })
                .PrimaryKey(t => t.AVRId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShAvrFulls");
        }
    }
}
