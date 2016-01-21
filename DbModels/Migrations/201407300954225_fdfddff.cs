namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfddff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DbTasks", "StartDate", c => c.DateTime());
            AddColumn("dbo.DbTasks", "Interval", c => c.String());
            AddColumn("dbo.ShCloneUpdateLogs", "StartTime", c => c.DateTime());
            AddColumn("dbo.ShCloneUpdateLogs", "EndTime", c => c.DateTime());
            AddColumn("dbo.ShCloneUpdateLogs", "Task_Id", c => c.Int());
            CreateIndex("dbo.ShCloneUpdateLogs", "Task_Id");
            AddForeignKey("dbo.ShCloneUpdateLogs", "Task_Id", "dbo.DbTasks", "Id");
            DropColumn("dbo.ShCloneUpdateLogs", "Time");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShCloneUpdateLogs", "Time", c => c.DateTime());
            DropForeignKey("dbo.ShCloneUpdateLogs", "Task_Id", "dbo.DbTasks");
            DropIndex("dbo.ShCloneUpdateLogs", new[] { "Task_Id" });
            DropColumn("dbo.ShCloneUpdateLogs", "Task_Id");
            DropColumn("dbo.ShCloneUpdateLogs", "EndTime");
            DropColumn("dbo.ShCloneUpdateLogs", "StartTime");
            DropColumn("dbo.DbTasks", "Interval");
            DropColumn("dbo.DbTasks", "StartDate");
        }
    }
}
