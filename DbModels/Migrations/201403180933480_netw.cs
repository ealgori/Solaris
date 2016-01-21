namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class netw : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SATTOes", "Network", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOes", "Network");
        }
    }
}
