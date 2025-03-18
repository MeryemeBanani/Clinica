using Clinica.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using System.Drawing;
using System.Web.Helpers;


namespace Clinica.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClinicaContext db = new ClinicaContext();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]

        [ValidateAntiForgeryToken]
        public ActionResult Login(Utente user)
        {
            if (ModelState.IsValid)
            {

                var utente = db.Utenti.FirstOrDefault(u => u.UtenteID == user.UtenteID);

                if (utente != null && user.Password == utente.Password)
                {
                    FormsAuthentication.SetAuthCookie(utente.UtenteID, false);  // Crea il cookie di autenticazione
                    Response.Cookies["Ruolo"].Value = utente.Ruolo;
                    Response.Cookies["Ruolo"].Expires = DateTime.Now.AddMinutes(30);

                    return RedirectToAction("Index", "Home");  // Reindirizza alla pagina principale
                }
                else
                {
                    TempData["ErrorMessage"] = "Credenziali non valide!";
                }
            }

            return View();

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]

        [HttpPost]
        public ActionResult Cronistoria(string ID)
        {

            try
            {
                List<Visita> visite = db.Visite.Where(v => v.AnimaleID == ID).OrderByDescending(v => v.DataVisita).ToList();

                foreach (Visita v in visite)
                {
                    v.Animale = db.Animali.FirstOrDefault(m => m.ID == v.AnimaleID);
                }


                if (!visite.Any())
                {
                    TempData["Messaggio"] = "Nessuna visita trovata.";
                    return RedirectToAction("Index");
                }

                return View(visite);
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Si è verificato un errore nel recupero delle visite: " + ex.Message;
                return View();
            }

        }

        [Authorize]

        [HttpGet]
        public ActionResult RegistraAnimale()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken] // Per protezione CSRF
        public ActionResult RegistraAnimale(Animale animale)
        {

            try
            {
                if (ModelState.IsValid)
                {

                    if (animale.file != null && animale.file.ContentLength > 0)
                    {
                        string fileExt = Path.GetExtension(animale.file.FileName);
                        string fileNameUnique = $"{DateTime.Now.Ticks}{fileExt}";
                        string PathFile = Path.Combine(Server.MapPath("~/Content/imgUpload"), fileNameUnique);
                        animale.file.SaveAs(PathFile);
                        animale.PathFile = PathFile;
                        animale.NameFile = fileNameUnique;
                    }


                    animale.DataRegistrazione = DateTime.Now;


                    db.Animali.Add(animale);
                    db.SaveChanges();


                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                TempData["ErroreDB"] = "Errore inserimento nel database" + ex.Message;
            }


            return View(animale);
        }

        [Authorize]

        [HttpGet]
        public ActionResult AggiungiVisita(string id) //passo l'id dell'animale
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["MessaggioAdd"] = "Codice microchip non valido!";
                return RedirectToAction("Index");
            }

            var animale = db.Animali.FirstOrDefault(a => a.ID == id);

            if (animale == null)
            {
                TempData["MessaggioAdd"] = "Animale non trovato!";
                return RedirectToAction("Index");
            }


            Visita visita = new Visita
            {
                AnimaleID = animale.ID,
                Animale = animale,
                DataRegistrazione = animale.DataRegistrazione,
                Nome = animale.Nome,
                Tipologia = animale.Tipologia,
                Colore = animale.Colore,
                DataNascita = animale.DataNascita,
                MicroChip = animale.MicroChip,
                Smarrito = animale.Smarrito

            };

            return View(visita);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AggiungiVisita(Visita visita)
        {

            db.Visite.Add(visita);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize]

        [HttpGet]
        public ActionResult Contabilizzazione()
        {

            return View();

        }

        [Authorize]

        [HttpGet]
        public ActionResult GetRicoveriAttivi()
        {
            var ricoveriAttivi = db.Visite.Where(v => v.Necessita_Ricovero && v.Smarrito).Select(v => new
            {
                v.ID,
                v.AnimaleID,
                NomeAnimale = v.Animale.Nome,
                v.DataVisita, // lasciamo la data senza formattarla qui
                v.TipoVisita,
                v.Necessita_Ricovero
            }).ToList().Select(v => new
            {
                v.ID,
                v.AnimaleID,
                v.NomeAnimale,
                DataVisita = v.DataVisita.ToString("dd-MM-yyyy"),
                v.TipoVisita,
                v.Necessita_Ricovero
            }).ToList();



            return Json(ricoveriAttivi, JsonRequestBehavior.AllowGet);
        }

        [Authorize]

        [HttpPost]

        public ActionResult AggiornaRicovero(int id, bool necessitaRicovero)
        {

            var visita = db.Visite.FirstOrDefault(v => v.ID == id);  //prendo la visita dal db con lo stesso id
            if (visita != null)
            {
                visita.Necessita_Ricovero = necessitaRicovero;
                db.SaveChanges(); //aggiorno il db con la nuova necessità
                return Json(new { success = true });
            }
            return Json(new { success = false });


        }



        [HttpGet]
        public ActionResult CercaAnimale()
        {
            return View();
        }


        [HttpGet]
        public JsonResult VerificaRicovero(string microchip)
        {

            var visita = db.Visite.Where(v => v.AnimaleID == microchip && v.Necessita_Ricovero == true).Select(v => new
            {
                v.AnimaleID,
                v.Nome,
                NomeFile = v.Animale.NameFile,
                v.Necessita_Ricovero
            })
                           .FirstOrDefault();

            if (visita != null)
            {

                return Json(new
                {
                    isRicoverato = true,
                    visita.AnimaleID,
                    visita.Nome,
                    visita.NomeFile
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new { isRicoverato = false }, JsonRequestBehavior.AllowGet);
            }

        }

        [Authorize(Roles="Admin")]

        [HttpGet]
        public ActionResult API()
        {

           
                return View();
         


        }

        [Authorize(Roles = "Admin")]

        [HttpGet]

        public ActionResult ListaVisiteDaMicroChip(string microchip)
        {
           
                var visite = db.Visite.Where(v => v.AnimaleID == microchip).Select(v => new
                {
                    v.AnimaleID,
                    v.Nome,
                    NomeFile = v.Animale.NameFile,
                    v.Necessita_Ricovero
                });

                //ho estrapolato una SERIE DI VISITE dello stesso animale


                if (visite.Any())
                {
                    return Json(new
                    {
                        CiSonoVisite = true,
                        Visite = visite
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { CiSonoVisite = false }, JsonRequestBehavior.AllowGet);
                }   

        }

        [Authorize(Roles = "Admin")]

        [HttpGet]
        public ActionResult ListaAnimaliDaTipologia(string tipologia)
        {
                {
                    var animali = db.Animali.Where(a => db.Visite.Any(v => v.AnimaleID == a.ID && v.Necessita_Ricovero && a.Tipologia == tipologia)).Select(a => new
                    {
                        a.ID,
                        a.Nome,
                        a.Colore,
                        NomeFile = a.NameFile,
                        a.DataNascita,
                        a.Anagrafica_Proprietario

                    });

                    //ho estrapolato una SERIE DI ANIMALI della stessa tipologia, non ho usato come tipi cane/gatto ma direttamente le razze, prendo solo quelli che hanno  Necessita ricovero true


                    if (animali.Any())
                    {
                        return Json(new
                        {
                            CiSonoAnimali = true,
                            Animali = animali

                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { CiSonoAnimali = false }, JsonRequestBehavior.AllowGet);
                    }

                }
          
        }

    }
}