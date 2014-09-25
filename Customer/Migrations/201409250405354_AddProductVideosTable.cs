namespace Customer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductVideosTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProductVideos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        YouTubeUrl = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProductVideos");
        }
    }
}
