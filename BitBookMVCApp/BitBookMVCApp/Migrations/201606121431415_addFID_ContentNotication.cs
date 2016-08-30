namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFID_ContentNotication : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContentNotifications", "FriendId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContentNotifications", "FriendId");
        }
    }
}
