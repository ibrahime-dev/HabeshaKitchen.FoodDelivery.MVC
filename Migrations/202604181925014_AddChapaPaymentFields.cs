namespace FoodDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChapaPaymentFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "TxRef", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "TxRef");
        }
    }
}
