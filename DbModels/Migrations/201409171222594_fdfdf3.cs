namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdf3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SubContractors", "NameRef", c => c.String());
            AddColumn("dbo.ShAVRFs", "SubcontractorRef", c => c.String());
            AddColumn("dbo.ShAVRSs", "SubcontractorRef", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRSs", "SubcontractorRef");
            DropColumn("dbo.ShAVRFs", "SubcontractorRef");
            DropColumn("dbo.SubContractors", "NameRef");
        }
    }
}
