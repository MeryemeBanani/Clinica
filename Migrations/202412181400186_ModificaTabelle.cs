namespace Clinica.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModificaTabelle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Visite", "DataVisita", c => c.DateTime(nullable: false));
            AddColumn("dbo.Visite", "Necessita_Ricovero", c => c.Boolean(nullable: false));
            AddColumn("dbo.Visite", "TipoVisita", c => c.String());
            DropColumn("dbo.Visite", "Anagrafica_Proprietario");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Visite", "Anagrafica_Proprietario", c => c.String());
            DropColumn("dbo.Visite", "TipoVisita");
            DropColumn("dbo.Visite", "Necessita_Ricovero");
            DropColumn("dbo.Visite", "DataVisita");
        }
    }
}
