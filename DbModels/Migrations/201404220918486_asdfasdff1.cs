namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfasdff1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATActs", "CreateName", c => c.String());
            AddColumn("dbo.SATActs", "CreateDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATActs", "CreateDate");
            DropColumn("dbo.SATActs", "CreateName");
        }
    }
}
