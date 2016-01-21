namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdtty : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShTOes",
                c => new
                    {
                        TO = c.String(nullable: false, maxLength: 128),
                        TOapproved = c.String(),
                    })
                .PrimaryKey(t => t.TO);
            
            CreateTable(
                "dbo.ShTOItems",
                c => new
                    {
                        TOItem = c.String(nullable: false, maxLength: 128),
                        PORTOItem = c.String(),
                        TOId = c.String(),
                        Site = c.String(),
                        FOL = c.String(),
                        FIX = c.String(),
                        TOPlanDate = c.DateTime(),
                        TOFactDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.TOItem);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShTOItems");
            DropTable("dbo.ShTOes");
        }
    }
}
