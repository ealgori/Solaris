namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fadfas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOItems", "PlanDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOItems", "PlanDate");
        }
    }
}
