namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adfaf : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SATActItems", "SATAct_Id1", "dbo.SATActs");
            DropForeignKey("dbo.SATActItems", "SATAct_Id2", "dbo.SATActs");
            DropIndex("dbo.SATActItems", new[] { "SATAct_Id1" });
            DropIndex("dbo.SATActItems", new[] { "SATAct_Id2" });
            DropColumn("dbo.SATActItems", "SATAct_Id1");
            DropColumn("dbo.SATActItems", "SATAct_Id2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SATActItems", "SATAct_Id2", c => c.Int());
            AddColumn("dbo.SATActItems", "SATAct_Id1", c => c.Int());
            CreateIndex("dbo.SATActItems", "SATAct_Id2");
            CreateIndex("dbo.SATActItems", "SATAct_Id1");
            AddForeignKey("dbo.SATActItems", "SATAct_Id2", "dbo.SATActs", "Id");
            AddForeignKey("dbo.SATActItems", "SATAct_Id1", "dbo.SATActs", "Id");
        }
    }
}
