namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfdf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PriceListRevisions", "SignDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PriceListRevisions", "ExpiryDate", c => c.DateTime());
            AddColumn("dbo.PriceListRevisions", "PaymentTerms", c => c.String());
            AddColumn("dbo.PriceListRevisions", "VAT", c => c.String());
            AddColumn("dbo.PriceListRevisions", "CreationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.SAPCodes", "EmailId", c => c.String());
            DropColumn("dbo.PriceLists", "SignDate");
            DropColumn("dbo.PriceLists", "ExpiryDate");
            DropColumn("dbo.PriceLists", "PaymentTerms");
            DropColumn("dbo.PriceLists", "VAT");
            DropColumn("dbo.PriceLists", "CreationDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PriceLists", "CreationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.PriceLists", "VAT", c => c.String());
            AddColumn("dbo.PriceLists", "PaymentTerms", c => c.String());
            AddColumn("dbo.PriceLists", "ExpiryDate", c => c.DateTime());
            AddColumn("dbo.PriceLists", "SignDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.SAPCodes", "EmailId");
            DropColumn("dbo.PriceListRevisions", "CreationDate");
            DropColumn("dbo.PriceListRevisions", "VAT");
            DropColumn("dbo.PriceListRevisions", "PaymentTerms");
            DropColumn("dbo.PriceListRevisions", "ExpiryDate");
            DropColumn("dbo.PriceListRevisions", "SignDate");
        }
    }
}
