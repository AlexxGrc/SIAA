using Org.BouncyCastle.Ocsp;
using PagedList;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.Models.Soporte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SIAAPI.Controllers.Soporte
{
    public class SoporteController : Controller
    {
        // GET: Soporte
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string Status, string StatusCliente)
        {
            string ConsultaSql = "Select * from MensajesSoporte";
            string Filtro = string.Empty;
            string Orden = "order by id_Ticket desc";
            string cadenaSQl = string.Empty;

            //List<MensajesSoporte> Elesoporte = null;
            MSdbContext dbs = new MSdbContext();

            if (Status == null)
            {
                Status = "";
            }
            ViewBag.Status = new List<SelectListItem>()
                 {
                 new SelectListItem { Text = "Abierto", Value = "Abierto" },
                new SelectListItem { Text = "Cerrado", Value = "Cerrado" },

                new SelectListItem { Text = "Todos", Value = "Todos" , Selected = true}

                  };

            ViewBag.Statusseleccionado = Status;

            if (StatusCliente == null)
            {
                StatusCliente = "";
            }
            ViewBag.StatusCliente = new List<SelectListItem>()
                 {
                 new SelectListItem { Text = "Abierto", Value = "Abierto" },
                new SelectListItem { Text = "Cerrado", Value = "Cerrado" },

                new SelectListItem { Text = "Todos", Value = "Todos" , Selected = true}

                  };

            ViewBag.StatusClienteseleccionado = StatusCliente;

            if (searchString == null)
            {
                searchString = currentFilter;
            }



            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;

            //Paginación
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;


            //Busqueda

            ///tabla filtro: serie
            if (!String.IsNullOrEmpty(searchString))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where Mensaje like '% " + searchString + " %'";
                }
                else
                {
                    Filtro += "and  Mensaje like '% " + searchString + " %'";
                }

            }
            ///tabla filtro: status
            if (Status != "Todas")
            {
                if (Status == "Abierto")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where EstadoTicket = 'true'";
                    }
                    else
                    {
                        Filtro += " and EstadoTicket = 'true";
                    }
                }
                if (Status == "Cerrado")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where EstadoTicket = 'false'";
                    }
                    else
                    {
                        Filtro += " and EstadoTicket = 'false'";
                    }
                }
            }
            ///tabla filtro: CerradoPorCliente
            if (StatusCliente != "Todas")
            {
                if (StatusCliente == "Abierto")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where CerradoPorCliente = 'true'";
                    }
                    else
                    {
                        Filtro += " and CerradoPorCliente = 'true'";
                    }
                }
                if (StatusCliente == "Cerrado")
                {
                    if (Filtro == string.Empty)
                    {
                        Filtro = "where CerradoPorCliente = 'false'";
                    }
                    else
                    {
                        Filtro += " and CerradoPorCliente = 'false'";
                    }
                }
            }

            //if (!string.IsNullOrEmpty(searchString))
            //{
            //    if (Status != "")
            //    {
            //        if (Status == "Abierto")
            //        {
            //            estado = true;
            //        }
            //        elementos = elementos.Where(s => s.EstadoTicket == estado && (s.Id_Ticket.ToString().Contains(searchString) || s.Mensaje.Contains(searchString)));


            //    }
            //    else
            //    {
            //        elementos = elementos.Where(s => s.Id_Ticket.ToString().Contains(searchString) || s.Mensaje.Contains(searchString));

            //    }
            //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));




            //Ordenacion

            switch (sortOrder)
            {
                case "Id_Ticket":
                    //elementos = elementos.OrderByDescending(s => s.Id_Ticket);
                    Orden = "order by id_Ticket asc";
                    break;

                default:
                    //elementos = elementos.OrderByDescending(s => s.Id_Ticket);
                    Orden = "order by id_Ticket desc";
                    break;
            }

            cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;
             var elementos = dbs.Database.SqlQuery<MensajesSoporte>(cadenaSQl).ToList();
    //Paginación
    // DROPDOWNLIST FOR UPDATING PAGE SIZE
    //int count = dbs.MenSoportes.OrderByDescending(e => e.Id_Ticket).Count(); // Total number of elements
            int count = elementos.Count();// Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;

            //return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.ToPagedList(pageNumber, pageSize));

        }


        public ActionResult DarSoporte(int id)
        {

            string Scadenasql = "update MensajesSoporte set Estado='1' where Id_ticket=" + id;
            new MSdbContext().Database.ExecuteSqlCommand(Scadenasql);
            return Json(new HttpStatusCodeResult(200));
        }
        public ActionResult CreateMensaje()
        {
            //MSdbContext dbs = new MSdbContext();

            //var lista = from e in dbs.MenSoportes
            //            orderby e.Id_Ticket
            //            select e;
            //return View(lista);
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMensaje(MensajesSoporte elemento)
        {

            UserContext db = new UserContext();
            RolesContext dbr = new RolesContext();
            UserRoleContext dbu = new UserRoleContext();
            ProveedorContext Db = new ProveedorContext();

            List<SIAAPI.Models.Login.User> userid = db.Database.SqlQuery<SIAAPI.Models.Login.User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            List<UserRole> userrol = db.Database.SqlQuery<UserRole>("select * from [dbo].[UserRole] where userid='" + usuario + "'").ToList();
            int roleid = userrol.Select(r => r.RoleID).FirstOrDefault();
            ClsDatoString rol = Db.Database.SqlQuery<ClsDatoString>("Select ROleName as Dato from[dbo].[Roles] where RoleID = " + roleid + "").ToList()[0];
            //string userrol = use         
            //clcdatostring

            string cadenasql = "Insert into MensajesSoporte([Fecha_Hora],[Mensaje], [Rol],[UserID] ,[EstadoTicket],CerradoPorCliente, ErrorGeneradoPor) values ( GetDate(), '" + elemento.Mensaje + "','" + rol.Dato + "' , " + usuario +", 'true','false','X')";
            new MSdbContext().Database.ExecuteSqlCommand(cadenasql);
            return RedirectToAction("Index");
        }

        public ActionResult EditMensaje(int id)
        {
            MSdbContext dbs = new MSdbContext();
            var elemento = dbs.MenSoportes.Single(m => m.Id_Ticket == id);

            var ErrorLst = new List<SelectListItem>();
            ErrorLst.Add(new SelectListItem { Text = "Aun sin definir", Value = "X" });
            ErrorLst.Add(new SelectListItem { Text = "Datos", Value = "D" });
            ErrorLst.Add(new SelectListItem { Text = "Gestor de la BD", Value = "BD" });
            ErrorLst.Add(new SelectListItem { Text = "Servicio Internet", Value = "I" });
            ErrorLst.Add(new SelectListItem { Text = "Servidor del Sistema", Value = "SS" });
            ErrorLst.Add(new SelectListItem { Text = "Sistema", Value = "SI" });
            ErrorLst.Add(new SelectListItem { Text = "Usuario", Value = "U" });
            ErrorLst.Add(new SelectListItem { Text = "Falla de Luz", Value = "L" });
            ErrorLst.Add(new SelectListItem { Text = "Otro", Value = "O" });
            ViewBag.ErrorGeneradoPor = new SelectList(ErrorLst, "Value", "Text");
            return View(elemento);
        }

        [HttpPost]
        public ActionResult EditMensaje(int id, FormCollection collection)
        {
            MSdbContext dbs = new MSdbContext();
            try
            {
                // TODO: Add update logic here
                var elemento = dbs.MenSoportes.Single(m => m.Id_Ticket == id);
                if (TryUpdateModel(elemento))
                {
                    dbs.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(elemento);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Respuesta(int id)
        {
            MRdbContext mRdb = new MRdbContext();
            RespuestaSoporte elemento = new RespuestaSoporte();
            elemento.Id_Ticket = id;
            //ClsDatoString mess = mRdb.Database.SqlQuery<ClsDatoString>("Select Mensaje as Dato from[dbo].[MensajesSoporte] where Id_Ticket = " + id + "").ToList().FirstOrDefault();
            //ViewBag.Id_Ticket = mess.Dato;
            MensajesSoporte mensajes = new MSdbContext().MenSoportes.Find(id);
            ViewBag.Id_Ticket = mensajes.Id_Ticket;
            ViewBag.Mensaje = mensajes.Mensaje;
            var PrioridadLst = new List<SelectListItem>();
            PrioridadLst.Add(new SelectListItem { Text = "Alta", Value = "A" });
            PrioridadLst.Add(new SelectListItem { Text = "Media", Value = "M" });
            PrioridadLst.Add(new SelectListItem { Text = "Baja", Value = "B" });
            ViewBag.Prioridad = new SelectList(PrioridadLst, "Value", "Text");

            return View(elemento);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Respuesta(int id, RespuestaSoporte elemento)
        {
            UserContext db = new UserContext();
            RolesContext dbr = new RolesContext();
            UserRoleContext dbu = new UserRoleContext();

            List<SIAAPI.Models.Login.User> userid = db.Database.SqlQuery<SIAAPI.Models.Login.User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            string nombre = User.Identity.Name;
            List<UserRole> userrol = db.Database.SqlQuery<UserRole>("select * from [dbo].[UserRole] where userid='" + usuario + "'").ToList();
            int roleid = userrol.Select(r => r.RoleID).FirstOrDefault();
            ClsDatoString rol = dbr.Database.SqlQuery<ClsDatoString>("Select ROleName as Dato from[dbo].[Roles] where RoleID = " + roleid + "").ToList()[0];

            if (elemento.EstadoTicket)
            {
                ///se activo para cerrarlo, entonces cambiamos a 1 DE TICKET CERRADO
                ///
                elemento.EstadoTicket = false;
            }
            else
            {
                // si el check no esta activo entonces sigue abierto el ticket

                elemento.EstadoTicket = true;
            }

            if (elemento.CerradoPorCliente)
            {
                ///se activo para cerrarlo, entonces cambiamos a 1 DE TICKET CERRADO
                ///
                elemento.CerradoPorCliente = false;
            }
            else
            {
                // si el check no esta activo entonces sigue abierto el ticket

                elemento.CerradoPorCliente = true;
            }

            MRdbContext mRdb = new MRdbContext();
            ViewBag.Id_Ticket = id;

            var PrioridadLst = new List<SelectListItem>();

            PrioridadLst.Add(new SelectListItem { Text = "Alta", Value = "A" });
            PrioridadLst.Add(new SelectListItem { Text = "Media", Value = "M" });
            PrioridadLst.Add(new SelectListItem { Text = "Baja", Value = "B" });
            PrioridadLst.Add(new SelectListItem { Text = "Puede Esperar", Value = "P" });
            PrioridadLst.Add(new SelectListItem { Text = "Urgente", Value = "U" });
            ViewBag.Prioridad = new SelectList(PrioridadLst, "Value", "Text");

            //string cadenasql1 = "Insert into RespuestaSoporte([Id_Ticket]),[Fecha_Atencion],[Respuesta], [Estado],[Prioridad]  values (" + id + " , GetDate() ,' " + elemento.Respuesta + " ', 0 ,' " + ViewBag.Prioridad  + " ')";
            string cadenasql1 = "Insert into RespuestaSoporte([Id_Ticket],[Fecha_Atencion],[Respuesta], [EstadoTicket],[Prioridad],[Rol], [UserN],[CerradoPorCliente])  values (" + id + " , GetDate() ,'" + elemento.Respuesta + "', '" + elemento.EstadoTicket + "','" + elemento.Prioridad + "','" + rol.Dato + "' , '" + nombre + "', '"+ elemento.CerradoPorCliente+ "')";
            string cadenasql2 = "update MensajesSoporte set EstadoTicket = '" + elemento.EstadoTicket + "', CerradoPorCliente= '" + elemento.CerradoPorCliente + "' where [Id_Ticket] = " + id + " ";
            new MRdbContext().Database.ExecuteSqlCommand(cadenasql1);
            new MRdbContext().Database.ExecuteSqlCommand(cadenasql2);
            return RedirectToAction("Details", new { id = id });
        }
        public ActionResult Delete(int id)
        {
            //(m => m.Id_Ticket == id)
            var elemento = new MSdbContext().MenSoportes.Find(id); 
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }

        // POST: Almacen/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
            
                new MSdbContext().Database.ExecuteSqlCommand("delete from RespuestaSoporte where Id_Ticket = " +id+ " ");
            
                var elemento = new MSdbContext().MenSoportes.Single(m => m.Id_Ticket == id);
                new MSdbContext().MenSoportes.Remove(elemento);
                new MSdbContext().SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

        public ActionResult Details(int id)
        {

            //UserContext db = new UserContext();

            var lista = new MSdbContext().MenSoportes.Find(id);
           
            return View(lista);
        }

    }
}