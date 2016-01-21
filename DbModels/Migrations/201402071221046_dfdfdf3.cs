namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfdf3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShCloneUpdateLogs", "Task_Id", "dbo.DbTasks");
            DropIndex("dbo.ShCloneUpdateLogs", new[] { "Task_Id" });
            AddColumn("dbo.ShCloneUpdateLogs", "Time", c => c.DateTime());
          
            DropColumn("dbo.ShCloneUpdateLogs", "StartTime");
            DropColumn("dbo.ShCloneUpdateLogs", "EndTime");
            DropColumn("dbo.ShCloneUpdateLogs", "Task_Id");
            DropTable("dbo.ShPORTOItems");
            DropTable("dbo.ShPORTOes");
            DropTable("dbo.ShTOes");
            DropTable("dbo.ShTOTemps");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ShTOTemps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TO = c.String(),
                        Site = c.String(),
                        Email = c.String(),
                        Stamp = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShTOes",
                c => new
                    {
                        TO = c.String(nullable: false, maxLength: 128),
                        Subcontractor = c.String(),
                        Region = c.String(),
                    })
                .PrimaryKey(t => t.TO);
            
            CreateTable(
                "dbo.ShPORTOes",
                c => new
                    {
                        PORTO = c.String(nullable: false, maxLength: 128),
                        TO = c.String(),
                    })
                .PrimaryKey(t => t.PORTO);
            
            CreateTable(
                "dbo.ShPORTOItems",
                c => new
                    {
                        PORTOItem = c.Int(nullable: false, identity: true),
                        PORTO = c.String(),
                        Site = c.String(),
                        FOL = c.String(),
                        FIX = c.String(),
                    })
                .PrimaryKey(t => t.PORTOItem);
            
            AddColumn("dbo.ShCloneUpdateLogs", "Task_Id", c => c.Int());
            AddColumn("dbo.ShCloneUpdateLogs", "EndTime", c => c.DateTime());
            AddColumn("dbo.ShCloneUpdateLogs", "StartTime", c => c.DateTime());
            AlterColumn("dbo.ShAVRSs", "CreationPORDate", c => c.DateTime());
            AlterColumn("dbo.ShAVRFs", "CreationPORDate", c => c.DateTime());
            AlterColumn("dbo.PORNetworks", "Network2014", c => c.Int(nullable: false));
            DropColumn("dbo.ShCloneUpdateLogs", "Time");
            CreateIndex("dbo.ShCloneUpdateLogs", "Task_Id");
            AddForeignKey("dbo.ShCloneUpdateLogs", "Task_Id", "dbo.DbTasks", "Id");
        }
    }
}
