namespace FoodDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixPaymentRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Payments", "Id", "dbo.Orders");
            DropColumn("dbo.Payments", "OrderId");
            RenameColumn(table: "dbo.Payments", name: "Id", newName: "OrderId");
            RenameIndex(table: "dbo.Payments", name: "IX_Id", newName: "IX_OrderId");
            DropPrimaryKey("dbo.Payments");
            AddColumn("dbo.Orders", "Payment_Id", c => c.Int());
            AddColumn("dbo.Payments", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Payments", "Id");
            CreateIndex("dbo.Orders", "Payment_Id");
            AddForeignKey("dbo.Orders", "Payment_Id", "dbo.Payments", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "Payment_Id", "dbo.Payments");
            DropIndex("dbo.Orders", new[] { "Payment_Id" });
            DropPrimaryKey("dbo.Payments");
            DropColumn("dbo.Payments", "Id");
            DropColumn("dbo.Orders", "Payment_Id");
            RenameIndex(table: "dbo.Payments", name: "IX_OrderId", newName: "IX_Id");
            RenameColumn(table: "dbo.Payments", name: "OrderId", newName: "Id");
            AddPrimaryKey("dbo.Payments", "Id");
            AddColumn("dbo.Payments", "OrderId", c => c.Int(nullable: false));
            AddForeignKey("dbo.Payments", "Id", "dbo.Orders", "Id", cascadeDelete: true);
        }
    }
}
