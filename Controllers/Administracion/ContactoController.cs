using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Administracion
{
    public class ContactoController : Controller
    {
        // GET: Chofer

        ContactoContext db = new ContactoContext();
        public ActionResult Index()
        {
            var listadechoferes = db.Contacto.ToList();
            return View(listadechoferes);
        }

        [HttpPost]
        public ActionResult Create(string input1, string input2, string input3, string input4)
        {
            db.Database.ExecuteSqlCommand("insert into Contacto([Nombre],[Email],[Telefono],[Mensaje]) valueS ('" + input1 + "','" + input2 + "','" + input3 + "','" + input4 + "')");
            return RedirectToAction("Contact", "Home");

        }



        ////////////////  Contacto Cliente
        [Authorize(Roles = "Cliente")]
        public ActionResult ContactoC()
        {
            ViewBag.showSuccessAlert = false;
            int p = 0;

            SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDCliente] as Dato from [dbo].[Clientes] where [RFC]='" + User.Identity.Name + "'").ToList()[0];
            p = c.Dato;
            Clientes cliente = new ClientesContext().Clientes.Find(p);
            ViewBag.cliente = cliente;
            
            return View();
        }

        [HttpPost]
        public ActionResult ContactoC(Contacto elemento)
        {
            int p = 0;

                SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select  [IDCliente] as Dato from [dbo].[Clientes] where [RFC]='" + User.Identity.Name + "'").ToList()[0];
                p = c.Dato;
            Clientes cliente = new ClientesContext().Clientes.Find(p);
         
            try
            {
                // TODO: Add insert logic here
                string sqlText = "insert into Contacto([Departamento],[Fecha],[RFC],[TipoUsuario],[Nombre],[Email],[Telefono],[Mensaje]) values ('" + elemento.Departamento + "',GetDate(),'"+ cliente.RFC + "','C','" + elemento.Nombre + "','" + elemento.Email + "','" + elemento.Telefono + "','" + elemento.Mensaje + "')";
                db.Database.ExecuteSqlCommand(sqlText);
                ViewBag.showSuccessAlert = true;
                return View();
            }
            catch
            {
               
                ViewBag.showSuccessAlert = false;
                return View();
            }
        }



        ///////////// Contacto Proveedor
        [Authorize(Roles = "Proveedor")]
        public ActionResult ContactoP()
        {
            ViewBag.showSuccessAlert = false;
            int p = -1;

            SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select[IDProveedor] as Dato from[dbo].[ContactosProv] where[Email] = '" + User.Identity.Name + "'").ToList()[0];
            p = c.Dato;
            Proveedor proveedor = new ProveedorContext().Proveedores.Find(p);
            Session["Proveedor"] = p;

            return View();
        }

        [HttpPost]
        public ActionResult ContactoP(Contacto elemento)
        {
            int p = -1;

            SIAAPI.Models.Comercial.ClsDatoEntero c = db.Database.SqlQuery<SIAAPI.Models.Comercial.ClsDatoEntero>("select[IDProveedor] as Dato from[dbo].[ContactosProv] where[Email] = '" + User.Identity.Name + "'").ToList()[0];
            p = c.Dato;
            Proveedor proveedor = new ProveedorContext().Proveedores.Find(p);
            Session["Proveedor"] = p;
            try
            {
                // TODO: Add insert logic here
                string sqlText = "insert into Contacto([Departamento],[Fecha],[RFC],[TipoUsuario],[Nombre],[Email],[Telefono],[Mensaje]) values ('" + elemento.Departamento + "',GetDate(),'"+ proveedor.RFC +"','P','" + elemento.Nombre + "','" + elemento.Email + "','" + elemento.Telefono + "','" + elemento.Mensaje + "')";
                db.Database.ExecuteSqlCommand(sqlText);
                ViewBag.showSuccessAlert = true;
                return View();
            }
            catch
            {
                ViewBag.showSuccessAlert = false;
                return View();
            }
        }

        [Authorize(Roles = "Administrador,Facturacion,Sistemas,Gerencia,GerenteVentas")]
        public ActionResult VMensajes()
        {
            VMensajesContext db =new  VMensajesContext();
            var lista = from e in db.VMensajes
                        orderby e.IDContacto descending
                        select e;
            return View(lista);
        }
        [Authorize(Roles = "Administrador,Facturacion,Sistemas,Gerencia,GerenteVentas")]
        public ActionResult Atencion(int id)
        {
            VMensajesContext db = new VMensajesContext();
            var lista = from e in db.VMensajes
                        orderby e.IDContacto descending
                        select e;
            var elemento = db.VMensajes.Single(m => m.IDContacto == id);
            ViewBag.elemento = elemento;
            ContactoContext dbc = new ContactoContext();
            Contacto elemento2= dbc.Contacto.Find(id);
            //var elemento2 = dbc.Contacto.Single(m => m.IDContacto == id);
            ViewBag.elemento2 = elemento2;
            return View(elemento);
        }
        [HttpPost]
        public ActionResult Atencion( FormCollection collection)
        {
            int idcontacto = int.Parse(collection["IDContacto"].ToString());
            string por = collection["Por"].ToString();
            string observacion = collection["Observacion"].ToString();
            // TODO: Add update logic here
            string cadena = "update contacto set FechaAtencion=GetDate(), Por= '" + por + "' , Observacion = '" + observacion + "', Atendido= 1 where IDContacto =" + idcontacto + "";
            new ContactoContext().Database.ExecuteSqlCommand(cadena);
            return RedirectToAction("VMensajes");

        }
        public ActionResult Details(int id)
        {
            VMensajesContext db = new VMensajesContext();
            var lista = from e in db.VMensajes
                        orderby e.IDContacto descending
                        select e;
            var elemento = db.VMensajes.Single(m => m.IDContacto == id);
            ViewBag.elemento = elemento;
            ContactoContext dbc = new ContactoContext();
            var elemento2 = dbc.Contacto.Single(m => m.IDContacto == id);
            ViewBag.elemento2 = elemento2;
            return View(elemento);
        }

        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, Contacto collection)
        {
            VMensajesContext db = new VMensajesContext();
            var lista = from e in db.VMensajes
                        orderby e.IDContacto descending
                        select e;
            var elemento = db.VMensajes.Single(m => m.IDContacto == id);
            ViewBag.elemento = elemento;
            ContactoContext dbc = new ContactoContext();
            var elemento2 = dbc.Contacto.Single(m => m.IDContacto == id);
            ViewBag.elemento2 = elemento2;
            return RedirectToAction("VMensajes");
        }
    }
}