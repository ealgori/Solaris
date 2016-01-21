namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class gfasaa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PORNetworks", "SiteBranch", c => c.String());
            AddColumn("dbo.SATTOes", "SubContractorSapNumber", c => c.String());
            AddColumn("dbo.SATTOes", "SubContractorAddress", c => c.String());
            AddColumn("dbo.SATTOes", "ProceListNumbers", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SATTOes", "ProceListNumbers");
            DropColumn("dbo.SATTOes", "SubContractorAddress");
            DropColumn("dbo.SATTOes", "SubContractorSapNumber");
            DropColumn("dbo.PORNetworks", "SiteBranch");
        }
    }
}
