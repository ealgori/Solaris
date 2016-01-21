namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfasd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PORs", "AVRId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PORs", "AVRId");
        }
    }
}
