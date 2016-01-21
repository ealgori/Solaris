namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfasdf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOItems", "PriceFromPL", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ShTOItems", "IDItemFromPL", c => c.Int(nullable: false));
            AddColumn("dbo.ShTOItems", "DescriptionFromPL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOItems", "DescriptionFromPL");
            DropColumn("dbo.ShTOItems", "IDItemFromPL");
            DropColumn("dbo.ShTOItems", "PriceFromPL");
        }
    }
}
