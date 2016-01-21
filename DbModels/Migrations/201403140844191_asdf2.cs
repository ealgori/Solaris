namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdf2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShContacts", "SubcFace", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShContacts", "SubcFace");
        }
    }
}
