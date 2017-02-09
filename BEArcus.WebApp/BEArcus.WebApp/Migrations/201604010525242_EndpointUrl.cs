namespace BEArcus.WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EndpointUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "EndpointUrl", c => c.String());
            AddColumn("dbo.AspNetUsers", "AuthorizationKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AuthorizationKey");
            DropColumn("dbo.AspNetUsers", "EndpointUrl");
        }
    }
}
