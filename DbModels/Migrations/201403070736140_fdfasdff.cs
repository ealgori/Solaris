namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfasdff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOItems", "PLItemRevisionID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOItems", "PLItemRevisionID");
        }
    }
}
