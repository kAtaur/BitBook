namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateNotificationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContentNotifications", "PostId", c => c.String());
            AddColumn("dbo.ContentNotifications", "TextPost", c => c.String());
            AddColumn("dbo.ContentNotifications", "ImagePost", c => c.String());
            AddColumn("dbo.ContentNotifications", "UserName", c => c.String());
            AddColumn("dbo.ContentNotifications", "Notification", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContentNotifications", "Notification");
            DropColumn("dbo.ContentNotifications", "UserName");
            DropColumn("dbo.ContentNotifications", "ImagePost");
            DropColumn("dbo.ContentNotifications", "TextPost");
            DropColumn("dbo.ContentNotifications", "PostId");
        }
    }
}
