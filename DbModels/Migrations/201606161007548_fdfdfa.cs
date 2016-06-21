namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdfa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShActs", "SendToSubcontracor", c => c.DateTime());
            AddColumn("dbo.ShActs", "CreateDate", c => c.DateTime());
            AddColumn("dbo.ShLimits", "Alias", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShLimits", "Alias");
            DropColumn("dbo.ShActs", "CreateDate");
            DropColumn("dbo.ShActs", "SendToSubcontracor");
        }
    }
}
