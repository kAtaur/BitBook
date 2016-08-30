namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FriendStatus : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Friends", "Status", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Friends", "Status", c => c.Int(nullable: false));
        }
    }
}
