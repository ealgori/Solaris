namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fadfasa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOItems", "Unit", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOItems", "Unit");
        }
    }
}
