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
                // sotto posso usare la versione hashtata della password per maggiore sicurezza, per ora visto che  non c'è il campo registrazione utente uso le password in chiaro nel db e anche qui (COSA DA NON FARE!)
                //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password); questa è la riga di codice per salvare le password hashtate e nel controllo sarebbe Crypto.VerifyHashedPassword(utente.Password, user.Password)

                if (utente != null && utente.Password== user.Password)
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
            if (string.IsNullOrWhiteSpace(ID))
            {
                TempData["Messaggio"] = "ID non valido.";
                return RedirectToAction("Index");
            }


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
                        try
                        {
                            string uploadFolder = Server.MapPath("~/Content/imgUpload");

                            // Assicura che la cartella esista
                            if (!Directory.Exists(uploadFolder))
                            {
                                Directory.CreateDirectory(uploadFolder);
                            }

                            string fileExt = Path.GetExtension(animale.file.FileName);
                            string fileNameUnique = $"{DateTime.Now.Ticks}{fileExt}";
                            string savedFilePath = Path.Combine(uploadFolder, fileNameUnique);

                            using (var fileStream = System.IO.File.Create(savedFilePath))
                            {
                                animale.file.InputStream.CopyTo(fileStream);
                            }

                            animale.PathFile = "/Content/imgUpload/" + fileNameUnique; // Path relativo per visualizzazione
                            animale.NameFile = fileNameUnique;
                        }
                        catch (Exception ex)
                        {
                            TempData["ErroreFile"] = "Errore durante il salvataggio del file: " + ex.Message;
                            return View(animale);
                        }
                    }

                    animale.DataRegistrazione = DateTime.Now;

                    db.Animali.Add(animale);
                    db.SaveChanges(); // Salvataggio nel database

                    TempData["AggiuntaAnimale"] = "Animale aggiunto con successo!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErroreDB"] = "Dati non validi. Controlla i campi.";
                    return View(animale);
                }
            }
            catch (Exception ex)
            {
                TempData["ErroreDB"] = "Errore inserimento nel database: " + ex.Message;
                if (ex.InnerException != null)
                {
                    TempData["ErroreDB"] += " Dettagli: " + ex.InnerException.Message;
                }
                return View(animale);
            }
        }


        [Authorize]

        [HttpGet]
        public ActionResult AggiungiVisita(string id) //passo l'id dell'animale
        {
            if (string.IsNullOrWhiteSpace(id))
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
                Smarrito = animale.Smarrito,
                DataVisita = DateTime.Now //verrà poi modificato dall'utente, metto questa riga solo per far partire l'anno dal 2025 e non dal 0001


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

        public JsonResult GetRicoveriAttivi()
        {
            
            var ricoveriAttivi = db.Visite
                .Where(v => v.Necessita_Ricovero)
                .Select(v => new
                {
                    v.ID,
                    v.AnimaleID,
                    NomeAnimale = v.Animale.Nome,
                    DataVisita = v.DataVisita, // Recupera la data senza formattarla
                    v.TipoVisita,
                    v.Necessita_Ricovero
                })
                .ToList() // Esegui la query in memoria
                .Select(v => new
                {
                    v.ID,
                    v.AnimaleID,
                    v.NomeAnimale,
                    DataVisita = v.DataVisita.ToString("yyyy-MM-dd"), // Formatta la data dopo che è stata recuperata
                    v.TipoVisita,
                    v.Necessita_Ricovero
                })
                .ToList();

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

                // Recupera i ricoveri attivi aggiornati
                var ricoveriAttivi = db.Visite.Where(v => v.Necessita_Ricovero && v.Smarrito)
                                              .Select(v => new
                                              {
                                                  v.ID,
                                                  v.AnimaleID,
                                                  NomeAnimale = v.Animale.Nome,
                                                  v.DataVisita,
                                                  v.TipoVisita,
                                                  v.Necessita_Ricovero
                                              })
                                              .ToList()
                                              .Select(v => new
                                              {
                                                  v.ID,
                                                  v.AnimaleID,
                                                  v.NomeAnimale,
                                                  DataVisita = v.DataVisita.ToString("dd-MM-yyyy"),
                                                  v.TipoVisita,
                                                  v.Necessita_Ricovero
                                              })
                                              .ToList();

                return Json(new { success = true, ricoveriAttivi = ricoveriAttivi });
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
            if (string.IsNullOrWhiteSpace(microchip))
            {
                return Json(new { isRicoverato = false, messaggio = "Microchip mancante o non valido." }, JsonRequestBehavior.AllowGet);
            }


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
            if (string.IsNullOrWhiteSpace(microchip))
            {
                return Json(new { CiSonoVisite = false, messaggio = "Microchip mancante o non valido." }, JsonRequestBehavior.AllowGet);
            }


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
            if (string.IsNullOrWhiteSpace(tipologia))
            {
                return Json(new { CiSonoAnimali = false, messaggio = "Tipologia non specificata." }, JsonRequestBehavior.AllowGet);
            }

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