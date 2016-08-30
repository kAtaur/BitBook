namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModelPostComment : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PostComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        PostId = c.Int(nullable: false),
                        UserComment = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PostComments");
        }
    }
}
