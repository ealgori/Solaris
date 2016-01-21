namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addmigration5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRItems", "ECRType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRItems", "ECRType");
        }
    }
}
