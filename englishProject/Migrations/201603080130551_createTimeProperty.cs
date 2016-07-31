namespace englishProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createTimeProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "createTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "createTime");
        }
    }
}
