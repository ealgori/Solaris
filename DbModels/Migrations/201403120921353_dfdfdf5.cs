namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfdf5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SATTOItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TOItemId = c.Int(nullable: false),
                        PricePerItem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Site = c.String(),
                        SiteAddress = c.String(),
                        Description = c.String(),
                        PriceListRevision_Id = c.Int(),
                        PriceListRevisionItem_Id = c.Int(),
                        SATTO_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PriceListRevisions", t => t.PriceListRevision_Id)
                .ForeignKey("dbo.PriceListRevisionItems", t => t.PriceListRevisionItem_Id)
                .ForeignKey("dbo.SATTOes", t => t.SATTO_Id)
                .Index(t => t.PriceListRevision_Id)
                .Index(t => t.PriceListRevisionItem_Id)
                .Index(t => t.SATTO_Id);
            
            CreateTable(
                "dbo.SATTOes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TO = c.String(),
                        Activity = c.String(),
                        SubContractor = c.String(),
                        ToType = c.String(),
                        Total = c.Decimal(precision: 18, scale: 2),
                        Region = c.String(),
                        Branch = c.String(),
                        UploadedToSh = c.Boolean(nullable: false),
                        ShUploadDate = c.DateTime(),
                        ShComment = c.String(),
                        CreateUserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SATTOItems", "SATTO_Id", "dbo.SATTOes");
            DropForeignKey("dbo.SATTOItems", "PriceListRevisionItem_Id", "dbo.PriceListRevisionItems");
            DropForeignKey("dbo.SATTOItems", "PriceListRevision_Id", "dbo.PriceListRevisions");
            DropIndex("dbo.SATTOItems", new[] { "SATTO_Id" });
            DropIndex("dbo.SATTOItems", new[] { "PriceListRevisionItem_Id" });
            DropIndex("dbo.SATTOItems", new[] { "PriceListRevision_Id" });
            DropTable("dbo.SATTOes");
            DropTable("dbo.SATTOItems");
        }
    }
}
