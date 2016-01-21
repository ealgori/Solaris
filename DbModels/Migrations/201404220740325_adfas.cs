namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adfas : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SATActItems", "ShId", c => c.String());
            AlterColumn("dbo.ShMatToItems", "MatTOId", c => c.String(nullable: false, maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShMatToItems", "MatTOId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.SATActItems", "ShId", c => c.Int(nullable: false));
        }
    }
}
