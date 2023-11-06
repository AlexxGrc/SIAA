using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Inventarios;
using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using System.Security.Cryptography;
using System.Text;

namespace SIAAPI.Controllers.Comercial
{
    public class AlmacenistaController : Controller
    {
        
        private AlmacenistaContext db = new AlmacenistaContext();
       

        [Authorize(Roles = "Administrador,Almacen,Sistemas,Gerencia")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            string ConsultaSql = "select * from Almacenista";
            string cadenaSQl = string.Empty;
            string Orden = "order by Nombre";
            string Filtro = string.Empty;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.AlmacenistaSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "";
            // ViewBag.AlmSortParm = String.IsNullOrEmpty(sortOrder) ? "Almacen" : "Almacen";

            //Buscar 
            if (!String.IsNullOrEmpty(searchString))
            {
                if (Filtro == string.Empty)
                {
                    Filtro = "where Nombre like '%" + searchString + "%' or RFC like '%" + searchString + "%'";
                }
                else
                {
                    Filtro += " and Nombre like '%" + searchString + "%' or RFC like '%" + searchString + "%'";
                }

            }
            //ordenar
            switch (sortOrder)
            {
                case "Nombre":
                    Orden = " order by  Nombre desc ";
                    break;
               
                default:
                    Orden = " order by Nombre ";
                    break;
            }

            ViewBag.CurrentFilter = searchString;
            //Paginación
            cadenaSQl = ConsultaSql + " " + Filtro + " " + Orden;
            var elementos = db.Database.SqlQuery<Almacenista>(cadenaSQl).ToList();

            //Paginación
            int count = db.Almacenistas.OrderBy(e => e.IDA).Count(); // Total number of elements

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
            int pageSize = (PageSize ?? 4);
            ViewBag.psize = pageSize;


            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        // GET: FamAlm/Details/5
        public ActionResult Details(int? id)
        {
            var elemento = db.Database.SqlQuery<Almacenista>("select * from Almacenista where  IDA= " + id + "").ToList().FirstOrDefault();

            if (elemento == null)
            {
                return RedirectToAction("Index");
            }
            return View(elemento);
        }

        // GET: FamAlm/Create
        public ActionResult Create()
        {
            c_TipoContratoContext dbf = new c_TipoContratoContext();
            c_TipoJornadaContext dba = new c_TipoJornadaContext();
            c_PeriodicidadPagoContext dbp = new c_PeriodicidadPagoContext();
            OficinaContext of = new OficinaContext();
            ViewBag.IDTipoContrato = new SelectList(dbf.c_TipoContratos, "IDTipoContrato", "Descripcion");
            ViewBag.IDTipoJornada = new SelectList(dba.c_TipoJornadas, "IDTipoJornada", "Descripcion");
            ViewBag.IDPeriocidadPago = new SelectList(dbp.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion");
            ViewBag.IDOficina = new SelectList(of.Oficinas, "IDOficina", "NombreOficina");
            return View();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Almacenista almacen)
        {
            HttpPostedFileBase archivo = Request.Files["Image1"];
            if (archivo.FileName != "")
            {
                almacen.Photo = new byte[archivo.ContentLength];
                archivo.InputStream.Read(almacen.Photo, 0, archivo.ContentLength);
            }

            string fecha = DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year;
            string pass = MD5P(almacen.ClaveAcceso);
            almacen.ClaveAcceso = pass;
            almacen.FechaIngreso = DateTime.Now;
            try
            {
                
                string cadena = "Insert into Almacenista(RFC, Nombre,Mail,Telefono,FechaIngreso,IDTipoContrato,IDTipoJornada,IDPeriocidadPago,IDOficina,ClaveAcceso,Activo,Notas,Photo) Values" +
                                             "('" + almacen.RFC + "', '" + almacen.Nombre + "','"+ almacen.Mail +"','"+almacen.Telefono+ "',SYSDATETIMEOFFSET(),"+almacen.IDTipoContrato+","+almacen.IDTipoJornada+"," +almacen.IDPeriocidadPago+","+ almacen.IDOficina+ ",'"+ pass+"','"+ almacen.Activo+"','"+ almacen.Notas+"',"+ almacen.Photo+ ")";
                //db.Database.ExecuteSqlCommand(cadena);
                db.Almacenistas.Add(almacen);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                c_TipoContratoContext dbf = new c_TipoContratoContext();
                c_TipoJornadaContext dba = new c_TipoJornadaContext();
                c_PeriodicidadPagoContext dbp = new c_PeriodicidadPagoContext();
                OficinaContext of = new OficinaContext();
                ViewBag.IDTipoContrato = new SelectList(dbf.c_TipoContratos, "IDTipoContrato", "Descripcion");
                ViewBag.IDTipoJornada = new SelectList(dba.c_TipoJornadas, "IDTipoJornada", "Descripcion");
                ViewBag.IDPeriocidadPago = new SelectList(dbp.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion");
                ViewBag.IDOficina = new SelectList(of.Oficinas, "IDOficina", "NombreOficina");
                return View();
            }


        }
        private string MD5P(string password)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(password));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        // GET: FamAlm/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var elementos = db.Database.SqlQuery<Almacenista>("select * from Almacenista where IDA= " + id + "").ToList()[0];
           c_TipoContratoContext dbf = new c_TipoContratoContext();
            c_TipoJornadaContext dba = new c_TipoJornadaContext();
            c_PeriodicidadPagoContext dbp = new c_PeriodicidadPagoContext();
            OficinaContext of = new OficinaContext();
            ViewBag.IDTipoContrato = new SelectList(dbf.c_TipoContratos, "IDTipoContrato", "Descripcion", elementos.IDTipoContrato);
            ViewBag.IDTipoJornada = new SelectList(dba.c_TipoJornadas, "IDTipoJornada", "Descripcion", elementos.IDTipoJornada);
            ViewBag.IDPeriocidadPago = new SelectList(dbp.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion", elementos.IDPeriocidadPago);
            ViewBag.IDOficina = new SelectList(of.Oficinas, "IDOficina", "NombreOficina",elementos.IDOficina);
            return View(elementos);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Almacenista alm, int id)
        {
            try
            {
                var elemento = db.Almacenistas.Single(m => m.IDA == id);
                HttpPostedFileBase archivo = Request.Files["Image1"];
                if (archivo.FileName != "")
                {
                    elemento.Photo = new byte[archivo.ContentLength];
                    archivo.InputStream.Read(elemento.Photo, 0, archivo.ContentLength);
                }

                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                c_TipoContratoContext dbf = new c_TipoContratoContext();
                c_TipoJornadaContext dba = new c_TipoJornadaContext();
                c_PeriodicidadPagoContext dbp = new c_PeriodicidadPagoContext();
                OficinaContext of = new OficinaContext();
                ViewBag.IDTipoContrato = new SelectList(dbf.c_TipoContratos, "IDTipoContrato", "Descripcion", alm.IDTipoContrato);
                ViewBag.IDTipoJornada = new SelectList(dba.c_TipoJornadas, "IDTipoJornada", "Descripcion", alm.IDTipoJornada);
                ViewBag.IDPeriocidadPago = new SelectList(dbp.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion", alm.IDPeriocidadPago);
                ViewBag.IDOficina = new SelectList(of.Oficinas, "IDOficina", "NombreOficina", alm.IDOficina);
                return View(alm);
            }
            catch (Exception err)
            {
                c_TipoContratoContext dbf = new c_TipoContratoContext();
                c_TipoJornadaContext dba = new c_TipoJornadaContext();
                c_PeriodicidadPagoContext dbp = new c_PeriodicidadPagoContext();
                OficinaContext of = new OficinaContext();
                ViewBag.IDTipoContrato = new SelectList(dbf.c_TipoContratos, "IDTipoContrato", "Descripcion", alm.IDTipoContrato);
                ViewBag.IDTipoJornada = new SelectList(dba.c_TipoJornadas, "IDTipoJornada", "Descripcion", alm.IDTipoJornada);
                ViewBag.IDPeriocidadPago = new SelectList(dbp.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion", alm.IDPeriocidadPago);
                ViewBag.IDOficina = new SelectList(of.Oficinas, "IDOficina", "NombreOficina", alm.IDOficina);
                return View();
            }
         
            

           
        }

        // GET: FamAlm/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var elemento = db.Database.SqlQuery<Almacenista>("select * from Almacenista where IDA= " + id + "").ToList().FirstOrDefault();

            return View(elemento);
        }

        // POST: FamAlm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            string cadena = "Delete from Almacenista where IDA= " + id + "";
            db.Database.ExecuteSqlCommand(cadena);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
