namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class post : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        PostId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        UserName = c.String(),
                        TextPost = c.String(),
                        ImgPost = c.String(),
                        PostDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.PostId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Posts");
        }
    }
}
