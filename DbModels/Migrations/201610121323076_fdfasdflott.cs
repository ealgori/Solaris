namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfasdflott : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                        Status = c.String(),
                        Message = c.String(),
                        File = c.Binary(),
                        TaskLog_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TaskLogs", t => t.TaskLog_Id)
                .Index(t => t.TaskLog_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "TaskLog_Id", "dbo.TaskLogs");
            DropIndex("dbo.Logs", new[] { "TaskLog_Id" });
            DropTable("dbo.Logs");
        }
    }
}
