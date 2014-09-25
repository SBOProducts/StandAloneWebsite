namespace Customer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategorySequence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "Sequence", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Categories", "Sequence");
        }
    }
}
