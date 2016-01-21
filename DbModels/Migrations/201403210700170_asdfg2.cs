namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfg2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShWIHRequests", "RequestSentToODdate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShWIHRequests", "RequestSentToODdate", c => c.DateTime(nullable: false));
        }
    }
}
