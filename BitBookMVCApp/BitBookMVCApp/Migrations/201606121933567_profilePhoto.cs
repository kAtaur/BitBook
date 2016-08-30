namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class profilePhoto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProfilePhotoes",
                c => new
                    {
                        ProfilePhotoId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ProfileImage = c.String(),
                    })
                .PrimaryKey(t => t.ProfilePhotoId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProfilePhotoes");
        }
    }
}
