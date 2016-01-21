namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dhhr : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ShMatToItems",
                c => new
                    {
                        MatTOId = c.Int(nullable: false, identity: true),
                        TOId = c.String(),
                        Unit = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        Price = c.Decimal(precision: 18, scale: 2),
                        IDItemFromPL = c.Int(),
                        PLItemRevisionID = c.Int(),
                    })
                .PrimaryKey(t => t.MatTOId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ShMatToItems");
        }
    }
}
