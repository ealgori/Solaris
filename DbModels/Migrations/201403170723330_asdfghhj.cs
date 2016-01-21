namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class asdfghhj : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PORActivities", "TOWorkDescription", c => c.String());
            AddColumn("dbo.SATTOes", "NomerDogovora", c => c.String());
            AddColumn("dbo.SATTOes", "DataDogovora", c => c.DateTime());
            AddColumn("dbo.ShTOes", "NomerDogovora", c => c.String());
            AddColumn("dbo.ShTOes", "DataDogovora", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOes", "DataDogovora");
            DropColumn("dbo.ShTOes", "NomerDogovora");
            DropColumn("dbo.SATTOes", "DataDogovora");
            DropColumn("dbo.SATTOes", "NomerDogovora");
            DropColumn("dbo.PORActivities", "TOWorkDescription");
        }
    }
}
