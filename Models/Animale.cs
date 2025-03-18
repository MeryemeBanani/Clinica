using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Clinica.Models
{
    public class Animale
    {
        [Key]
        public string ID { get; set; } //chiave primaria
        public DateTime DataRegistrazione { get; set; }
        public string Nome { get; set; }
        public string Tipologia { get; set; }
        public string Colore { get; set; }
        public DateTime DataNascita { get; set; }
        public bool MicroChip { get; set; }
        public bool Smarrito { get; set; }
        public string Anagrafica_Proprietario { get; set; }
        public virtual ICollection<Visita> Visite { get; set; }
        public string PathFile { get; set; }
        public string NameFile { get; set; }

        
        [NotMapped]
        public HttpPostedFileBase file { get; set; }

    }
}