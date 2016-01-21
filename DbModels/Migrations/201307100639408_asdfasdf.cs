namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfasdf : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShAVRItems", "Price", c => c.Decimal(precision: 18, scale: 4));
            AlterColumn("dbo.ShAVRItems", "Quantity", c => c.Decimal(precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShAVRItems", "Quantity", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.ShAVRItems", "Price", c => c.Decimal(precision: 18, scale: 2));
        }
    }
}
