namespace DbModels.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dfdfadsf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ShAVRSs", "ObjectCreationDateTime", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "TaskSubcontractorNumber", c => c.String());
            AddColumn("dbo.ShAVRSs", "DepartmentManager", c => c.String());
            AddColumn("dbo.ShAVRSs", "BranchManagar", c => c.String());
            AddColumn("dbo.ShAVRSs", "FactVypolneniiaRabotPodtverzhdaiuCB", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShAVRSs", "FactVypolneniiaRabotPodtverzhdaiuRukOtd", c => c.String());
            AddColumn("dbo.ShAVRSs", "ZayavkaECRAdmPoluchenaVobrabotku", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "DataVipuskaPO", c => c.String());
            AddColumn("dbo.ShAVRSs", "PORotpravlenVOD", c => c.String());
            AddColumn("dbo.ShAVRSs", "KomentariiECRAdmKzayavke", c => c.String());
            AddColumn("dbo.ShAVRSs", "SentToSourcing", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "ApproovedBySoursing", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "Signed", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "SentToSubcontractor", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "BillNumber", c => c.String());
            AddColumn("dbo.ShAVRSs", "FacruteNumber", c => c.String());
            AddColumn("dbo.ShAVRSs", "KZDPoluchen", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "StatusRassmotreniya", c => c.String());
            AddColumn("dbo.ShAVRSs", "CommentarijRassmotreniya", c => c.String());
            AddColumn("dbo.ShAVRSs", "PeredanoVOplatu", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "OtpravlenoPodryadchikuDlyaRassmotreniya", c => c.DateTime());
            AddColumn("dbo.ShAVRSs", "DeliveryNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShAVRSs", "DeliveryNumber");
            DropColumn("dbo.ShAVRSs", "OtpravlenoPodryadchikuDlyaRassmotreniya");
            DropColumn("dbo.ShAVRSs", "PeredanoVOplatu");
            DropColumn("dbo.ShAVRSs", "CommentarijRassmotreniya");
            DropColumn("dbo.ShAVRSs", "StatusRassmotreniya");
            DropColumn("dbo.ShAVRSs", "KZDPoluchen");
            DropColumn("dbo.ShAVRSs", "FacruteNumber");
            DropColumn("dbo.ShAVRSs", "BillNumber");
            DropColumn("dbo.ShAVRSs", "SentToSubcontractor");
            DropColumn("dbo.ShAVRSs", "Signed");
            DropColumn("dbo.ShAVRSs", "ApproovedBySoursing");
            DropColumn("dbo.ShAVRSs", "SentToSourcing");
            DropColumn("dbo.ShAVRSs", "KomentariiECRAdmKzayavke");
            DropColumn("dbo.ShAVRSs", "PORotpravlenVOD");
            DropColumn("dbo.ShAVRSs", "DataVipuskaPO");
            DropColumn("dbo.ShAVRSs", "ZayavkaECRAdmPoluchenaVobrabotku");
            DropColumn("dbo.ShAVRSs", "FactVypolneniiaRabotPodtverzhdaiuRukOtd");
            DropColumn("dbo.ShAVRSs", "FactVypolneniiaRabotPodtverzhdaiuCB");
            DropColumn("dbo.ShAVRSs", "BranchManagar");
            DropColumn("dbo.ShAVRSs", "DepartmentManager");
            DropColumn("dbo.ShAVRSs", "TaskSubcontractorNumber");
            DropColumn("dbo.ShAVRSs", "ObjectCreationDateTime");
        }
    }
}
