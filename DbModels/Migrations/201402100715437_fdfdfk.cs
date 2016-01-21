namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdfk : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShContacts", "EMailAddress", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShContacts", "EMailAddress", c => c.String());
        }
    }
}
