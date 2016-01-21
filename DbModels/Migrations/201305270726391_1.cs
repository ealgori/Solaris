namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShAVRItems",
                c => new
                    {
                        AVRItemId = c.Int(nullable: false, identity: true),
                        AVRId = c.String(),
                        FIXId = c.String(),
                        FOLId = c.String(),
                        SiteId = c.String(),
                        Description = c.String(),
                        Price = c.Decimal(precision: 18, scale: 2),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.AVRItemId);
            
            CreateTable(
                "dbo.ShAVRs",
                c => new
                    {
                        AVRId = c.String(nullable: false, maxLength: 128),
                        Subregion = c.String(),
                        TotalAmount = c.Decimal(precision: 18, scale: 2),
                        WorkStart = c.DateTime(nullable: false),
                        WorkEnd = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AVRId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShAVRs");
            DropTable("dbo.ShAVRItems");
        }
    }
}
