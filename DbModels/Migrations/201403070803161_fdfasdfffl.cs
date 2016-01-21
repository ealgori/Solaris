namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfasdfffl : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShTOItems", "PLItemRevisionID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShTOItems", "PLItemRevisionID", c => c.Int(nullable: false));
        }
    }
}
