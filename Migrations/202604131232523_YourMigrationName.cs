namespace FoodDelivery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class YourMigrationName : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "Restaurant_Id", "dbo.Restaurants");
            DropIndex("dbo.AspNetUsers", new[] { "Restaurant_Id" });
            DropColumn("dbo.AspNetUsers", "Restaurant_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Restaurant_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "Restaurant_Id");
            AddForeignKey("dbo.AspNetUsers", "Restaurant_Id", "dbo.Restaurants", "Id");
        }
    }
}
