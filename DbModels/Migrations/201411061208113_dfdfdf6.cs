namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfdf6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOes", "EquipmentTO", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShTOItems", "EquipmentQuantity", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ShTOItems", "EquipmentName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOItems", "EquipmentName");
            DropColumn("dbo.ShTOItems", "EquipmentQuantity");
            DropColumn("dbo.ShTOes", "EquipmentTO");
        }
    }
}
