namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfghhjh : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOes", "WorkDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOes", "WorkDescription");
        }
    }
}
