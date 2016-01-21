namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdf1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShSITEs", "DataVvodaVeqspluatatciu", c => c.DateTime());
            AddColumn("dbo.ShSITEs", "TipAMS", c => c.String());
            AddColumn("dbo.ShSITEs", "KolvoAMS", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ShSITEs", "VidTOAMS", c => c.String());
            AddColumn("dbo.ShSITEs", "MarkaSKV", c => c.String());
            AddColumn("dbo.ShSITEs", "KolvoSKV", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ShSITEs", "TipSKV", c => c.String());
            AddColumn("dbo.ShSITEs", "VidTOSKV", c => c.String());
            AddColumn("dbo.ShSITEs", "TippSKV", c => c.String());
            AddColumn("dbo.ShSITEs", "KolvopSKV", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ShSITEs", "TipMobilnoiGU", c => c.String());
            AddColumn("dbo.ShSITEs", "TipStatcionarnoiGU", c => c.String());
            AddColumn("dbo.ShSITEs", "TipAUGPT", c => c.String());
            AddColumn("dbo.ShSITEs", "KolvoAUGPT", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("dbo.ShTOes", "TOType", c => c.String());
            AddColumn("dbo.ShTOes", "TOTotalAmmountApproved", c => c.String());
            AddColumn("dbo.ShTOes", "TOTotalAmmount", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShTOes", "TOTotalAmmount");
            DropColumn("dbo.ShTOes", "TOTotalAmmountApproved");
            DropColumn("dbo.ShTOes", "TOType");
            DropColumn("dbo.ShSITEs", "KolvoAUGPT");
            DropColumn("dbo.ShSITEs", "TipAUGPT");
            DropColumn("dbo.ShSITEs", "TipStatcionarnoiGU");
            DropColumn("dbo.ShSITEs", "TipMobilnoiGU");
            DropColumn("dbo.ShSITEs", "KolvopSKV");
            DropColumn("dbo.ShSITEs", "TippSKV");
            DropColumn("dbo.ShSITEs", "VidTOSKV");
            DropColumn("dbo.ShSITEs", "TipSKV");
            DropColumn("dbo.ShSITEs", "KolvoSKV");
            DropColumn("dbo.ShSITEs", "MarkaSKV");
            DropColumn("dbo.ShSITEs", "VidTOAMS");
            DropColumn("dbo.ShSITEs", "KolvoAMS");
            DropColumn("dbo.ShSITEs", "TipAMS");
            DropColumn("dbo.ShSITEs", "DataVvodaVeqspluatatciu");
        }
    }
}
