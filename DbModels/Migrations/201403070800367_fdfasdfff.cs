namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfasdfff : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShTOItems", "IDItemFromPL", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShTOItems", "IDItemFromPL", c => c.Int(nullable: false));
        }
    }
}
