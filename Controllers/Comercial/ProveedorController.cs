
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using PagedList;
using Reportes;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SIAAPI.Reportes;

namespace SIAAPI.Controllers.Comercial
{
    [Authorize(Roles = "Administrador,Gerencia,Sistemas,Comercial,Compras,Facturacion")]

    public class ProveedorController : Controller
    {
       private VProveedoresContext dbv = new VProveedoresContext();
       private ProveedorContext db = new ProveedorContext();
        public ActionResult Report(Proveedor proveedor)
        {
            ProveedorReporte proveedorReport = new ProveedorReporte();
            byte[] abytes = proveedorReport.PrepareReport(GetProveedor());
            return File(abytes, "application/pdf");
        }
        
        public List<Proveedor> GetProveedor() ///idarticulo trae el tipo de articulo
        {
            List<Proveedor> proveedor;
            proveedor = db.Database.SqlQuery<Proveedor>("select* from dbo.Proveedores where Status='Activo'").ToList();
            //proveedor = db.Database.SqlQuery<Proveedor>("select * from Proveedores").ToList();
            return proveedor;

        }

        public List<VProveedores> GetVProveedor() 
        {
            List<VProveedores> proveedor;
            proveedor = dbv.Database.SqlQuery<VProveedores>("select* from dbo.VProveedores").ToList();
            return proveedor;
        }

        // GET: Proveedor
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.EmpresaSortParm = String.IsNullOrEmpty(sortOrder) ? "Empresa" : "Empresa";
            ViewBag.EstadoSortParm = String.IsNullOrEmpty(sortOrder) ? "Estado" : "Estado";
            ViewBag.PaisSortParm = String.IsNullOrEmpty(sortOrder) ? "Pais" : "Pais";
            ViewBag.MunicipioSortParm = String.IsNullOrEmpty(sortOrder) ? "Municipio" : "Municipio";
            ViewBag.ProductoSortParm = String.IsNullOrEmpty(sortOrder) ? "Producto" : "Producto";
            // Not sure here
           

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
           
            var elementos = from s in dbv.VProveedores
                           select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Empresa.Contains(searchString) || s.Estado.Contains(searchString) || s.Municipio.Contains(searchString) || s.Producto.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Municipio":
                    //elementos = elementos.OrderByDescending(s => s.IDProveedor);
                    if (elementos == elementos.OrderBy(s => s.Municipio))
                    { elementos = elementos.OrderByDescending(s => s.Municipio); }

                    else
                    { elementos = elementos.OrderBy(s => s.Municipio); }
                    break;
                case "Empresa":
                    if (elementos == elementos.OrderBy(s => s.Empresa))
                    { elementos = elementos.OrderByDescending(s => s.Empresa); }

                    else
                    { elementos = elementos.OrderBy(s => s.Empresa);}
                    break;
                case "Estado":
                    if (elementos == elementos.OrderBy(s => s.Estado))
                    { elementos = elementos.OrderByDescending(s => s.Estado); }

                    else
                    { elementos = elementos.OrderBy(s => s.Estado); }
                    break;
                case "Pais":
                    if (elementos == elementos.OrderBy(s => s.Pais))
                    { elementos = elementos.OrderByDescending(s => s.Pais); }

                    else
                    { elementos = elementos.OrderBy(s => s.Pais); }
                    
                    break;
                case "Producto":
                    if (elementos == elementos.OrderBy(s => s.Producto))
                    { elementos = elementos.OrderByDescending(s => s.Producto); }

                    else
                    { elementos = elementos.OrderBy(s => s.Producto); }
                    
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Empresa);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbv.VProveedores.Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;
            //return View("Index", "_LayoutCopy", elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }


        public ActionResult getJsonEstadoPorPais(int id)
        {
            var estado = new EstadosRepository().GetEstadoPorPais(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getEstadoPorPais(int idp)
        {
            var estado = new EstadosRepository().GetEstadoPorPais(idp);
            return estado;

        }
        public string convertirfechaamericana(string data)
        {
            DateTime fecha = DateTime.Parse(data);
            string nuevafecha = fecha.Year + "/" + fecha.Month + "/" + fecha.Day;
            return nuevafecha;
        }

        // GET: Proveedor/Details/5
        public ActionResult Details(int? id, int page=1, int pagesize=10, string searchString="")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session["IDProveedor"] = id;
            Proveedor proveedor = db.Proveedores.Find(id);
            //Paises
            ViewBag.IDPais = new EstadosRepository().GetPaisSelec(proveedor.IDPais);
            ViewBag.IDEstado = new EstadosRepository().GetEstadoPorPaisSelec(proveedor.IDPais, proveedor.IDEstado);
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;

            if (proveedor == null)
            {
                return HttpNotFound();
            }
            return View(proveedor);
        }

        // GET: Proveedor/Create
        public ActionResult Create()
        {
            //ViewBag.IDEstado = new SelectList(db.Estados, "IDEstado", "Estado");
            ViewBag.IDFormaPago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion");
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion");
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion");
            ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscal, "IDRegimenFiscal", "Descripcion");
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion");
            ViewBag.IDTipoIVA = new SelectList(db.c_TiposIVA, "IDTipoIVA", "Descripcion");
            var listas = new ElementosRepository().GetStatus();
            ViewBag.ComboStatus = listas;
            var listac = new ElementosRepository().GetConfianza();
            ViewBag.ComboConfianza = listac;

            //Paises
            var datosPaises = db.Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> liP = new List<SelectListItem>();
            liP.Add(new SelectListItem { Text = "--Selecciona un Pais--", Value = "0" });
            foreach (var a in datosPaises)
            {
                liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });

            }
            ViewBag.ListPais = liP;
            ViewBag.ListEstado = getEstadoPorPais(0);

            return View();
        }

        // POST: Proveedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Proveedor proveedor)
        {
            int idpais = proveedor.IDPais;
            int idedo = proveedor.IDEstado;
            int idiva = proveedor.IDTipoIVA;
            if (ModelState.IsValid)
            {
                //db.Proveedores.Add(proveedor);
                //db.SaveChanges();
                string cadena = " insert into dbo.Proveedores(IDRegimenFiscal, Empresa, Calle, NoExt, NoInt, Colonia, Municipio, CP, IDEstado, IDPais, IDMoneda, IDFormaPago, IDMetodoPago, [Status], IDCondicionesPago, Confianza, Servicio, TEntrego, Calidad, Producto, Telefonouno, Telefonodos, RFC, IDTipoIVA) ";
                cadena = cadena + "values (" + proveedor.IDRegimenFiscal + ", '" + proveedor.Empresa + "','" + proveedor.Calle + "', '" + proveedor.NoExt + "', '" + proveedor.NoInt + "', '" + proveedor.Colonia + "','" + proveedor.Municipio + "','" + proveedor.CP + "', " + idedo + ", " + idpais + ",  ";
                cadena = cadena + ""+ proveedor.IDMoneda +", "+ proveedor.IDFormaPago +", "+ proveedor.IDMetodoPago +", '"+ proveedor.Status +"', "+ proveedor.IDCondicionesPago +", '"+ proveedor.Confianza +"', "+ proveedor.Servicio +",  "+proveedor.Tentrego+", "+proveedor.Calidad +", '"+proveedor.Producto +"', '"+ proveedor.Telefonouno + "', '" + proveedor.Telefonodos + "', '"+ proveedor.RFC + "', " + proveedor.IDTipoIVA + ") ";
                db.Database.ExecuteSqlCommand(cadena);
                return RedirectToAction("Index");
            }

            //ViewBag.IDEstado = new SelectList(db.Estados, "IDEstado", "Estado", proveedor.IDEstado);
            ViewBag.IDFormaPago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", proveedor.IDFormaPago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", proveedor.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", proveedor.IDMoneda);
            ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscal, "IDRegimenFiscal", "Descripcion", proveedor.IDRegimenFiscal);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", proveedor.IDCondicionesPago);
            ViewBag.IDTipoIVA = new SelectList(db.c_TiposIVA, "IDTipoIVA", "Descripcion", proveedor.IDTipoIVA);
            var listas = new ElementosRepository().GetStatus();
            ViewBag.ComboStatus = listas;
            var listac = new ElementosRepository().GetConfianza();
            ViewBag.ComboConfianza = listac;
            //Paises
            var datosPaises = db.Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> liP = new List<SelectListItem>();
            liP.Add(new SelectListItem { Text = "--Selecciona un Pais--", Value = "0" });
            foreach (var a in datosPaises)
            {
                liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });

            }
            ViewBag.ListPais = liP;
            ViewBag.ListEstado = getEstadoPorPais(0);

            return View(proveedor);
        }

        // GET: Proveedor/Edit/5
        public ActionResult Edit(int? id, int page = 1, int pagesize = 10, string searchString = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedor proveedor = db.Proveedores.Find(id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }
            //ViewBag.IDEstado = new SelectList(db.Estados, "IDEstado", "Estado", proveedor.IDEstado);
            ViewBag.IDFormaPago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", proveedor.IDFormaPago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", proveedor.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", proveedor.IDMoneda);
            ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscal, "IDRegimenFiscal", "Descripcion", proveedor.IDRegimenFiscal);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", proveedor.IDCondicionesPago);
            ViewBag.IDTipoIVA = new SelectList(db.c_TiposIVA, "IDTipoIVA", "Descripcion", proveedor.IDTipoIVA);
            var listas = new ElementosRepository().GetStatus();
            ViewBag.ComboStatus = listas;
            var listac = new ElementosRepository().GetConfianza();
            ViewBag.ComboConfianza = listac;

            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;

            //Paises
            ViewBag.IDPais = new EstadosRepository().GetPaisSelec(proveedor.IDPais);
            ViewBag.IDEstado = new EstadosRepository().GetEstadoPorPaisSelec(proveedor.IDPais, proveedor.IDEstado);
            return View(proveedor);
        }

        // POST: Proveedor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Proveedor proveedor, int page = 1, int pagesize = 10, string searchString = "")
        {
            int idpais = proveedor.IDPais;
            int idedo = proveedor.IDEstado;
            int idiva = proveedor.IDTipoIVA;
            if (ModelState.IsValid)
            {
                //db.Entry(proveedor).State = System.Data.Entity.EntityState.Modified;
                //db.SaveChanges();
                string cadena = " update dbo.Proveedores set  IDRegimenFiscal = " + proveedor.IDRegimenFiscal + ", Empresa = '" + proveedor.Empresa + "', Calle = '" + proveedor.Calle + "', NoExt = '" + proveedor.NoExt + "', NoInt = '" + proveedor.NoInt + "', Colonia = '" + proveedor.Colonia + "', Municipio = '" + proveedor.Municipio + "', CP = '" + proveedor.CP + "' , IDEstado =" + idedo + ", IDPais =" + idpais + ", IDMoneda = " + proveedor.IDMoneda + "";
                cadena = cadena + ", IDFormaPago = " + proveedor.IDFormaPago + ", IDMetodoPago = " + proveedor.IDMetodoPago + ", Status = '" + proveedor.Status + "', IDCondicionesPago = " + proveedor.IDCondicionesPago + ", Confianza ='" + proveedor.Confianza + "', Servicio = " + proveedor.Servicio + ", TEntrego = " + proveedor.Tentrego + ", Calidad =" + proveedor.Calidad + ", Producto = '" + proveedor.Producto + "', Telefonouno = '" + proveedor.Telefonouno + "', Telefonodos ='" + proveedor.Telefonodos + "', RFC = '" + proveedor.RFC + "', IDTipoIVA = " + proveedor.IDTipoIVA + "";
                cadena = cadena + "where IDProveedor = "+ proveedor.IDProveedor +"";
                db.Database.ExecuteSqlCommand(cadena);
                return RedirectToAction("Index", new { page = page, PageSize = pagesize, searchString = searchString });
            }
          
            ViewBag.IDFormaPago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", proveedor.IDFormaPago);
            ViewBag.IDMetodoPago = new SelectList(db.c_MetodoPagos, "IDMetodoPago", "Descripcion", proveedor.IDMetodoPago);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", proveedor.IDMoneda);
            ViewBag.IDRegimenFiscal = new SelectList(db.c_RegimenFiscal, "IDRegimenFiscal", "Descripcion", proveedor.IDRegimenFiscal);
            ViewBag.IDCondicionesPago = new SelectList(db.CondicionesPagos, "IDCondicionesPago", "Descripcion", proveedor.IDCondicionesPago);
            ViewBag.IDTipoIVA = new SelectList(db.c_TiposIVA, "IDTipoIVA", "Descripcion", proveedor.IDTipoIVA);
            var listas = new ElementosRepository().GetStatus();
            ViewBag.ComboStatus = listas;
            var listac = new ElementosRepository().GetConfianza();
            ViewBag.ComboConfianza = listac;
            //Paises
            ViewBag.IDPais = new EstadosRepository().GetPaisSelec(proveedor.IDPais);
            ViewBag.IDEstado = new EstadosRepository().GetEstadoPorPaisSelec(proveedor.IDPais, proveedor.IDEstado);
            return View(proveedor);
        }

        // GET: Proveedor/Delete/5
        public ActionResult Delete(int? id, int page = 1, int pagesize = 10, string searchString = "")
        {
            ViewBag.Mensaje = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proveedor proveedor = db.Proveedores.Find(id);
            if (proveedor == null)
            {
                return HttpNotFound();
            }
            return View(proveedor);
        }

        // POST: Proveedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Proveedor proveedor = db.Proveedores.Find(id);
                db.Proveedores.Remove(proveedor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception err)
            {
                ViewBag.Mensaje = "Hay datos que utilizan a este proveedor";
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /////////////////////////////////////////////////////////////////////////////
        public ActionResult VerContactos(int id, int page = 1, int pagesize = 10, string searchString = "")
        {
           Session["IDProveedor"] = id;

            ContactosProvContext db = new ContactosProvContext();
            var lista = from e in db.ContactosProvs
                        where e.IDProveedor == id
                        orderby e.IDContactoProv
                        select e;
            //return View(elemento);
            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;

            return View(lista);
        }

        public ActionResult CreateContacto(string searchString = "")
        {
            ViewBag.searchString = searchString;
            return View();
        }

        [HttpPost]
        public ActionResult CreateContacto(ContactosProv elemento, string searchString="")
        {
            ViewBag.searchString = searchString;
            try
            {
                ContactosProvContext db = new ContactosProvContext();
                Int32 idproveedor = Int32.Parse(System.Web.HttpContext.Current.Session["IDProveedor"].ToString());
                elemento.IDProveedor = idproveedor;
                db.ContactosProvs.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("details", new { id = elemento.IDProveedor, searchString= searchString });
            }
            catch (Exception err)
            {
                return View();
            }
        }

        public ActionResult EditContacto(int id, string searchString="")
        {
            ViewBag.searchString = searchString;
            ContactosProvContext db = new ContactosProvContext();

            var elemento = db.ContactosProvs.Single(m => m.IDContactoProv == id);

            return View(elemento);


        }

        // POST: ModeloProceso/Edit/5
        [HttpPost]
        public ActionResult EditContacto(ContactosProv Elemento, string searchString="")
        {
            ViewBag.searchString = searchString;
            try
            {
                Int32 idproveedor = Int32.Parse(Session["IDProveedor"].ToString());
                ContactosProvContext db = new ContactosProvContext();
                var elemento = db.ContactosProvs.Single(m => m.IDContactoProv == Elemento.IDContactoProv);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("details", new { id = idproveedor, searchString= searchString });
                }
                return View(Elemento);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult DeleteContacto(int id, string searchString = "")
        {
            ViewBag.searchString = searchString;
            ContactosProvContext db = new ContactosProvContext();

            var elemento = db.ContactosProvs.Single(m => m.IDContactoProv == id);

            return View(elemento);


        }

        [HttpPost]
        public ActionResult DeleteContacto(int id, ContactosProv collection,  string searchString = "")
        {
            ViewBag.searchString = searchString;
            try
            {
                // TODO: Add delete logic here
                var db = new ContactosProvContext();
                var elemento = db.ContactosProvs.Single(m => m.IDContactoProv == id);
                db.ContactosProvs.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("details", new { id = elemento.IDProveedor, searchString= searchString });

            }
            catch
            {
                return View();
            }
        }

        // GET: Proveedor/Details/5
        public ActionResult DetailsContacto(int id,  string  searchString= "")
        {
            ViewBag.searchString = searchString;
            ContactosProvContext db = new ContactosProvContext();
            var elemento = db.ContactosProvs.Single(m => m.IDContactoProv == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }


        // POST: Proveedor/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DetailsContacto(int id, ContactosProv collection, string searchString="")
        {
            ViewBag.searchString = searchString;
            ContactosProvContext db = new ContactosProvContext();
            var elemento = db.ContactosProvs.Single(m => m.IDContactoProv == id);
            return View(elemento);
        }
        /////////////////////////////////////////////////////////////////////////////////7
       

        // GET: BancosProv
        public ActionResult VerBancos(int id, int page = 1, int pagesize = 10, string searchString = "")
        {
            BancosProvContext db = new BancosProvContext();
           Session["IDProveedor"] = id;
            
            var lista = from e in db.BancosProvs.Include(b => b.Banco).Include(b => b.Moneda).Include(b => b.Proveedor)
            where e.IDProveedor == id
                        orderby e.IDBancosProv
                        select e;

            ViewBag.page = page;
            ViewBag.pagesize = pagesize;
            ViewBag.searchString = searchString;

            return View(lista);

        }

        // GET: BancosProv/Details/5
        public ActionResult DetailsBanco(int? id, string searchString="")
        {
            ViewBag.searchString = searchString;
            BancosProvContext db = new BancosProvContext();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BancosProv bancosProv = db.BancosProvs.Find(id);
            if (bancosProv == null)
            {
                return HttpNotFound();
            }
            return View(bancosProv);
        }

        // GET: BancosProv/Create
        public ActionResult CreateBanco(string searchString ="" )
        {
            ViewBag.searchString = searchString;
            BancosProvContext db = new BancosProvContext();
            //ViewBag.IDBanco = new SelectList(db.c_Banco, "IDBanco", "Nombre");
            var BcoLst = new List<string>();
            var BcoQry = from d in db.c_Banco
                         orderby d.Nombre
                         select d.Nombre;
            BcoLst.AddRange(BcoQry.Distinct());
            ViewBag.IDBanco = new SelectList(BcoLst);
            ViewBag.IDMoneda = new SelectList(db.c_Moneda, "IDMoneda", "Descripcion");
            ViewBag.searchString = searchString;
            return View();
        }

        // POST: BancosProv/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateBanco(BancosProv bancosProv, string searchString= "" )
        {
            BancosProvContext db = new BancosProvContext();
            Int32 idproveedor = Int32.Parse(Session["IDProveedor"].ToString());
            ViewBag.searchString = searchString;
            bancosProv.IDProveedor = idproveedor;
            try
            {
                db.BancosProvs.Add(bancosProv);
                db.SaveChanges();
                return RedirectToAction("details", new { id = bancosProv.IDProveedor, searchString= searchString });
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
                ViewBag.IDBanco = new SelectList(db.c_Banco, "IDBanco", "Nombre", bancosProv.IDBanco);
                ViewBag.IDMoneda = new SelectList(db.c_Moneda, "IDMoneda", "Descripcion", bancosProv.IDMoneda);
                ViewBag.searchString = searchString;
                return View();
            }
        }

        // GET: BancosProv/Edit/5
        public ActionResult EditBanco(int? id, string searchString = "")
        {
            ViewBag.searchString = searchString;
            BancosProvContext db = new BancosProvContext();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BancosProv bancosProv = db.BancosProvs.Find(id);
            if (bancosProv == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDBanco = new SelectList(db.c_Banco, "IDBanco", "Nombre", bancosProv.IDBanco);
            ViewBag.IDMoneda = new SelectList(db.c_Moneda, "IDMoneda", "Descripcion", bancosProv.IDMoneda);
            ViewBag.searchString = searchString;
            return View(bancosProv);
        }

        // POST: BancosProv/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditBanco(BancosProv bancosProv, string searchString = "")
        {

            BancosProvContext db = new BancosProvContext();
            if (ModelState.IsValid)
            {
                Int32 idproveedor = Int32.Parse(Session["IDProveedor"].ToString());
                bancosProv.IDProveedor = idproveedor;
                db.Entry(bancosProv).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("details", new { id = bancosProv.IDProveedor, searchString= searchString });
            }
            ViewBag.IDBanco = new SelectList(db.c_Banco, "IDBanco", "Nombre", bancosProv.IDBanco);
            ViewBag.IDMoneda = new SelectList(db.c_Moneda, "IDMoneda", "Descripcion", bancosProv.IDMoneda);
            ViewBag.searchString = searchString;
            return View(bancosProv);
        }

        // GET: BancosProv/Delete/5
        public ActionResult DeleteBanco(int? id, string searchString = "")
        {
            ViewBag.searchString = searchString;
            BancosProvContext db = new BancosProvContext();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BancosProv bancosProv = db.BancosProvs.Find(id);
            if (bancosProv == null)
            {
                return HttpNotFound();
            }
            return View(bancosProv);
        }

        // POST: BancosProv/Delete/5
        [HttpPost, ActionName("DeleteBanco")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmedB(int id,string searchString = "")
        {
            ViewBag.searchString = searchString;
            BancosProvContext db = new BancosProvContext();
            BancosProv bancosProv = db.BancosProvs.Find(id);
            Int32 idproveedor = Int32.Parse(Session["IDProveedor"].ToString());
            bancosProv.IDProveedor = idproveedor;
            db.BancosProvs.Remove(bancosProv);
            db.SaveChanges();
            return RedirectToAction("details", new { id = bancosProv.IDProveedor, searchString = searchString });
        }
    
     
        public void GenerarExcelProveedor()
        {
            //Listado de datos

            List<Proveedor> proveedores = new List<Proveedor>();
            string cadena = "select * from Proveedores";
            proveedores = db.Database.SqlQuery<Proveedor>(cadena).ToList();

            //var bancos = dba.c_Bancos.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Proveedores");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:F1"].Style.Font.Size = 20;
            Sheet.Cells["A1:F1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:F1"].Style.Font.Bold = true;
            Sheet.Cells["A1:F1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Proveedores");
            Sheet.Cells["A1:F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:F2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:F2"].Style.Font.Size = 12;
            Sheet.Cells["A2:F2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:F2"].Style.Font.Bold = true;

            Sheet.Cells["A2"].RichText.Add("EMPRESA");
            Sheet.Cells["B2"].RichText.Add("DIRECCIÓN");
            Sheet.Cells["C2"].RichText.Add("MUNICIPIO");
            Sheet.Cells["D2"].RichText.Add("COLONIA");
            Sheet.Cells["E2"].RichText.Add("RFC");
            Sheet.Cells["F2"].RichText.Add("TELÉFONO");

            row = 3;
            foreach (var item in proveedores)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.Empresa;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Calle + " " + item.NoExt;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Municipio;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.Colonia;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.RFC;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Telefonouno;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelProveedor.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        public ActionResult ReportePorProveedores()
        {

            List<Proveedor> cl = new List<Proveedor>();
            string cadenaf = "SELECT * FROM Proveedores";
            cl = db.Database.SqlQuery<Proveedor>(cadenaf).ToList();
            List<SelectListItem> listacll = new List<SelectListItem>();
            listacll.Add(new SelectListItem { Text = "--Todos los proveedores--", Value = "0" });

            foreach (var m in cl)
            {
                listacll.Add(new SelectListItem { Text = m.Empresa, Value = m.IDProveedor.ToString() });
            }
            ViewBag.proveedor = listacll;

            return View();
        }

        [HttpPost]
        public ActionResult ReportePorProveedores(ReporteProveedor modelo, Proveedor A)
        {
            int idproveedor = A.IDProveedor;
            try
            {
                ProveedorContext dbc = new ProveedorContext();
                Proveedor cls = dbc.Proveedores.Find(A.IDProveedor);
            }
            catch (Exception ERR)
            {

            }

            ReporteProveedor report = new ReporteProveedor();
            //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
            byte[] abytes = report.PrepareReport(idproveedor);
            return File(abytes, "application/pdf");
            //return Redirect("index");
        }

        

    }
}
