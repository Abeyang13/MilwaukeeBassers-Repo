namespace FishingProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedProductOrder : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductOrders", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ProductOrders", "Paid", c => c.Boolean(nullable: false));
            DropColumn("dbo.Orders", "Total");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Total", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.ProductOrders", "Paid");
            DropColumn("dbo.ProductOrders", "Total");
        }
    }
}
