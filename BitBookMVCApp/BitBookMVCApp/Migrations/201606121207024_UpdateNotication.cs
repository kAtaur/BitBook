namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateNotication : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UpdateNotifications",
                c => new
                    {
                        UpdateNotificationId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Content = c.String(),
                        ContentDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UpdateNotificationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UpdateNotifications");
        }
    }
}
