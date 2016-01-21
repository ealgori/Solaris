namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdfdff : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Activities");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Activities",
                c => new
                    {
                        ActivityName = c.String(nullable: false, maxLength: 128),
                        TOType = c.String(),
                    })
                .PrimaryKey(t => t.ActivityName);
            
        }
    }
}
