namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfdf2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WIHRequests", "RejectReason", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WIHRequests", "RejectReason");
        }
    }
}
