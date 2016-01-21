namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfddf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DbTasks", "Params", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DbTasks", "Params");
        }
    }
}
