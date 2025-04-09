namespace Clinica.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NomeMigrazione : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Animali", "Nome", c => c.String(nullable: false));
            AlterColumn("dbo.Animali", "Tipologia", c => c.String(nullable: false));
            AlterColumn("dbo.Animali", "Colore", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Animali", "Colore", c => c.String());
            AlterColumn("dbo.Animali", "Tipologia", c => c.String());
            AlterColumn("dbo.Animali", "Nome", c => c.String());
        }
    }
}
