namespace BitBookMVCApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class profile_photo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Educations",
                c => new
                    {
                        EducationId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        EduTitle = c.String(),
                        EduInstitute = c.String(),
                        EduYear = c.String(),
                    })
                .PrimaryKey(t => t.EducationId);
            
            CreateTable(
                "dbo.Experiences",
                c => new
                    {
                        ExperienceId = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        ExpDesignation = c.String(),
                        ExpCompany = c.String(),
                        ExpYear = c.String(),
                    })
                .PrimaryKey(t => t.ExperienceId);
            
            AddColumn("dbo.Profiles", "ProfilePhoto", c => c.String());
            AddColumn("dbo.Profiles", "CoverPhoto", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "CoverPhoto");
            DropColumn("dbo.Profiles", "ProfilePhoto");
            DropTable("dbo.Experiences");
            DropTable("dbo.Educations");
        }
    }
}
