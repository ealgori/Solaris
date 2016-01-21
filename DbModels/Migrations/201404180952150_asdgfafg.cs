namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdgfafg : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShActs",
                c => new
                    {
                        Act = c.String(nullable: false, maxLength: 128),
                        SentDatePeriodStart = c.DateTime(),
                        SentDatePeriodFinish = c.DateTime(),
                        SentNoteInformation = c.String(),
                    })
                .PrimaryKey(t => t.Act);
            
            AddColumn("dbo.ShTOItems", "ActId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOItems", "ActId");
            DropTable("dbo.ShActs");
        }
    }
}
