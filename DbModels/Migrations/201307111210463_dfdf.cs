namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdf : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WIHRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Filename = c.String(),
                        WIHNumber = c.String(),
                        Type = c.String(),
                        SendDate = c.DateTime(nullable: false),
                        ReceivedWIHNumberDate = c.DateTime(),
                        Approved = c.Boolean(),
                        ReceivedWIHResultDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WIHRequests");
        }
    }
}
