namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class coverPhoto : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CoverPhotoes",
                c => new
                    {
                        CoverPhotoId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        CoverImage = c.String(),
                    })
                .PrimaryKey(t => t.CoverPhotoId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CoverPhotoes");
        }
    }
}
