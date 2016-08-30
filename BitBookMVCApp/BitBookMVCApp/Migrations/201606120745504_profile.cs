namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class profile : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        ProfileId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        Email = c.String(),
                        Gender = c.String(),
                        Contact = c.String(),
                        City = c.String(),
                        Country = c.String(),
                        AreaOfInterest = c.String(),
                    })
                .PrimaryKey(t => t.ProfileId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Profiles");
        }
    }
}
