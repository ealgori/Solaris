namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adfa : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShFuelLists",
                c => new
                    {
                        FuelList = c.String(nullable: false, maxLength: 128),
                        Required = c.String(),
                        Generator = c.String(),
                        Responsible = c.String(),
                        Manager = c.String(),
                    })
                .PrimaryKey(t => t.FuelList);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShFuelLists");
        }
    }
}
