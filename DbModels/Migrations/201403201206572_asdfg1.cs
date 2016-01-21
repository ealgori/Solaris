namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfg1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShWIHRequests",
                c => new
                    {
                        WIHrequests = c.String(nullable: false, maxLength: 128),
                        RequestSentToODdate = c.DateTime(nullable: false),
                        WIHnumber = c.String(),
                        CompletedByOD = c.DateTime(),
                        RejectedByOD = c.DateTime(),
                        RejectedComment = c.String(),
                        TOid = c.String(),
                        CorrectionCompleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.WIHrequests);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShWIHRequests");
        }
    }
}
