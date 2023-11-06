using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using System.Threading.Tasks;
using PagedList;
using System.Security.Cryptography;
using System.Text;
using SIAAPI.ViewModels.Comercial;
using SIAAPI.Models.Login;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using SIAAPI.Models.Comisiones;
using SIAAPI.Reportes;

namespace SIAAPI.Controllers.Comercial

{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion, GerenteVentas")]
    public class VendedorController : Controller
    {
        private VendedorContext db = new VendedorContext();
        private CuotaMensualContext dbCu = new CuotaMensualContext();
        private CierreVentasContext db2 = new CierreVentasContext();
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

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
            //Paginación

            var elementos = from s in db.Vendedores.Include(v => v.c_Moneda).Include(v => v.c_PeriodicidadPago).Include(v => v.c_TipoContrato).Include(v => v.Oficina).Include(v => v.TipoVendedor)
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.Nombre.Contains(searchString));

            }

            //Ordenacion


            //Paginación
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Vendedores.OrderBy(e => e.IDVendedor).Count(); // Total number of elements

            // return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.OrderBy(i => i.Nombre).ToPagedList(page ?? 1, 3));
            //Paginación
            return View(elementos.ToList());
        }


        // GET: Vendedor
        //public ActionResult Index()
        //{
        //    var vendedores = db.Vendedores.Include(v => v.c_Moneda).Include(v => v.c_PeriodicidadPago).Include(v => v.c_TipoContrato).Include(v => v.Oficina).Include(v => v.TipoVendedor);
        //    return View(vendedores.ToList());
        //}

        // GET: Vendedor/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendedor vendedor = db.Vendedores.Find(id);
            if (vendedor == null)
            {
                return HttpNotFound();
            }
            return View(vendedor);
        }

        // GET: Vendedor/Create
        public ActionResult Create()
        {
            //List<SelectListItem> tipocuota = new List<SelectListItem>();
            //tipocuota.Add(new SelectListItem { Text = "FijaH", Value = "1" });
            //tipocuota.Add(new SelectListItem { Text = "MensualH", Value = "2" });
            //ViewBag.IDTipoCuota = tipocuota;

            ViewBag.IDTipoCuota = new SelectList(db.c_TipoCuota, "IDTipoCuota", "TipoCuota");
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion");
            ViewBag.IDPeriocidadPago = new SelectList(db.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion");
            ViewBag.IDTipoContrato = new SelectList(db.c_TipoContratos, "IDTipoContrato", "Descripcion");
            ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina");
            ViewBag.IDTipoVendedor = new SelectList(db.TipoVendedores, "IDTipoVendedor", "DescripcionVendedor");
            var lista = new VendedorRepository().GetEsquema();
            ViewBag.listaEsquema = lista;
            return View();
        }

        // POST: Vendedor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Vendedor vendedor)
        {
            //ViewBag.Data1 = valScript;

            try
            {

                HttpPostedFileBase archivo = Request.Files["Image1"];
                if (archivo.FileName != "")
                {
                    vendedor.Photo = new byte[archivo.ContentLength];
                    archivo.InputStream.Read(vendedor.Photo, 0, archivo.ContentLength);
                }

                string insertar = "INSERT INTO [dbo].[Vendedor]([RFC],[Nombre],[CuotaVendedor],[IDTipoContrato],[AutorizadoACotizar],[Activo],[Porcentajecomision],[IDTipoVendedor],[IDPeriocidadPago],[IDOficina],[Mail],[Telefono],[Perfil],[Photo],[Notas],[IDMoneda],[Comision],[EsquemaComision],[IDTipoCuota]) values('" +
                    vendedor.RFC + "','" + vendedor.Nombre + "','" + vendedor.CuotaVendedor + "','" + vendedor.IDTipoContrato + "','" + vendedor.AutorizadoACotizar + "','" + vendedor.Activo + "','" + vendedor.Porcentajecomision + "','" + vendedor.IDTipoVendedor + "','" + vendedor.IDPeriocidadPago + "','" + vendedor.IDOficina + "','" + vendedor.Mail + "','" + vendedor.Telefono + "','" + vendedor.Perfil + "','" + vendedor.Photo + "','" + vendedor.Notas + "','" + vendedor.IDMoneda + "','" + vendedor.Comision + "','" + vendedor.EsquemaComision + "','" + vendedor.IDTipoCuota + "')";
                db.Database.ExecuteSqlCommand(insertar);
                //db.Vendedores.Add(vendedor);
                //string pass = MD5P(vendedor.Perfil);

                //try
                //{
                //    db.Database.ExecuteSqlCommand("insert into [dbo].[User]([Username],[Password],[EmailID],[Estado]) values ('" + vendedor.Mail + "','" + pass + "','" + vendedor.Nombre + "','Asignado')");
                //    List<User> numero = db.Database.SqlQuery<User>("SELECT * FROM [dbo].[User] WHERE UserID = (SELECT MAX(UserID) from [dbo].[User])").ToList();
                //    int num = numero.Select(s => s.UserID).FirstOrDefault();
                //    ClsDatoEntero numrol = db.Database.SqlQuery<ClsDatoEntero>("select RoleID as Dato from Roles where ROleName='Ventas'").ToList()[0];
                //    db.Database.ExecuteSqlCommand("insert into [dbo].[UserRole]([RoleID],[UserID]) values ('" + numrol.Dato + "','" + num + "')");
                //    db.SaveChanges();
                //}
                //catch (Exception err)
                //{

                //}
                return RedirectToAction("Index");
            }
            catch (Exception er)
            {
                ViewBag.IDTipoCuota = new SelectList(db.c_TipoCuota, "IDTipoCuota", "TipoCuota", vendedor.IDTipoCuota);
                ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", vendedor.IDMoneda);
                ViewBag.IDPeriocidadPago = new SelectList(db.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion", vendedor.IDPeriocidadPago);
                ViewBag.IDTipoContrato = new SelectList(db.c_TipoContratos, "IDTipoContrato", "Descripcion", vendedor.IDTipoContrato);
                ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina", vendedor.IDOficina);
                ViewBag.IDTipoVendedor = new SelectList(db.TipoVendedores, "IDTipoVendedor", "DescripcionVendedor", vendedor.IDTipoVendedor);
                var lista = new VendedorRepository().GetEsquema();
                ViewBag.listaEsquema = lista;
                ViewBag.mensaje = er.Message;
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



        // GET: Vendedor/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendedor vendedor = db.Vendedores.Find(id);
            if (vendedor == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDTipoCuota = new SelectList(db.c_TipoCuota, "IDTipoCuota", "TipoCuota", vendedor.IDTipoCuota);
            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", vendedor.IDMoneda);
            ViewBag.IDPeriocidadPago = new SelectList(db.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion", vendedor.IDPeriocidadPago);
            ViewBag.IDTipoContrato = new SelectList(db.c_TipoContratos, "IDTipoContrato", "Descripcion", vendedor.IDTipoContrato);
            ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina", vendedor.IDOficina);
            ViewBag.IDTipoVendedor = new SelectList(db.TipoVendedores, "IDTipoVendedor", "DescripcionVendedor", vendedor.IDTipoVendedor);
            var lista = new VendedorRepository().GetEsquema();
            ViewBag.listaEsquema = lista;
            return View(vendedor);
        }

        // POST: Vendedor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Vendedor vendedor, int id)
        {
            try
            {
                var elemento = db.Vendedores.Single(m => m.IDVendedor == id);
                HttpPostedFileBase archivo = Request.Files["Image1"];
                if (archivo.FileName != "")
                {
                    elemento.Photo = new byte[archivo.ContentLength];
                    archivo.InputStream.Read(elemento.Photo, 0, archivo.ContentLength);
                }

                if (TryUpdateModel(elemento))
                {
                    //string pass = MD5P(vendedor.Perfil);
                    //db.Database.ExecuteSqlCommand("update [dbo].[User] set [Username]='" + vendedor.Mail + "',[Password]='" + pass + "',[EmailID]='" + vendedor.Nombre + "' where Session='" + ses + "'");
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.IDTipoCuota = new SelectList(db.c_TipoCuota, "IDTipoCuota", "TipoCuota", vendedor.IDTipoCuota);
                ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", vendedor.IDMoneda);
                ViewBag.IDPeriocidadPago = new SelectList(db.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion", vendedor.IDPeriocidadPago);
                ViewBag.IDTipoContrato = new SelectList(db.c_TipoContratos, "IDTipoContrato", "Descripcion", vendedor.IDTipoContrato);
                ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina", vendedor.IDOficina);
                ViewBag.IDTipoVendedor = new SelectList(db.TipoVendedores, "IDTipoVendedor", "DescripcionVendedor", vendedor.IDTipoVendedor);
                var lista = new VendedorRepository().GetEsquema();
                ViewBag.listaEsquema = lista;
                return View(vendedor);
            }
            catch (Exception err)
            {
                ViewBag.IDTipoCuota = new SelectList(db.c_TipoCuota, "IDTipoCuota", "TipoCuota", vendedor.IDTipoCuota);
                ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", vendedor.IDMoneda);
                ViewBag.IDPeriocidadPago = new SelectList(db.c_PeriocidadPagos, "IDPeriocidadPago", "Descripcion", vendedor.IDPeriocidadPago);
                ViewBag.IDTipoContrato = new SelectList(db.c_TipoContratos, "IDTipoContrato", "Descripcion", vendedor.IDTipoContrato);
                ViewBag.IDOficina = new SelectList(db.Oficinas, "IDOficina", "NombreOficina", vendedor.IDOficina);
                ViewBag.IDTipoVendedor = new SelectList(db.TipoVendedores, "IDTipoVendedor", "DescripcionVendedor", vendedor.IDTipoVendedor);
                var lista = new VendedorRepository().GetEsquema();
                ViewBag.listaEsquema = lista;
                return View();
            }


        }

        // GET: Vendedor/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vendedor vendedor = db.Vendedores.Find(id);
            if (vendedor == null)
            {
                return HttpNotFound();
            }
            return View(vendedor);
        }

        // POST: Vendedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vendedor vendedor = db.Vendedores.Find(id);
            db.Vendedores.Remove(vendedor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> RenderImage(int id)
        {
            VendedorContext db = new VendedorContext();
            Vendedor item = await db.Vendedores.FindAsync(id);

            byte[] photoBack = item.Photo;

            return File(photoBack, "image/png");
        }
     

        public ActionResult CargarCuotaMensual(int id)

        {
            //List<CuotaMensual> elementos = dbCu.Database.SqlQuery<CuotaMensual>("select * from [dbo].[CuotaMensual] where IDVendedor =" + id + "").ToList();
            Vendedor vendedorAR = db.Vendedores.Find(id);
            //Vendedor vendedorAR = new VendedorContext().Vendedores.Find(id);
            //CuotaMensual vendedorAR = new CuotaMensualContext().Vendedores.Find(id);
            ViewBag.id = id;
            ViewBag.nombre = vendedorAR.Nombre;

            ViewBag.IDMoneda = new SelectList(db.c_Monedas, "IDMoneda", "Descripcion", vendedorAR.IDMoneda);
            ViewBag.IDMes = new SelectList(db2.c_MesesBD, "IDMes", "Mes");

            List<SelectListItem> meses = new List<SelectListItem>();
            //List<Meses> mesesall = db.Database.SqlQuery<VEntrega>("select en.IDEntrega,en.IDCliente,en.CalleEntrega,en.NumExtEntrega,ISNULL(en.NumIntEntrega,0),en.ColoniaEntrega,en.MunicipioEntrega,en.CPEntrega,en.ObservacionEntrega,es.Estado from dbo.Entrega as en inner join Estados as es on es.IDEstado=en.IDEstado where IDCliente='" + cambio.Dato + "'").ToList();
            CuotaMensual cuota = new CuotaMensual();
            cuota.IDVendedor = id;
            cuota.Ano = DateTime.Now.Year;
            //meses.Add(new SelectListItem { Text = "Enero", Value ="1" });
            //meses.Add(new SelectListItem { Text = "Febrero", Value = "2" });
            //meses.Add(new SelectListItem { Text = "Marzo", Value = "3" });
            //meses.Add(new SelectListItem { Text = "Abril", Value = "4" });
            //meses.Add(new SelectListItem { Text = "Mayo", Value = "5" });
            //meses.Add(new SelectListItem { Text = "Junio", Value = "6" });
            //meses.Add(new SelectListItem { Text = "Julio", Value = "7" });
            //meses.Add(new SelectListItem { Text = "Agosto", Value = "8" });
            //meses.Add(new SelectListItem { Text = "Septiembre", Value = "9" });
            //meses.Add(new SelectListItem { Text = "Octubre", Value = "10" });
            //meses.Add(new SelectListItem { Text = "Noviembre", Value = "11" });
            //meses.Add(new SelectListItem { Text = "Diciembre", Value = "12" });
            //ViewBag.meses = meses;

            return View(cuota);
        }

        [HttpPost]
        public ActionResult InsertarCuotaMensual(int IDMes, int Ano, decimal Cuota, decimal PorcComision, int IDMoneda, int IDVendedor)
        {
            ViewBag.id = IDVendedor;
            Vendedor vendedorAR = db.Vendedores.Find(IDVendedor);
            //ViewBag.idCuota = IDCuotaMensual;
            //CuotaMensual cuotaAR = dbCu.CuotaMensual.Find(IDCuotaMensual);

            string cadena1 = "select TOP 24 * from CuotaMensual where idVendedor=" + ViewBag.id + " order by ano desc, idmes desc";
            string cadena2 = "select * from CuotaMensual where IDVendedor= " + IDVendedor + " and ano=" + Ano + " and idmes=" + IDMes + "";
            CuotaMensualContext dbart = new CuotaMensualContext();
            List<CuotaMensual> articulos = dbart.Database.SqlQuery<CuotaMensual>(cadena1).ToList();
            List<CuotaMensual> datosrep = dbart.Database.SqlQuery<CuotaMensual>(cadena2).ToList();

            try
            {

                if (datosrep.Count <= 0)
                {
                    dbCu.Database.ExecuteSqlCommand("insert into CuotaMensual(IDMes, Ano, Cuota, PorcComision, IDMoneda, IDVendedor) values(" + IDMes + ", " + Ano + ", " + Cuota + ", " + PorcComision + ", " + IDMoneda + "," + IDVendedor + ")");
                }
                else
                {
                    throw new Exception("No puedes ingresar la cuota de este mes, debido a que existe un registo");
                }
            }
            catch (Exception err)
            {
                return Json(err.Message);
            }
            return RedirectToAction("CargarCuotaMensual", new { id = IDVendedor });
        }

        [HttpPost]
        public JsonResult EditCuotas(int Mes, int Ano, decimal Cuota, decimal PorcComision, int IDMoneda, int IDVendedor, int IDCuotaMensual)
        {
            try
            {
                new CuotaMensualContext().Database.ExecuteSqlCommand("update CuotaMensual set Mes = 99, Ano = 9999, Cuota = 9999, PorcComision = 99, IDMoneda = 180 where IDVendedor = 9 and IDCuotaMensual = 2");


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


        public ActionResult Comisiones(string sortOrder, string currentFilter, string searchString, int? page)
        {

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
            //Paginación

            var elementos = from s in db.Vendedores.Include(v => v.c_Moneda).Include(v => v.c_PeriodicidadPago).Include(v => v.c_TipoContrato).Include(v => v.Oficina).Include(v => v.TipoVendedor)
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.Nombre.Contains(searchString));

            }

            //Ordenacion


            //Paginación
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Vendedores.OrderBy(e => e.IDVendedor).Count(); // Total number of elements

            // return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.OrderBy(i => i.Nombre).ToPagedList(page ?? 1, 3));
            //Paginación
            return View(elementos.ToList());
        }
        [HttpPost]
        public JsonResult DeleteCuotaMensual(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from CuotaMensual where IDCuotaMensual ='" + id + "'");
                return Json(true);

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        public ActionResult ReportePorVendedores()
        {

            List<Vendedor> lstVen = new List<Vendedor>();
            string sqlVen = "SELECT * FROM Vendedor";
            lstVen = db.Database.SqlQuery<Vendedor>(sqlVen).ToList();
            List<SelectListItem> listaven = new List<SelectListItem>();
            listaven.Add(new SelectListItem { Text = "--Todos los Vendedores--", Value = "0" });

            foreach (var m in lstVen)
            {
                listaven.Add(new SelectListItem { Text = m.Nombre, Value = m.IDVendedor.ToString() });
            }
            ViewBag.IDVendedor = listaven;

            return View();
        }

        [HttpPost]
        public ActionResult ReportePorVendedores(ReporteVendedor modelo, Vendedor A)
        {
            int idvendedor = A.IDVendedor;
            try
            {
                VendedorContext dbc = new VendedorContext();
                Vendedor cls = dbc.Vendedores.Find(A.IDVendedor);
            }
            catch (Exception ERR)
            {

            }

            ReporteVendedor report = new ReporteVendedor();
            //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
            byte[] abytes = report.PrepareReport(idvendedor);
            return File(abytes, "application/pdf");
            //return Redirect("index");
        }

    }
}
