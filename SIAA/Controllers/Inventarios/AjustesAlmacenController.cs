using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Inventarios;
using SIAAPI.Models.Comercial;
using PagedList;
using SIAAPI.Models.Login;
using SIAAPI.ViewModels.Comercial;
using OfficeOpenXml;
using System.Drawing;
using System.Globalization;
using OfficeOpenXml.Style;
using System.IO;
using static SIAAPI.Controllers.Comercial.InventarioAlmacenController;
using SIAAPI.Reportes;

namespace SIAAPI.Controllers.Inventarios
{
    public class AjustesAlmacenController : Controller
    {
        private AjustesAlmacenContext db = new AjustesAlmacenContext();

        [Authorize(Roles = "Administrador,Gerencia, Sistemas, AlmacenAjuste,Almacenista")]

        // GET: AjustesAlmacen
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = sortOrder == "FechaAjuste" ? "date_desc" : "Date";
            ViewBag.AlmacenSortParm = String.IsNullOrEmpty(sortOrder) ? "IDAlmacen" : "IDAlmacen";
            ViewBag.ArticuloSortParm = String.IsNullOrEmpty(sortOrder) ? "IDArticulo" : "IDArticulo";
            ViewBag.CaracteristicaSortParm = String.IsNullOrEmpty(sortOrder) ? "IDCaracteristica" : "IDCaracteristica";
            // Not sure here
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
            DateTime fechaini = DateTime.Now.AddDays(-60);
            //Paginación
            var elementos = from s in db.AjustesAlmacenes.Where(s=> s.FechaAjuste>= fechaini)
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.Almacen.Descripcion.Contains(searchString) || s.Articulo.Cref.Contains(searchString) || s.Articulo.Descripcion.Contains(searchString) || s.Caracteristica.Presentacion.Contains(searchString));

            } 
            
            //Ordenacion

            switch (sortOrder)
            {
                case "Date":
                    elementos = elementos.OrderBy(s => s.FechaAjuste);
                    break;
                case "date_desc":
                    elementos = elementos.OrderByDescending(s => s.FechaAjuste);
                    break;
                case "IDAlmacen":
                    elementos = elementos.OrderBy(s => s.Almacen.Descripcion);
                    break;
                case "IDArticulo":
                    elementos = elementos.OrderBy(s => s.Articulo.Descripcion);
                    break;
                case "IDCaracteristica":
                    elementos = elementos.OrderBy(s => s.Caracteristica.Presentacion);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDAjuste);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = elementos.Count(); // Total number of elements

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

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
            //return View(db.AjustesAlmacenes.ToList());
        }

        // GET: AjustesAlmacens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AjustesAlmacen ajustesAlmacen = db.AjustesAlmacenes.Find(id);
            if (ajustesAlmacen == null)
            {
                return HttpNotFound();
            }
            return View(ajustesAlmacen);
        }


        //obtiene los productos por almacen
        public ActionResult getJsonProductoAlmacen(int id)
        {
            var inventario = new VInventarioAlmacenRepository().GetArticuloxalmacentodos(id);
            Session["Almacen"] = id;
            return Json(inventario, JsonRequestBehavior.AllowGet); ;

        }
       
        public IEnumerable<SelectListItem> getProductoAlmacen(int idalmacen)
        {
            var inventario = new VInventarioAlmacenRepository().GetArticuloxalmacentodos(idalmacen);
            Session["Almacen"] = idalmacen;
            return inventario;

        } // Fin getProductoAlmacen


        // obtiene las caracteristicas de los productos
        public ActionResult getJsonCaracteristicaPorArticulo(int id)
        {
            int idalmacen = int.Parse(Session["Almacen"].ToString());
            var presentacion = new ArticuloRepository().GetCaracteristicaPorArticulo(id,idalmacen);
            return Json(presentacion, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getCaracteristicaPorProducto(int idarticulo)
        {
            int idalmacen = int.Parse(Session["Almacen"].ToString());
            var presentacion= new ArticuloRepository().GetCaracteristicaPorArticulo(idarticulo,idalmacen);
            return presentacion;

        } // Fin getCaracteristicaProducto

        public JsonResult getarticulosblando(string buscar)
        {
            buscar = buscar.Remove(buscar.Length - 1);
            var Articulos = new ArticuloContext().Articulo.Where(s => s.Cref.Contains(buscar)).OrderBy(S => S.Cref);

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (Articulo art in Articulos)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Cref + " " + art.Descripcion;
                elemento.Value = art.IDArticulo.ToString();
                opciones.Add(elemento);
            }

            return Json(opciones, JsonRequestBehavior.AllowGet);
        }

        // GET: AjustesAlmacens/Create
        public ActionResult Create()
        {
            AjustesAlmacen elemento = new AjustesAlmacen();
            elemento.FechaAjuste = DateTime.Now;
            List < User > userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            
            ViewBag.UserID = usuario;
         

            ViewBag.InventarioList = getProductoAlmacen(0);
            ViewBag.CaracteristicaList = getCaracteristicaPorProducto(0);

         

            //Almacen de de ajuste
            var almacenS = db.Almacenes.ToList();
            List<SelectListItem> liaS = new List<SelectListItem>();
            liaS.Add(new SelectListItem { Text = "--Selecciona un Almacen--", Value = "0" });
            foreach (var a in almacenS)
            {
                liaS.Add(new SelectListItem { Text = a.CodAlm + " | " + a.Descripcion, Value = a.IDAlmacen.ToString() });

            }
            ViewBag.datosAlmacenS = liaS;

            ViewBag.InventarioList = getProductoAlmacen(0);

            List<SelectListItem> car = new List<SelectListItem>();
            car.Add(new SelectListItem { Text = "--Selecciona un Almacen--", Value = "0" });

            ViewBag.Caracacteristicas = car;

           

        

            return View(elemento);
        }



        // POST: AjustesAlmacens/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AjustesAlmacen ajustesAlmacen, int? idalmacen)
        {
            ////AjustesAlmacen elemento = new AjustesAlmacen();
            ////idalmacen = elemento.Almacen.IDAlmacen;
            //int idarticulo = ajustesAlmacen.IDArticulo;

            ajustesAlmacen.FechaAjuste = DateTime.Now;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            //int idcar = ajustesAlmacen.ID;
            //ClsDatoEntero idart = db.Database.SqlQuery<ClsDatoEntero>("Select Articulo_IDArticulo as Dato from dbo.Caracteristica where ID = " + ajustesAlmacen.ID + "").ToList()[0];
            //ClsDatoEntero idpres = db.Database.SqlQuery<ClsDatoEntero>("Select IDPresentacion as Dato from dbo.Caracteristica where ID = " + ajustesAlmacen.ID + "").ToList()[0];
            string fecha2 = ajustesAlmacen.FechaAjuste.ToString("yyyy/MM/dd");

            //if (ModelState.IsValid)
            //{
            //    try
            //    {
            ClsDatoEntero idmovalm = db.Database.SqlQuery<ClsDatoEntero>("Select max(IDMovimiento) as Dato from [dbo].[MovimientoArticulo]").ToList()[0];
            var TipoA = ajustesAlmacen.TipoOperacion;
            try
            {

                if (TipoA == Tipo.Entrada)
                {
                    ClsDatoDecimal exiS = db.Database.SqlQuery<ClsDatoDecimal>("select (Existencia + " + ajustesAlmacen.Cantidad + ") as Dato from [dbo].[VInventarioAlmacen] where IDAlmacen=" + idalmacen + "  and IDCaracteristica = " + ajustesAlmacen.ID + "").ToList().FirstOrDefault();
                    if (exiS == null)
                    {
                        return Json(new { errorMessage = "No existe en presentación del Artículo en ese Almacén, generar Inventario Inicial" });//200 es ok para html
                    }
                    Caracteristica cara = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + ajustesAlmacen.ID).FirstOrDefault();

                    //Grava el movimiento de Entrada del Almacen
                    string comandos = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + ajustesAlmacen.ID + "," + cara.IDPresentacion + "," + ajustesAlmacen.IDArticulo + ",'Ajuste Inventario', " + ajustesAlmacen.Cantidad + ", 'Ajuste Inventario'," + idmovalm.Dato + ",'" + ajustesAlmacen.Lote + "', " + idalmacen + ", 'E', " + exiS.Dato + ", '" + ajustesAlmacen.Observacion + "', SYSDATETIMEOFFSET ( ))";
                    db.Database.ExecuteSqlCommand(comandos);

                    //Actualiza la existencia del articulo que entro al almacen
                    string comandos2 = "update dbo.InventarioAlmacen set Existencia= " + exiS.Dato + " where [IDAlmacen]=" + ajustesAlmacen.IDAlmacen + "  and [IDCaracteristica]= " + ajustesAlmacen.ID + "";
                    db.Database.ExecuteSqlCommand(comandos2);

                    db.Database.ExecuteSqlCommand("update inventarioalmacen set existencia=0 where existencia<0 and [IDAlmacen]=" + ajustesAlmacen.IDAlmacen + "  and [IDCaracteristica]= " + ajustesAlmacen.ID);
                    //Actualiza articulos disponibles
                    ClsDatoDecimal disp = db.Database.SqlQuery<ClsDatoDecimal>("select (Existencia-Apartado) as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + idalmacen + " and  IDCaracteristica = " + ajustesAlmacen.ID + "").ToList().FirstOrDefault();
                    string comandos3 = "update dbo.InventarioAlmacen set Disponibilidad= " + disp.Dato + " where [IDAlmacen]=" + ajustesAlmacen.IDAlmacen + "  and [IDCaracteristica]= " + ajustesAlmacen.ID + "";
                    db.Database.ExecuteSqlCommand(comandos3);
                    db.Database.ExecuteSqlCommand("update inventarioalmacen set disponibilidad=(existencia-apartado) where [IDAlmacen]=" + ajustesAlmacen.IDAlmacen + "  and [IDCaracteristica]= " + ajustesAlmacen.ID);

                    //Grava el movimiento de Ajuste al Almacen
                    string comandos4 = "insert into dbo.AjustesAlmacen(FechaAjuste, IDAlmacen, IDArticulo, ID, Lote, Cantidad, TipoOperacion, UserID, Observacion) values(GETDATE()," + idalmacen + ", " + ajustesAlmacen.IDArticulo + ", " + ajustesAlmacen.ID + ",  '" + ajustesAlmacen.Lote + "', " + ajustesAlmacen.Cantidad + ", 0, " + usuario + ",'" + ajustesAlmacen.Observacion + "')";
                    db.Database.ExecuteSqlCommand(comandos4);
                }
                if (TipoA == Tipo.Salida)
                {
                    ClsDatoDecimal exiS = db.Database.SqlQuery<ClsDatoDecimal>("select (Existencia - " + ajustesAlmacen.Cantidad + ") as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + idalmacen + " and IDArticulo = " + ajustesAlmacen.IDArticulo + " and IDCaracteristica = " + ajustesAlmacen.ID + "").ToList()[0];

                    Caracteristica cara1 = new ArticuloContext().Database.SqlQuery<Caracteristica>("select * from Caracteristica where id=" + ajustesAlmacen.ID).FirstOrDefault();
                    //Grava el movimiento de Entrada del Almacen
                    string comandos = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + ajustesAlmacen.ID + "," + cara1.IDPresentacion + "," + cara1.Articulo_IDArticulo + ",'Ajuste Inventario', " + ajustesAlmacen.Cantidad + ", 'Ajuste Inventario'," + idmovalm.Dato + ",'" + ajustesAlmacen.Lote + "', " + idalmacen + ", 'S', " + exiS.Dato + ", '" + ajustesAlmacen.Observacion + "', SYSDATETIMEOFFSET ( ))";
                    db.Database.ExecuteSqlCommand(comandos);

                    //Actualiza la existencia del articulo que entro al almacen
                    string comandos2 = "update dbo.InventarioAlmacen set Existencia= " + exiS.Dato + " where [IDAlmacen]=" + idalmacen + "  and [IDCaracteristica]= " + ajustesAlmacen.ID + "";
                    db.Database.ExecuteSqlCommand(comandos2);
                    db.Database.ExecuteSqlCommand("UPDATE INVENTARIOALMACEN SET EXISTENCIA=0 WHERE EXISTENCIA<0 and [IDAlmacen]=" + idalmacen + "  and [IDCaracteristica]= " + ajustesAlmacen.ID);
                    //Actualiza articulos disponibles
                    ClsDatoDecimal disp = db.Database.SqlQuery<ClsDatoDecimal>("select (Existencia-Apartado) as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + idalmacen + " and IDArticulo = " + ajustesAlmacen.IDArticulo + " and IDCaracteristica = " + ajustesAlmacen.ID + "").ToList()[0];
                    string comandos3 = "update dbo.InventarioAlmacen set Disponibilidad= " + disp.Dato + " where [IDAlmacen]=" + idalmacen + "  and [IDCaracteristica]= " + ajustesAlmacen.ID + "";
                    db.Database.ExecuteSqlCommand(comandos3);
                    db.Database.ExecuteSqlCommand("UPDATE INVENTARIOALMACEN SET DISPONIBILIDAD=(EXISTENCIA-APARTADO) WHERE [IDAlmacen]=" + idalmacen + "  and [IDCaracteristica]= " + ajustesAlmacen.ID);

                    //Grava el movimiento de Ajuste al Almacen
                    string comandos4 = "insert into dbo.AjustesAlmacen(FechaAjuste, IDAlmacen, IDArticulo, ID, Lote, Cantidad, TipoOperacion, UserID, Observacion) values(GETDATE()," + idalmacen + ", " + ajustesAlmacen.IDArticulo + ", " + ajustesAlmacen.ID + ",  '" + ajustesAlmacen.Lote + "', " + ajustesAlmacen.Cantidad + ", 1, " + usuario + ",'" + ajustesAlmacen.Observacion + "')";
                    db.Database.ExecuteSqlCommand(comandos4);
                }
            }
            catch (Exception err)
            {
                string error = err.Message;

                List<User> userid2 = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario2 = userid2.Select(s => s.UserID).FirstOrDefault();

                ViewBag.UserID = usuario2;
                //Almacen
                var almacen = db.Almacenes.ToList();
                List<SelectListItem> lia = new List<SelectListItem>();
                lia.Add(new SelectListItem { Text = "--Selecciona un Almacen--", Value = "0" });
                foreach (var a in almacen)
                {
                    if (a.IDAlmacen != ajustesAlmacen.IDAlmacen) // SI NO ES EL MODELO SOLO LO AÑADE AL COMBO
                    {
                        lia.Add(new SelectListItem { Text = a.IDAlmacen + " | " + a.CodAlm + " | " + a.Descripcion, Value = a.IDAlmacen.ToString() });
                    }
                    else // SI ES IGUAL AL MODELO LO CREA Y LO SELECCIONA
                    {
                        lia.Add(new SelectListItem { Text = a.IDAlmacen + " | " + a.CodAlm + " | " + a.Descripcion, Value = ajustesAlmacen.IDAlmacen.ToString(), Selected = true });
                    }


                }
                ViewBag.datosAlmacen = lia;

                ViewBag.InventarioList = getProductoAlmacen(0);
                ViewBag.CaracteristicaList = getCaracteristicaPorProducto(0);
                return View(ajustesAlmacen);
            }
            return RedirectToAction("Index");
        }
        // GET: AjustesAlmacens/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AjustesAlmacen ajustesAlmacen = db.AjustesAlmacenes.Find(id);
            if (ajustesAlmacen == null)
            {
                return HttpNotFound();
            }
            return View(ajustesAlmacen);
        }

        // POST: AjustesAlmacens/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDAjuste,FechaAjuste,IDAlmacen,IDArticulo,ID,Lote,Cantidad,TipoOperacion,UserID,Observacion")] AjustesAlmacen ajustesAlmacen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ajustesAlmacen).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ajustesAlmacen);
        }

        // GET: AjustesAlmacens/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AjustesAlmacen ajustesAlmacen = db.AjustesAlmacenes.Find(id);
            if (ajustesAlmacen == null)
            {
                return HttpNotFound();
            }
            return View(ajustesAlmacen);
        }

        // POST: AjustesAlmacens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AjustesAlmacen ajustesAlmacen = db.AjustesAlmacenes.Find(id);
            db.AjustesAlmacenes.Remove(ajustesAlmacen);
            db.SaveChanges();
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


        public ActionResult EntreFechasAI()
        {
            EFecha elemento = new EFecha();

            return View(elemento);
        }

        [HttpPost]
        public ActionResult EntreFechasAI(EFecha modelo, FormCollection coleccion)
        {
            VAjustesAlmacenContext dbr = new VAjustesAlmacenContext();
            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();

            string cual = coleccion.Get("Enviar");

            string cadena = "";
            if (cual == "Generar reporte")
            {
                return View();
            }
            if (cual == "Generar excel")
            {

                cadena = "select * from VAjusteAlmacen where fechaAjuste >= '" + FI + "' and fechaAjuste <='" + FF + " 23:59:00' ";
                var datos = dbr.Database.SqlQuery<VAjustesAlmacen>(cadena).ToList();
                ViewBag.datos = datos;

                ExcelPackage Ep = new ExcelPackage();
                //Crear la hoja y poner el nombre de la pestaña del libro
                var Sheet = Ep.Workbook.Worksheets.Add("Ajustes Almacen");

                // en la fila1 formateo las celdas y coloco el título de la hoja
                // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:L1"].Style.Font.Size = 20;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L3"].Style.Font.Bold = true;
                Sheet.Cells["A1:L1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Listado de Ajustes de Almacen");

                row = 2;
                Sheet.Cells["A1:L1"].Style.Font.Size = 12;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
                Sheet.Cells["A2:L2"].Style.Font.Bold = true;
                //Subtitulo según el filtrado del periodo de datos
                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;
                //En la fila3 se da el formato a el encabezado
                row = 3;
                Sheet.Cells.Style.Font.Name = "Calibri";
                Sheet.Cells.Style.Font.Size = 10;
                Sheet.Cells["A3:L3"].Style.Font.Bold = true;
                Sheet.Cells["A3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                // Se establece el nombre que identifica a cada una de las columnas de datos.
                Sheet.Cells["A3"].RichText.Add("ID Ajuste");
                Sheet.Cells["B3"].RichText.Add("Fecha Ajuste"); ;
                Sheet.Cells["C3"].RichText.Add("Almacen");
                Sheet.Cells["D3"].RichText.Add("Clave");
                Sheet.Cells["E3"].RichText.Add("Articulo");
                Sheet.Cells["F3"].RichText.Add("Presentacion");
                Sheet.Cells["G3"].RichText.Add("Ver.");
                Sheet.Cells["H3"].RichText.Add("Lote");
                Sheet.Cells["I3"].RichText.Add("Cantidad");
                Sheet.Cells["J3"].RichText.Add("Tipo de Operación");
                Sheet.Cells["K3"].RichText.Add("Aplico");
                Sheet.Cells["L3"].RichText.Add("Observacion");

                //Aplicar borde doble al rango de celdas A3:Q3
                Sheet.Cells["A3:L3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

                // en la fila4, el foreach llenara cada fila de la hoja de excel, con cada fila que trae de la bd.
                // Se establecen los formatos para las celdas: Fecha, Moneda
                row = 4;
                Sheet.Cells.Style.Font.Bold = false;
                foreach (VAjustesAlmacen item in ViewBag.datos)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDAjuste;
                    Sheet.Cells[string.Format("B{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.FechaAjuste;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.Almacen;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Cref;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Articulo;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Presentacion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Version;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Lote;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Cantidad;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.TipoOperacion;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.UserName;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Observacion;
                    row++;
                }

                //Se genera el archivo y se descarga

                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "AjusteAlmacen.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();

                return Redirect("/blah");
            }
            return Redirect("index");
        }
        //////////////////////////////////////////
        ///VALE DE SALIDA ALMACEN
        public ActionResult ValeSalida(string currentFilter, string searchString, int Almacen = 0, int? IDFamilia = 0, string Mensaje="")
        {
            if (searchString == null)
            {
                searchString = currentFilter;
            }
            ViewBag.Mensaje = Mensaje;
            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            ViewBag.IDFamilia = new FamAlmRepository().GetAlmacenesF(Almacen);
            ViewBag.Almacen = new AlmacenRepository().GetAlmacenes();
            ViewBag.IDAlmacen = Almacen;
            ViewBag.Familia = IDFamilia;
            ViewBag.Almacenseleccionado = Almacen;/// mandar el viewbag el parametro que viene de la pagina anterior
            List<VInventarioAlmacen> almacen = new List<VInventarioAlmacen>();
            string cadena1 = "";
            if (Almacen != 0)
            {
                cadena1 = "select Top 50 * from VInventarioAlmacen as i inner join Articulo as a on i.idarticulo=a.idarticulo where  i.IDAlmacen=" + Almacen;

                if (IDFamilia != 0)
                {
                    cadena1 = "select Top 50 * from VInventarioAlmacen as i inner join Articulo as a on i.idarticulo=a.idarticulo where  i.IDAlmacen=" + Almacen + " and a.IDFamilia=" + IDFamilia;

                }
                if (!string.IsNullOrEmpty(searchString) && IDFamilia != 0)
                {
                    cadena1 = "select Top 50 * from VInventarioAlmacen as i inner join Articulo as a on i.idarticulo=a.idarticulo where  i.IDAlmacen=" + Almacen + " and a.IDFamilia=" + IDFamilia + " and a.Cref like '%" + searchString + " %'";

                }
                if (!string.IsNullOrEmpty(searchString) && IDFamilia == 0)
                {
                    cadena1 = "select Top 50 * from VInventarioAlmacen as i inner join Articulo as a on i.idarticulo=a.idarticulo where  i.IDAlmacen=" + Almacen + " and a.Cref like '%" + searchString + "%'";

                }
                almacen = db.Database.SqlQuery<VInventarioAlmacen>(cadena1).ToList();
                ViewBag.Registros = almacen.Count();

                return View(almacen);
            }

            return View();

        }
        public ActionResult AddCarritoGeneral(FormCollection coleccion, string searchString = "")

        {
            ViewBag.searchString = searchString;
            decimal Cantidad = decimal.Parse(coleccion.Get("Cantidad").ToString());
            int id = int.Parse(coleccion.Get("id").ToString());
            int IDAlmacen = int.Parse(coleccion.Get("Almacen").ToString());
            InventarioAlmacen inventario = new InventarioAlmacenContext().Database.SqlQuery<InventarioAlmacen>("SELECT*FROM INVENTARIOALMACEN WHERE IDALMACEN="+IDAlmacen+" AND IDCARACTERISTICA="+ id).ToList().FirstOrDefault();
            if (Cantidad>inventario.Existencia) 
            { 
                return RedirectToAction("ValeSalida", new { searchString = ViewBag.searchString, Almacen = IDAlmacen , Mensaje="La cantidad es mayor a la Existencia"});
            }
            string fecha = DateTime.Now.ToString("yyyy/MM/dd");
            ArticuloContext db = new ArticuloContext();
            Caracteristica c = db.Database.SqlQuery<Caracteristica>("select * from Caracteristica where ID=" + id).ToList().FirstOrDefault();
            try
            {
                Articulo articulo = db.Articulo.Find(c.Articulo_IDArticulo);
                //string usuario = System.Web.HttpContext.Current.Session["SessionU"].ToString();
                List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
                int usuario = userid.Select(s => s.UserID).FirstOrDefault();

                int numero = db.Database.SqlQuery<CarritoVale>("SELECT * FROM [dbo].[CarritoVale] where IDCaracteristica='" + id + "' and usuario='" + usuario + "'").ToList().Count;



                if (numero != 0)
                {
                    ViewBag.errorusuario = "INTENTASTE METER DOS VECES EL MISMO ARTICULO CON LA MISMA PRESENTACION";
                    ViewBag.Mensaje = "INTENTASTE METER DOS VECES EL MISMO ARTICULO CON LA MISMA PRESENTACION";
                }
                else
                {
                    string cadena = "";
                    //ClsDatoEntero contarcliente = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarrito) as Dato from Carrito where usuario='" + usuario + "'").ToList().FirstOrDefault();


                    cadena = "insert into CarritoVale (usuario, IDCaracteristica, Cantidad, Precio, IDAlmacen) values ('" + usuario + "'," + id + "," + Cantidad + ",dbo.GetPrecio(0," + c.Articulo_IDArticulo + ",0," + Cantidad + "), " + IDAlmacen + ")";

                   

                    db.Database.ExecuteSqlCommand(cadena);

                    ViewBag.Mensaje = "ARTICULO AGREGADO AL VALE";
                }

                return RedirectToAction("ValeSalida", new { searchString = ViewBag.searchString, Almacen = IDAlmacen, Mensaje= ViewBag.Mensaje });
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                return RedirectToAction("ValeSalida", new { searchString = ViewBag.searchString, Almacen = IDAlmacen });
            }
        }
        public ActionResult CrearValeSalida(int IDAlmacen)
        {


            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();


            CarritoContext car = new CarritoContext();

            ViewBag.IDAlmacen = IDAlmacen;



            List<VCarritoVale> pedido = db.Database.SqlQuery<VCarritoVale>("select CarritoVale.Precio,CarritoVale.IDCarritoVale,CarritoVale.usuario,CarritoVale.IDAlmacen, CarritoVale.IDCaracteristica,CarritoVale.Cantidad,c_ClaveUnidad.Nombre as Unidad, c_Moneda.ClaveMoneda as Moneda, Articulo.Cref as Cref, Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,CarritoVale.Precio * CarritoVale.Cantidad as Importe from  CarritoVale INNER JOIN Caracteristica ON CarritoVale.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Articulo.IDMoneda INNER JOIN  c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            //var divisa = db.Database.SqlQuery<ResumenFac>("select (select ClaveMoneda FROM C_MONEDA WHERE IDMoneda=Carrito.IDMoneda) as MonedaOrigen, (select SUM(Carrito.Precio * Carrito.Cantidad)) as Subtotal, SUM(Carrito.Precio * Carrito.Cantidad)*0.16 as IVA, (SUM(Carrito.Precio * Carrito.Cantidad)) + (SUM(Carrito.Precio * Carrito.Cantidad)*0.16) as Total ,0 as TotalPesos from Carrito INNER JOIN Caracteristica ON Carrito.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda = Carrito.IDMoneda INNER JOIN c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario=" + usuario + " group by Carrito.IDMoneda").ToList();
            //ViewBag.sumatoria = divisa;


            ViewBag.carrito = pedido;


            ClsDatoEntero c = db.Database.SqlQuery<ClsDatoEntero>("select count(IDCarritoVale) as Dato from CarritoVale where  usuario='" + usuario + "'").ToList()[0];
            int x = c.Dato;
            ViewBag.dato = x;

            ClsDatoEntero cantidad = db.Database.SqlQuery<ClsDatoEntero>("select count(Cantidad) as Dato from CarritoVale where Cantidad=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datocantidad = cantidad.Dato;

            ClsDatoEntero preciocontar = db.Database.SqlQuery<ClsDatoEntero>("select count(Precio) as Dato from CarritoVale where Precio=0 and usuario='" + usuario + "'").ToList()[0];
            ViewBag.datoprecio = preciocontar.Dato;


            List<ValidarCarrito> validaprecio = db.Database.SqlQuery<ValidarCarrito>("select CarritoVale.Precio, dbo.GetValidaCosto(Articulo.IDArticulo, CarritoVale.Cantidad, CarritoVale.Precio) as Dato from CarritoVale INNER JOIN Caracteristica ON CarritoVale.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo  where CarritoVale.usuario='" + usuario + "'").ToList();
            int countDato = validaprecio.Count(s => s.Dato.Equals(true));

            int countCarrito = pedido.Count();

            if (countDato == countCarrito)
            {
                ViewBag.validacarrito = 1;
            }
            else
            {
                ViewBag.validacarrito = 0;
            }
            //Termina la consulta del carrito
            return View();
        }

        [HttpPost]
        public ActionResult CrearValeSalida(ValeSalida vale, FormCollection coleccion)
        {
            string precioP = coleccion.Get("Precio");
            decimal subtotal = 0, iva = 0, total = 0, subtotalr = 0, ivar = 0, totalr = 0, importetotal = 0, importe = 0, importeiva = 0;
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<VCarritoVale> pedido = db.Database.SqlQuery<VCarritoVale>("select CarritoVale.Precio,CarritoVale.IDCarritoVale,CarritoVale.usuario,CarritoVale.IDAlmacen, CarritoVale.IDCaracteristica,CarritoVale.Cantidad,c_ClaveUnidad.Nombre as Unidad, c_Moneda.ClaveMoneda as Moneda, Articulo.Cref as Cref, Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,CarritoVale.Precio * CarritoVale.Cantidad as Importe from  CarritoVale INNER JOIN Caracteristica ON CarritoVale.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Articulo.IDMoneda INNER JOIN  c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where usuario='" + usuario + "'").ToList();

            try
            {
                foreach (var det in pedido)
                {
                    Articulo articulokit = new ArticuloContext().Articulo.Find(det.IDArticulo);

                    InventarioAlmacen ia = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == det.IDAlmacen && s.IDCaracteristica == det.IDCaracteristica).FirstOrDefault();

                    if (ia == null)
                    {

                        ClsDatoString almacen = db.Database.SqlQuery<ClsDatoString>("select Descripcion as Dato from Almacen where IDAlmacen ='" + det.IDAlmacen + "'").ToList()[0];



                        throw new Exception("No hay Existencia suficiente almacen para " + articulokit.Cref + " en la cantidad de " + det.Cantidad);
                    }
                    if (ia.Existencia < det.Cantidad)
                    {
                        throw new Exception("No hay Existencia suficiente almacen para " + articulokit.Cref + " en la cantidad de " + det.Cantidad);
                    }

                }
            }
            catch (Exception err)
            {

            }
            

            try
            {
                subtotal = pedido.Sum(s => s.Importe);
                iva = subtotal * (decimal)0.16;
                total = subtotal + iva;
            }
            catch (Exception e)
            {

            }
            ViewBag.Subtotal = subtotal.ToString("C");
            ViewBag.IVA = iva.ToString("C");
            ViewBag.Total = total.ToString("C");
            ViewBag.carrito = pedido;
            //Termina 

            if (ModelState.IsValid)
            {
                int num = 0;
                DateTime fecha = DateTime.Now;
                string fecha1 = fecha.ToString("yyyy/MM/dd");



                db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[ValeSalida]([Fecha],[IDAlmacen],[UserID],[Observacion],[Entregar],[Concepto],[Solicito], [Estado]) values ('" + fecha1 + "','" + vale.IDAlmacen + "','" + usuario + "','" + vale.Observacion + "','" + vale.Entregar + "','" + vale.Concepto + "','" + vale.Solicito + "','Inactivo')");
                //db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[EncPedido]([Fecha],[FechaRequiere],[IDCliente],[IDFormapago],[IDMoneda],[Observacion],[Subtotal],[IVA],[Total],[IDMetodoPago],[IDCondicionesPago],[IDAlmacen],[Status],[TipoCambio],[UserID],[IDUsoCFDI],[IDVendedor],[Entrega]) values ('" + fecha1 + "','" + fecha2 + "','" + encPedido.IDCliente + "','" + encPedido.IDFormapago + "','" + encPedido.IDMoneda + "','" + encPedido.Observacion + "','" + subtotal + "','" + iva + "','" + total + "','" + encPedido.IDMetodoPago + "','" + encPedido.IDCondicionesPago + "','" + encPedido.IDAlmacen + "','Inactivo','" + tCambio + "','" + usuario + "','" + encPedido.IDUsoCFDI + "','" + encPedido.IDVendedor + "','" + encPedido.Entrega + "')");
                db.SaveChanges();


                List<ValeSalida> numero;
                numero = db.Database.SqlQuery<ValeSalida>("SELECT * FROM [dbo].[ValeSalida] WHERE IDValeSalida = (SELECT MAX(IDValeSalida) from ValeSalida)").ToList();
                num = numero.Select(s => s.IDValeSalida).FirstOrDefault();

                //   string cadenafinal = "";
                int contador = 0;
               string[] arraydatos1 = precioP.Split(',');

                foreach (var det in pedido)
                {
                    decimal Precio = decimal.Parse(arraydatos1[contador]);

                    Articulo articulo = new ArticuloContext().Articulo.Find(det.IDArticulo);
                   
                    importe = det.Cantidad * Precio;
                    importeiva = importe * decimal.Parse(SIAAPI.Properties.Settings.Default.ValorIVA);
                    importetotal = importeiva + importe;
                    db.Database.ExecuteSqlCommand("INSERT INTO DetValeSalida([IDValeSalida],[IDArticulo],[Moneda],[IDCaracteristica],[Cantidad],[IDAlmacen],[Precio],[Importe]) values ('" + num + "','" + det.IDArticulo + "','" + det.Moneda + "','" + det.IDCaracteristica + "','" + det.Cantidad + "','" + det.IDAlmacen + "','" + Precio + "','" + importe + "')");

                    db.Database.ExecuteSqlCommand("delete from CarritoVale where IDCarritoVale='" + det.IDCarritoVale + "'");
                    db.SaveChanges();

                    contador++;














                }

                //var detallepedidoa = new ValeSalidaContext().DetValeSalida.Where(s => s.IDValeSalida == vale.IDValeSalida);











                return RedirectToAction("ValesDeSalida");


            }



            return View(vale);

        }

        public ActionResult ValesDeSalida(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = sortOrder == "FechaAjuste" ? "date_desc" : "Date";
            ViewBag.AlmacenSortParm = String.IsNullOrEmpty(sortOrder) ? "IDAlmacen" : "IDAlmacen";
            ViewBag.ArticuloSortParm = String.IsNullOrEmpty(sortOrder) ? "IDArticulo" : "IDArticulo";
            ViewBag.CaracteristicaSortParm = String.IsNullOrEmpty(sortOrder) ? "IDCaracteristica" : "IDCaracteristica";
            // Not sure here
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
            ValeSalidaContext dl = new ValeSalidaContext();
            ViewBag.CurrentFilter = searchString;
            DateTime fechaini = DateTime.Now.AddDays(-60);
            //Paginación
            string cadena;
            cadena = "select * from ValeSalida order by IDValeSalida desc";

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                cadena = "select * from ValeSalida where Solicito like '%" + searchString + "' or concepto like'%" + searchString + "%' or idvalesalida like '%" + searchString + "'";



            }
            List<ValeSalida> elementos = dl.Database.SqlQuery<ValeSalida>(cadena).ToList<ValeSalida>();

            //Ordenacion


            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = elementos.Count(); // Total number of elements

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

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
            //return View(db.AjustesAlmacenes.ToList());
        }

        public ActionResult CancelarVale(int? id)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            ValeSalida vales = new ValeSalidaContext().ValeSalida.Find(id);
            try
            {


                List<DetValeSalida> valeSalidas = new ArticuloContext().Database.SqlQuery<DetValeSalida>("select * from DetValeSalida where IDValeSalida = " + id).ToList();


                string fecha = DateTime.Now.ToString("yyyyMMdd");

               
                    foreach (var det in valeSalidas)
                    {
                        Articulo articulo = new ArticuloContext().Articulo.Find(det.IDArticulo);

                        try
                        {
                            if (articulo.CtrlStock)
                            {
                                Caracteristica carateristica = new ValeSalidaContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + det.IDCaracteristica).ToList().FirstOrDefault();
                                try
                                {
                                if (vales.Estado == "Activo")
                                {
                                    string cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia+ " + det.Cantidad + ") WHERE IDAlmacen = " + det.IDAlmacen + " and IDCaracteristica =" + det.IDCaracteristica + "";
                                    db.Database.ExecuteSqlCommand(cadenapro);
                                    db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad=(existencia-apartado) where IDAlmacen=" + det.IDAlmacen + " and idcaracteristica=" + det.IDCaracteristica);
                                }

                            }
                                catch (Exception err)
                                {

                                }

                                try
                                {
                                    InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == det.IDAlmacen && s.IDCaracteristica == det.IDCaracteristica).FirstOrDefault();

                                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + det.IDArticulo + ",'Cancelación vale de salida',";
                                    cadenam += det.Cantidad + ",'Vale de salida '," + id + ",'',";
                                    cadenam += det.IDAlmacen + ",'E'," + invO.Existencia + ",'Cancelación vale de salida',CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";

                                    db.Database.ExecuteSqlCommand(cadenam);
                                }
                                catch (Exception err)
                                {

                                }
                            }

                        }
                        catch (Exception err)
                        {

                        }

                    }

                


                db.Database.ExecuteSqlCommand("update ValeSalida set [Estado]='Cancelado' where IDValeSalida='" + id + "'");
            }
            catch (Exception err)
            {
                return Content(err.Message);
            }
            return RedirectToAction("ValesDeSalida");
        }

        public ActionResult DetailsVales(int? id)
        {

            List<VDetValeSalida> pedido = db.Database.SqlQuery<VDetValeSalida>("select DetValeSalida.Precio,DetValeSalida.IDDetValeSalida, DetValeSalida.IDAlmacen, DetValeSalida.IDCaracteristica,DetValeSalida.Cantidad,c_ClaveUnidad.Nombre as Unidad, c_Moneda.ClaveMoneda as Moneda, Articulo.Cref as Cref, Caracteristica.Presentacion as Presentacion,Articulo.Descripcion,Articulo.IDArticulo,DetValeSalida.Precio * DetValeSalida.Cantidad as Importe from  DetValeSalida INNER JOIN Caracteristica ON DetValeSalida.IDCaracteristica= Caracteristica.ID INNER JOIN Articulo ON Articulo.IDArticulo = Caracteristica.Articulo_IDArticulo INNER JOIN c_Moneda ON c_Moneda.IDMoneda =Articulo.IDMoneda INNER JOIN  c_ClaveUnidad ON c_ClaveUnidad.IDClaveUnidad = Articulo.IDClaveUnidad where DetValeSalida.IDValeSalida=" + id).ToList();

            ViewBag.req = pedido;


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ValeSalida valeSalida = new ValeSalidaContext().ValeSalida.Find(id);
            if (valeSalida == null)
            {
                return HttpNotFound();
            }
            return View(valeSalida);
        }

        [HttpPost]
        public JsonResult DeleteitemVale(int id)
        {
            try
            {
                CarritoContext car = new CarritoContext();
                car.Database.ExecuteSqlCommand("delete from CarritoVale where IDCarritoVale=" + id);

                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }

        public ActionResult PdfVale(int id)
        {

            ValeSalida pedido = new ValeSalidaContext().ValeSalida.Find(id);
            DocumentoVale x = new DocumentoVale();

            x.fecha = pedido.Fecha.ToShortDateString();
            x.folio = pedido.IDValeSalida.ToString();
            x.IDValeSalida = pedido.IDValeSalida;
            x.Entregado = pedido.Entregar;
            x.Observacion = pedido.Observacion;
            x.Concepto = pedido.Concepto;
            x.Solicito = pedido.Solicito;

            EmpresaContext dbe = new EmpresaContext();

            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);
            x.Empresa = empresa.RazonSocial;
            x.Telefono = empresa.Telefono;
            x.RFC = empresa.RFC;
            x.Direccion = empresa.Calle + " " + empresa.NoExt + " " + empresa.NoInt + "\n" + empresa.Colonia + " \n" + empresa.Municipio + "," + empresa.Estados.Estado;
            //x.firmadefinanzas = empresa.Director_finanzas;
            //x.firmadecompras = empresa.Persona_de_Compras + "";

            List<DetValeSalida> detalles = db.Database.SqlQuery<DetValeSalida>("select * from [dbo].[DetValeSalida] where IDValeSalida=" + id).ToList();

            int contador = 1;
            foreach (var item in detalles)
            {
                ProductoVale producto = new ProductoVale();
                Articulo arti = new ArticuloContext().Articulo.Find(item.IDArticulo);
                Almacen alma = new AlmacenContext().Almacenes.Find(item.IDAlmacen);
                Caracteristica caracteristica = new ValeSalidaContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + item.IDCaracteristica).ToList().FirstOrDefault();

                producto.ClaveProducto = arti.Cref;
                producto.idarticulo = arti.IDArticulo;
                producto.c_unidad = arti.c_ClaveUnidad.ClaveUnidad;
                producto.cantidad = item.Cantidad.ToString();
                producto.descripcion = arti.Descripcion;
                //producto.Presentacion = carateristica.Presentacion;
                producto.almacen = alma.CodAlm;
                //producto.valorUnitario = float.Parse(item.Costo.ToString());
                producto.v_unitario = float.Parse(item.Precio.ToString());
                producto.Moneda = item.Moneda;
                producto.importe = float.Parse(item.Importe.ToString());



                producto.Presentacion = "" + caracteristica.IDPresentacion.ToString(); //item.presentacion;//item.presentacion;

                producto.numIdentificacion = contador.ToString();
                contador++;

                x.productos.Add(producto);

            }

            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            //try
            //{


            CreaValePDF documentop = new CreaValePDF(logoempresa, x);

            // string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            //return new FilePathResult(documentop.nombreDocumento, contentType);

            //}
            //catch (Exception err)
            //{

            //}

            return RedirectToAction("ValesDeSalida");

        }
        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            System.Drawing.Image returnImage = null;
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                returnImage = System.Drawing.Image.FromStream(ms, true);//Exception occurs here
            }
            catch { }
            return returnImage;

        }



        public ActionResult AutorizarVale(int? id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("update ValeSalida set estado='Activo' where IDValeSalida=" + id);
            }
            catch (Exception err)
            {

            }
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            List<DetValeSalida> vale = db.Database.SqlQuery<DetValeSalida>("select * from DetValeSalida where IDValeSalida= " + id).ToList();

            foreach (var det in vale)
            {
                Articulo articulo = new ArticuloContext().Articulo.Find(det.IDArticulo);
                try
                {
                    if (articulo.CtrlStock)
                    {
                        Caracteristica carateristica = new ValeSalidaContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + det.IDCaracteristica).ToList().FirstOrDefault();
                        try
                        {
                            string cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia- " + det.Cantidad + ") WHERE IDAlmacen = " + det.IDAlmacen + " and IDCaracteristica =" + det.IDCaracteristica + "";
                            db.Database.ExecuteSqlCommand(cadenapro);
                            db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad=(existencia-apartado) where IDAlmacen=" + det.IDAlmacen + " and idcaracteristica=" + det.IDCaracteristica);

                        }
                        catch (Exception err)
                        {

                        }
                        try
                        {
                            InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == det.IDAlmacen && s.IDCaracteristica == det.IDCaracteristica).FirstOrDefault();

                            string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                            cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + det.IDArticulo + ",'Vale de salida',";
                            cadenam += det.Cantidad + ",'Vale de salida '," + id + ",'',";
                            cadenam += det.IDAlmacen + ",'S'," + invO.Existencia + ",'Autorización vale de salida', CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";

                            db.Database.ExecuteSqlCommand(cadenam);
                        }
                        catch (Exception err)
                        {

                        }
                    }

                }
                catch (Exception err)
                {

                }
            }

            return RedirectToAction("ValesDeSalida");
        }

    }
}
