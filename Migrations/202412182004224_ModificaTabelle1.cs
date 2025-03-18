        namespace Clinica.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModificaTabelle1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visite", "Smarrito", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Visite", "Smarrito");
        }
    }
}
