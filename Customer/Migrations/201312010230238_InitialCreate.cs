namespace Customer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MenuName = c.String(nullable: false, maxLength: 60),
                        Url = c.String(maxLength: 4000),
                        PageTitle = c.String(nullable: false, maxLength: 4000),
                        Description = c.String(maxLength: 4000),
                        KeyWords = c.String(maxLength: 4000),
                        IsPrivate = c.Boolean(nullable: false),
                        ContentID = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ContactForms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sent = c.DateTime(nullable: false),
                        Subject = c.String(nullable: false, maxLength: 4000),
                        EmailAddress = c.String(nullable: false, maxLength: 4000),
                        From = c.String(nullable: false, maxLength: 4000),
                        Message = c.String(nullable: false, maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HtmlContents",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        ImageFolder = c.Guid(nullable: false),
                        HTML = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ImageSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(maxLength: 4000),
                        ShowWatermark = c.Boolean(nullable: false),
                        Watermark = c.String(maxLength: 4000),
                        WatermarkOpacity = c.Int(nullable: false),
                        WatermarkHorizontalPosition = c.String(maxLength: 4000),
                        WatermarkVerticalPosition = c.String(maxLength: 4000),
                        MaxSize = c.Int(nullable: false),
                        MakeSquareImage = c.Boolean(nullable: false),
                        BackgroundColor = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MyAccounts",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        BusinessName = c.String(nullable: false, maxLength: 4000),
                        EmailAddress = c.String(nullable: false, maxLength: 4000),
                        SiteScripts = c.String(maxLength: 4000),
                        SiteStyle = c.String(maxLength: 4000),
                        LogoImage = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CategoryID = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 60),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 4000),
                        ImageFolder = c.Guid(nullable: false),
                        Tags = c.String(maxLength: 4000),
                        Created = c.DateTime(nullable: false),
                        IsPrivate = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AccountRegistrationCodes",
                c => new
                    {
                        ID = c.Guid(nullable: false),
                        Used = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.SiteImages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageFolder = c.Guid(nullable: false),
                        Sequence = c.Int(nullable: false),
                        Name = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SiteImages");
            DropTable("dbo.AccountRegistrationCodes");
            DropTable("dbo.Products");
            DropTable("dbo.MyAccounts");
            DropTable("dbo.ImageSettings");
            DropTable("dbo.HtmlContents");
            DropTable("dbo.ContactForms");
            DropTable("dbo.Categories");
        }
    }
}
