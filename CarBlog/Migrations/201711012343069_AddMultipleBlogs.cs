namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMultipleBlogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Blogs",
                c => new
                    {
                        Url = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Url);
            
            AddColumn("dbo.Comments", "BlogUrl", c => c.String(maxLength: 50));
            AddColumn("dbo.Publications", "BlogUrl", c => c.String(maxLength: 50));
            AddColumn("dbo.AspNetUsers", "BlogUrl", c => c.String(maxLength: 50));
            CreateIndex("dbo.Comments", "BlogUrl");
            CreateIndex("dbo.Publications", "BlogUrl");
            CreateIndex("dbo.AspNetUsers", "BlogUrl");
            AddForeignKey("dbo.Comments", "BlogUrl", "dbo.Blogs", "Url");
            AddForeignKey("dbo.Publications", "BlogUrl", "dbo.Blogs", "Url");
            AddForeignKey("dbo.AspNetUsers", "BlogUrl", "dbo.Blogs", "Url");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "BlogUrl", "dbo.Blogs");
            DropForeignKey("dbo.Publications", "BlogUrl", "dbo.Blogs");
            DropForeignKey("dbo.Comments", "BlogUrl", "dbo.Blogs");
            DropIndex("dbo.AspNetUsers", new[] { "BlogUrl" });
            DropIndex("dbo.Publications", new[] { "BlogUrl" });
            DropIndex("dbo.Comments", new[] { "BlogUrl" });
            DropColumn("dbo.AspNetUsers", "BlogUrl");
            DropColumn("dbo.Publications", "BlogUrl");
            DropColumn("dbo.Comments", "BlogUrl");
            DropTable("dbo.Blogs");
        }
    }
}
