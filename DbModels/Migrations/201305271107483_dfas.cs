namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PORs", "UploadedToSH", c => c.DateTime());
            AddColumn("dbo.PORs", "Discriminator", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PORs", "Discriminator");
            DropColumn("dbo.PORs", "UploadedToSH");
        }
    }
}
