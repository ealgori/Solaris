namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class afgbgsdfg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShSITEs", "Index", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShSITEs", "Index");
        }
    }
}
