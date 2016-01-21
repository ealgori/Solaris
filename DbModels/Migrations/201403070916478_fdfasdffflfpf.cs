namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfasdffflfpf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShSITEs", "KolvoGU", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShSITEs", "KolvoGU");
        }
    }
}
