namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asfash : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATActs", "UploadedToSH", c => c.DateTime());
            AlterColumn("dbo.SATActItems", "FactDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SATActItems", "FactDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.SATActs", "UploadedToSH");
        }
    }
}
