namespace FishingProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedProductOrder2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductOrders", "Size", c => c.String());
            AddColumn("dbo.ProductOrders", "Quantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductOrders", "Quantity");
            DropColumn("dbo.ProductOrders", "Size");
        }
    }
}
