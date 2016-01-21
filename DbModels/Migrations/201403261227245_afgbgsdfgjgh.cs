namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class afgbgsdfgjgh : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PurchaseRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PurchReqNo = c.String(),
                        PRItem = c.String(),
                        Activity_Id = c.Int(),
                        Network_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PORActivities", t => t.Activity_Id)
                .ForeignKey("dbo.PORNetworks", t => t.Network_Id)
                .Index(t => t.Activity_Id)
                .Index(t => t.Network_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PurchaseRequests", "Network_Id", "dbo.PORNetworks");
            DropForeignKey("dbo.PurchaseRequests", "Activity_Id", "dbo.PORActivities");
            DropIndex("dbo.PurchaseRequests", new[] { "Network_Id" });
            DropIndex("dbo.PurchaseRequests", new[] { "Activity_Id" });
            DropTable("dbo.PurchaseRequests");
        }
    }
}
