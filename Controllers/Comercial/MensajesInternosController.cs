using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SIAAPI.Controllers.Comercial
{
    public class MensajesInternosController : Controller
    {
        // GET: MensajesInternos
        public ActionResult Index()
        {
            List<MensajesInternos> Elementos = null;
            MIdbContext db = new MIdbContext();

            if (@Roles.IsUserInRole(User.Identity.Name, "Administrador"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Administrador").ToList(); }

            if (@Roles.IsUserInRole(User.Identity.Name, "Almacenista"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Almacenista").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Cliente"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Cliente").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Sistemas"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Sistemas").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Comercial"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Comercial").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Ventas"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Ventas").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Compras"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Compras").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Produccion"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Produccion").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "AdminProduccion"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "AdminProduccion").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "BitacoraProduccion"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "BitacoraProduccion").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Facturacion"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Facturacion").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Gerencia"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Gerencia").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Proveedor"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Proveedor").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Calidad"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Calidad").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "GerenteVentas"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "GerenteVentas").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Diseno"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Diseno").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Logistica"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Logistica").ToList();

            }
            if (@Roles.IsUserInRole(User.Identity.Name, "Contabilidad"))
            {
                Elementos = db.MenPros.Where(s => s.Estado == false && s.Rol == "Contabilidad").ToList();

            }

            return View(Elementos);
        }

        public ActionResult DarPorLeido (int id) {

            string cadenasql = "update MensajesInternos set Estado='1' where id_mensaje=" + id;
              new MIdbContext().Database.ExecuteSqlCommand(cadenasql);
            return Json(new HttpStatusCodeResult(200));
        }
    }
}