namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfdff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOes", "CreateUserDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.SATTOItems", "TOItemId", c => c.String());
            AlterColumn("dbo.SATTOes", "TO", c => c.String(nullable: false));
            AlterColumn("dbo.SATTOes", "Activity", c => c.String(nullable: false));
            AlterColumn("dbo.SATTOes", "SubContractor", c => c.String(nullable: false));
            AlterColumn("dbo.SATTOes", "ToType", c => c.String(nullable: false));
            AlterColumn("dbo.SATTOes", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.SATTOes", "Region", c => c.String(nullable: false));
            AlterColumn("dbo.SATTOes", "Branch", c => c.String(nullable: false));
            AlterColumn("dbo.SATTOes", "CreateUserName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SATTOes", "CreateUserName", c => c.String());
            AlterColumn("dbo.SATTOes", "Branch", c => c.String());
            AlterColumn("dbo.SATTOes", "Region", c => c.String());
            AlterColumn("dbo.SATTOes", "Total", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.SATTOes", "ToType", c => c.String());
            AlterColumn("dbo.SATTOes", "SubContractor", c => c.String());
            AlterColumn("dbo.SATTOes", "Activity", c => c.String());
            AlterColumn("dbo.SATTOes", "TO", c => c.String());
            AlterColumn("dbo.SATTOItems", "TOItemId", c => c.Int(nullable: false));
            DropColumn("dbo.SATTOes", "CreateUserDate");
        }
    }
}
