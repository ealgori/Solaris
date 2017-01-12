namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdfa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShWaylists", "Manager", c => c.String());
            AddColumn("dbo.ShWaylists", "Responsible", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShWaylists", "Responsible");
            DropColumn("dbo.ShWaylists", "Manager");
        }
    }
}
