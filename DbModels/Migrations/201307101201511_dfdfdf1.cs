namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfdf1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PORItems", "NetQty", c => c.Decimal(nullable: false, precision: 18, scale: 4));
            AlterColumn("dbo.PORItems", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PORItems", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PORItems", "NetQty", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
