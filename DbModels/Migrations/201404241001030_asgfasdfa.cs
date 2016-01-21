namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asgfasdfa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATActs", "UploadedToSHDate", c => c.DateTime());
           
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SATActs", "UploadedToSH", c => c.DateTime());
            DropColumn("dbo.SATActs", "UploadedToSHDate");
        }
    }
}
