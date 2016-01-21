namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubContractors", "ShName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SubContractors", "ShName");
        }
    }
}
