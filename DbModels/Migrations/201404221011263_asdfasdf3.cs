namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfasdf3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATActs", "UploadToSHComment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATActs", "UploadToSHComment");
        }
    }
}
