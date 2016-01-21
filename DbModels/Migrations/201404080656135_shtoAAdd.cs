namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shtoAAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOes", "PONumber", c => c.String());
            AddColumn("dbo.ShTOes", "RecallPO", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShTOes", "RecallPOComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOes", "RecallPOComment");
            DropColumn("dbo.ShTOes", "RecallPO");
            DropColumn("dbo.ShTOes", "PONumber");
        }
    }
}
