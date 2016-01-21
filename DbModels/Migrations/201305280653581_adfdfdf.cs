namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adfdfdf : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShFIXes",
                c => new
                    {
                        FIX = c.String(nullable: false, maxLength: 128),
                        Address = c.String(),
                        MacroRegion = c.String(),
                    })
                .PrimaryKey(t => t.FIX);
            
            CreateTable(
                "dbo.ShFOLs",
                c => new
                    {
                        FOL = c.String(nullable: false, maxLength: 128),
                        MacroRegion = c.String(),
                        Branch = c.String(),
                    })
                .PrimaryKey(t => t.FOL);
            
            CreateTable(
                "dbo.ShSITEs",
                c => new
                    {
                        Site = c.String(nullable: false, maxLength: 128),
                        Address = c.String(),
                        Branch = c.String(),
                        City = c.String(),
                        MacroRegion = c.String(),
                    })
                .PrimaryKey(t => t.Site);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShSITEs");
            DropTable("dbo.ShFOLs");
            DropTable("dbo.ShFIXes");
        }
    }
}
