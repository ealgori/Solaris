namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdf : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShFilialStructs",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 128),
                        RukFills = c.String(),
                        Engineers = c.String(),
                    })
                .PrimaryKey(t => t.Name);
            
            AddColumn("dbo.ShActs", "SendToSubcontracor", c => c.DateTime());
            AddColumn("dbo.ShActs", "CreateDate", c => c.DateTime());
            AddColumn("dbo.ShLimits", "Alias", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShLimits", "Alias");
            DropColumn("dbo.ShActs", "CreateDate");
            DropColumn("dbo.ShActs", "SendToSubcontracor");
            DropTable("dbo.ShFilialStructs");
        }
    }
}
