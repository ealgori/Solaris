namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class afgbgsdfgj : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOItems", "SiteIndex", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOItems", "SiteIndex");
        }
    }
}
