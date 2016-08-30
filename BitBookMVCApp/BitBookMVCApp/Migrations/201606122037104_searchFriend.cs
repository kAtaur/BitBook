namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class searchFriend : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.SearchFriends");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SearchFriends",
                c => new
                    {
                        SearchFriendId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        UserName = c.String(),
                        ProfilePhoto = c.String(),
                    })
                .PrimaryKey(t => t.SearchFriendId);
            
        }
    }
}
