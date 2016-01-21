namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfa1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOes", "PrintSOW", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOes", "PrintSOW");
        }
    }
}
