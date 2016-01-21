namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfasdf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOItems", "Type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOItems", "Type");
        }
    }
}
