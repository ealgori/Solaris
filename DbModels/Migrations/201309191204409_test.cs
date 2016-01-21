namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PORPriceListRevisions", "POR_Id", "dbo.PORs");
            DropForeignKey("dbo.PORPriceListRevisions", "PriceListRevision_Id", "dbo.PriceListRevisions");
            DropIndex("dbo.PORPriceListRevisions", new[] { "POR_Id" });
            DropIndex("dbo.PORPriceListRevisions", new[] { "PriceListRevision_Id" });
            CreateTable(
                "dbo.PriceListRevisionPORs",
                c => new
                    {
                        PriceListRevision_Id = c.Int(nullable: false),
                        POR_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PriceListRevision_Id, t.POR_Id })
                .ForeignKey("dbo.PriceListRevisions", t => t.PriceListRevision_Id, cascadeDelete: true)
                .ForeignKey("dbo.PORs", t => t.POR_Id, cascadeDelete: true)
                .Index(t => t.PriceListRevision_Id)
                .Index(t => t.POR_Id);
            
            DropTable("dbo.PORPriceListRevisions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PORPriceListRevisions",
                c => new
                    {
                        POR_Id = c.Int(nullable: false),
                        PriceListRevision_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.POR_Id, t.PriceListRevision_Id });
            
            DropForeignKey("dbo.PriceListRevisionPORs", "POR_Id", "dbo.PORs");
            DropForeignKey("dbo.PriceListRevisionPORs", "PriceListRevision_Id", "dbo.PriceListRevisions");
            DropIndex("dbo.PriceListRevisionPORs", new[] { "POR_Id" });
            DropIndex("dbo.PriceListRevisionPORs", new[] { "PriceListRevision_Id" });
            DropTable("dbo.PriceListRevisionPORs");
            CreateIndex("dbo.PORPriceListRevisions", "PriceListRevision_Id");
            CreateIndex("dbo.PORPriceListRevisions", "POR_Id");
            AddForeignKey("dbo.PORPriceListRevisions", "PriceListRevision_Id", "dbo.PriceListRevisions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PORPriceListRevisions", "POR_Id", "dbo.PORs", "Id", cascadeDelete: true);
        }
    }
}
