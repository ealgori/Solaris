namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOes", "Subcontractor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOes", "Subcontractor");
        }
    }
}
