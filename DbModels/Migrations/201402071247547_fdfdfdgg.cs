namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdfdgg : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PORNetworks", "Network2014", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PORNetworks", "Network2014", c => c.String());
        }
    }
}
