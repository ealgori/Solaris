namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfasdfdg : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SATTOes", "Region", c => c.String());
            AlterColumn("dbo.SATTOes", "Branch", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SATTOes", "Branch", c => c.String(nullable: false));
            AlterColumn("dbo.SATTOes", "Region", c => c.String(nullable: false));
        }
    }
}
