namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fdfdf5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PORItems", "Coeff", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PORItems", "Coeff");
        }
    }
}
