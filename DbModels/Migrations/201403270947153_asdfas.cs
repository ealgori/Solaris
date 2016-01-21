namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfas : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOes", "WorkDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOes", "WorkDescription");
        }
    }
}
