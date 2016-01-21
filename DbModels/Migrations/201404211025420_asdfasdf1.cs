namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfasdf1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SATActItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        FactDate = c.DateTime(nullable: false),
                        Site = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        SATAct_Id = c.Int(),
                        SATAct_Id1 = c.Int(),
                        SATAct_Id2 = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SATActs", t => t.SATAct_Id)
                .ForeignKey("dbo.SATActs", t => t.SATAct_Id1)
                .ForeignKey("dbo.SATActs", t => t.SATAct_Id2)
                .Index(t => t.SATAct_Id)
                .Index(t => t.SATAct_Id1)
                .Index(t => t.SATAct_Id2);
            
            CreateTable(
                "dbo.SATActs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TO = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SATActItems", "SATAct_Id2", "dbo.SATActs");
            DropForeignKey("dbo.SATActItems", "SATAct_Id1", "dbo.SATActs");
            DropForeignKey("dbo.SATActItems", "SATAct_Id", "dbo.SATActs");
            DropIndex("dbo.SATActItems", new[] { "SATAct_Id2" });
            DropIndex("dbo.SATActItems", new[] { "SATAct_Id1" });
            DropIndex("dbo.SATActItems", new[] { "SATAct_Id" });
            DropTable("dbo.SATActs");
            DropTable("dbo.SATActItems");
        }
    }
}
