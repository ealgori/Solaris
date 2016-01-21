namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfas1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOItems", "TOPlanDateSubcontractor", c => c.DateTime());
            AddColumn("dbo.ShTOItems", "Quantity", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ShTOItems", "Price", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOItems", "Price");
            DropColumn("dbo.ShTOItems", "Quantity");
            DropColumn("dbo.ShTOItems", "TOPlanDateSubcontractor");
        }
    }
}
