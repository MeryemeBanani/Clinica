using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Clinica.Models
{
    [Table("Visite")]
    public class Visita
    {
        [Key]
        public int ID { get; set; } //chiave primaria, auto-incremento gestito da database
        public string AnimaleID { get; set; } //chiave esterna

        [ForeignKey("AnimaleID")]
        public Animale Animale { get; set; }  

        [DataType(DataType.DateTime)]
        public DateTime DataRegistrazione { get; set; }
        public string Nome { get; set; }
        public string Tipologia { get; set;}
        public string Colore { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataNascita { get; set; }
        public bool MicroChip { get; set; }
        public DateTime DataVisita { get; set; }
        public bool Necessita_Ricovero { get; set; }

        public string TipoVisita { get; set; }
        public bool Smarrito { get; set; }
    }
}