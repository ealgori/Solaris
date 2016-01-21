namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fffddh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOes", "Network", c => c.String());
            AddColumn("dbo.ShTOes", "ActivityCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOes", "ActivityCode");
            DropColumn("dbo.ShTOes", "Network");
        }
    }
}
