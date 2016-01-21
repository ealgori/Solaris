namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOes", "POType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOes", "POType");
        }
    }
}
