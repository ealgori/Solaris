namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfaghj : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOes", "WOVAT", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOes", "WOVAT");
        }
    }
}
