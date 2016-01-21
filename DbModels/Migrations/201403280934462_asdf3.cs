namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdf3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShSostavRabotTOItems",
                c => new
                    {
                        SostavRabotTOItemId = c.String(nullable: false, maxLength: 128),
                        Quantity = c.Decimal(precision: 18, scale: 2),
                        Description = c.String(),
                        SostavRabotTOid = c.String(),
                        Price = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.SostavRabotTOItemId);
            
            CreateTable(
                "dbo.ShSostavRabotTOes",
                c => new
                    {
                        SostavRabotTOid = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.SostavRabotTOid);
            
            AddColumn("dbo.ShTOes", "SostavRabotTOid", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOes", "SostavRabotTOid");
            DropTable("dbo.ShSostavRabotTOes");
            DropTable("dbo.ShSostavRabotTOItems");
        }
    }
}
