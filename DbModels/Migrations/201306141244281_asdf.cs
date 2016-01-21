namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PORItems", "Site", c => c.String());
            AddColumn("dbo.PORItems", "FIX", c => c.String());
            AddColumn("dbo.PORItems", "FOL", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PORItems", "FOL");
            DropColumn("dbo.PORItems", "FIX");
            DropColumn("dbo.PORItems", "Site");
        }
    }
}
