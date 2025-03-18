namespace Clinica.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AggiungiRelazioneTraAnimaleEVisita : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Visite", "AnimaleID", "dbo.Animali");
            DropIndex("dbo.Visite", new[] { "AnimaleID" });
            AlterColumn("dbo.Visite", "AnimaleID", c => c.String(nullable: false, maxLength: 128));
            CreateIndex("dbo.Visite", "AnimaleID");
            AddForeignKey("dbo.Visite", "AnimaleID", "dbo.Animali", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Visite", "AnimaleID", "dbo.Animali");
            DropIndex("dbo.Visite", new[] { "AnimaleID" });
            AlterColumn("dbo.Visite", "AnimaleID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Visite", "AnimaleID");
            AddForeignKey("dbo.Visite", "AnimaleID", "dbo.Animali", "ID");
        }
    }
}
