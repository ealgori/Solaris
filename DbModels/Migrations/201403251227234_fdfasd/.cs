namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfasd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOes", "TotalMaterials", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.SATTOes", "TotalServices", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOes", "TotalServices");
            DropColumn("dbo.SATTOes", "TotalMaterials");
        }
    }
}
