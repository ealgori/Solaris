namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfasdff : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ShTOItems", "WorkConfirmedByEricssonBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShTOItems", "WorkConfirmedByEricssonBy", c => c.Boolean(nullable: false));
        }
    }
}
