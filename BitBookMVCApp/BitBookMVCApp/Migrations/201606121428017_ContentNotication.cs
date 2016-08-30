namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ContentNotication : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ContentNotifications",
                c => new
                    {
                        ContentNotificationId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Content = c.String(),
                        RequestDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ContentNotificationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ContentNotifications");
        }
    }
}
