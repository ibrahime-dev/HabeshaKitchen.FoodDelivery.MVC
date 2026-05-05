namespace FoodDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMenuItemImageUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "ImageUrl", c => c.String(maxLength: 300));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MenuItems", "ImageUrl");
        }
    }
}
