using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Clinica.Models
{
    public class Utente
    {
        [Key]
        public string UtenteID { get; set; } //sarebbe l'email
        public string Password { get; set; }
        public string Ruolo { get; set; }  
    }
}