namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShLimits", "Alias", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShLimits", "Alias");
        }
    }
}
