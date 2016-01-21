namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdf4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DbTasks", "ImportFileName5", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DbTasks", "ImportFileName5");
        }
    }
}
