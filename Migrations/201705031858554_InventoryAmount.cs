namespace CatsCloset.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InventoryAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "InventoryAmount", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "InventoryAmount");
        }
    }
}
