namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dffd : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PriceListMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComparablePriceList_Id = c.Int(),
                        PriceList_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceLists", t => t.ComparablePriceList_Id)
                .ForeignKey("dbo.PriceLists", t => t.PriceList_Id)
                .Index(t => t.ComparablePriceList_Id)
                .Index(t => t.PriceList_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PriceListMaps", "PriceList_Id", "dbo.PriceLists");
            DropForeignKey("dbo.PriceListMaps", "ComparablePriceList_Id", "dbo.PriceLists");
            DropIndex("dbo.PriceListMaps", new[] { "PriceList_Id" });
            DropIndex("dbo.PriceListMaps", new[] { "ComparablePriceList_Id" });
            DropTable("dbo.PriceListMaps");
        }
    }
}
