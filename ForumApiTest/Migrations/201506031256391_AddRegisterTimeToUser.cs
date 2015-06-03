namespace ForumApiTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRegisterTimeToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "RegisterTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "RegisterTime");
        }
    }
}
