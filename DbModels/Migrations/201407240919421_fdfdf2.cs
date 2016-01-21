namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdf2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShTOItems", "ReasonForPartialClosure", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOItems", "ReasonForPartialClosure");
        }
    }
}
