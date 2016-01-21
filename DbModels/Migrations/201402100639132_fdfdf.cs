namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdf : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShContacts",
                c => new
                    {
                        Contact = c.String(nullable: false, maxLength: 128),
                        EmailAddress = c.String(),
                    })
                .PrimaryKey(t => t.Contact);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShContacts");
        }
    }
}
