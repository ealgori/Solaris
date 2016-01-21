namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfasdfd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOItems", "MatTOItemId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOItems", "MatTOItemId");
        }
    }
}
