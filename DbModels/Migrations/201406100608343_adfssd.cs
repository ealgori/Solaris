namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adfssd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShSITEs", "KolvoStacionarnihGU", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ShSITEs", "KolvoMobilnihGU", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ShSITEs", "KolvoGU");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShSITEs", "KolvoGU", c => c.Decimal(precision: 18, scale: 2));
            DropColumn("dbo.ShSITEs", "KolvoMobilnihGU");
            DropColumn("dbo.ShSITEs", "KolvoStacionarnihGU");
        }
    }
}
