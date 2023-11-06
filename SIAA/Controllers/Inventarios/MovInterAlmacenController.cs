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
using SIAAPI.Models.Produccion;
using SIAAPI.ViewModels.Articulo;

using System.IO;
using SIAAPI.ViewModels.Comercial;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SIAAPI.Reportes;
using System.Globalization;
using SIAAPI.Models.Login;

namespace SIAAPI.Controllers.Inventarios
{
    public class MovInterAlmacenController : Controller
    {
        private MovInterAlmacenContext db = new MovInterAlmacenContext();
        private VMovInterAlmacenContext dbv = new VMovInterAlmacenContext();

        // GET: MovInterAlmacen
      
            
             [Authorize(Roles = "Administrador,Sistemas,Almacenista,Comercial,Ventas,Facturacion")]




        public ViewResult Index( string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //return View(dbv.VMovInterAlmacenes.ToList());
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDMovimientoSortParm = String.IsNullOrEmpty(sortOrder) ? "IDMovimiento" : "";
            ViewBag.FechaMovimientoSortParm = sortOrder == "FechaMovimiento" ? "date_desc" : "Date";
            ViewBag.AlmacenSalidaSortParm = String.IsNullOrEmpty(sortOrder) ? "AlmacenSalida_desc" : "AlmacenSalida";
            ViewBag.CrefSortParm = String.IsNullOrEmpty(sortOrder) ? "Cref" : "Cref";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.PresentacionSortParm = String.IsNullOrEmpty(sortOrder) ? "Presentacion" : "Presentacion";
            ViewBag.LoteSortParm = String.IsNullOrEmpty(sortOrder) ? "Lote" : "Lote";
            ViewBag.CantidadSortParm = String.IsNullOrEmpty(sortOrder) ? "Cantidad" : "Cantidad";
            ViewBag.EntregoSortParm = String.IsNullOrEmpty(sortOrder) ? "Entrego" : "Entrego";
            ViewBag.AlmacenEntradaSortParm = String.IsNullOrEmpty(sortOrder) ? "AlmacenEntrada" : "AlmacenEntrada";
            ViewBag.RecibioSortParm = String.IsNullOrEmpty(sortOrder) ? "Recibio" : "Recibio";

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

            //Paginación
            var elementos = from s in dbv.VMovInterAlmacenes
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.IDMovimiento.ToString().Contains(searchString) || s.FechaMovimiento.ToString().Contains(searchString) || s.AlmacenSalida.Contains(searchString) || s.Cref.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Lote.Contains(searchString) || s.Cantidad.ToString().Contains(searchString) || s.Entrego.Contains(searchString) || s.AlmacenEntrada.Contains(searchString) || s.Recibio.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "IDMovimiento":
                    elementos = elementos.OrderByDescending(s => s.IDMovimiento);
                    break;
                case "FechaMovimiento":
                    elementos = elementos.OrderBy(s => s.FechaMovimiento);
                    break;
                case "date_desc":
                    elementos = elementos.OrderBy(s => s.FechaMovimiento);
                    break;
                case "AlmacenSalida":
                    elementos = elementos.OrderBy(s => s.AlmacenSalida);
                    break;
                case "Cref":
                    elementos = elementos.OrderBy(s => s.Cref);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "Presentacion":
                    elementos = elementos.OrderBy(s => s.Presentacion);
                    break;
                case "Lote":
                    elementos = elementos.OrderBy(s => s.Lote);
                    break;
                case "Cantidad":
                    elementos = elementos.OrderBy(s => s.Cantidad);
                    break;
                case "Entrego":
                    elementos = elementos.OrderBy(s => s.Entrego);
                    break;
                case "AlmacenEntrada":
                    elementos = elementos.OrderBy(s => s.AlmacenEntrada);
                    break;
                case "Recibio":
                    elementos = elementos.OrderBy(s => s.Recibio);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDMovimiento);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbv.VMovInterAlmacenes.OrderByDescending(e => e.IDMovimiento).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25", Selected = true },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 25);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        // GET: MovInterAlmacen/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            VMovInterAlmacen movInterAlmacen = dbv.VMovInterAlmacenes.Find(id);
            if (movInterAlmacen == null)
            {
                return HttpNotFound();
            }
            List<VMovInterAlmacen> movimientoList = dbv.Database.SqlQuery<VMovInterAlmacen>("select [IdMovimiento],[FechaMovimiento],[IDAlmacenS],[AlmacenEntrada],[AlmacenSalida],[IDArticulo], [IDCaracteristica],[Cref], [Descripcion], [Presentacion], [Lote], [Cantidad], [IDTrabajadorS], [Entrego], [IDAlmacenE], [IDTrabajadorE], [Recibio], [Observacion] from dbo.VMovInterAlmacen where [IdMovimiento]='" + id + "'").ToList();
            ViewBag.movimiento = movimientoList;

            return View(movInterAlmacen);
        }


        public ActionResult getJsonProductoAlmacen(int id)
        {
            var inventario = new VInventarioAlmacenRepository().GetArticuloxalmacen(id);
            Session["Almacen"] = id;
            return Json(inventario, JsonRequestBehavior.AllowGet);
           

        }

        public ActionResult getJsonCaracteristicaArticuloAlmacen(int id)
        {
            int idalmacen = int.Parse(Session["Almacen"].ToString());
            var inventario = new VInventarioAlmacenRepository().GetListapresentaciones(id, idalmacen);
            return Json(inventario, JsonRequestBehavior.AllowGet); ;

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////7

        public IEnumerable<SelectListItem> getProductoAlmacen(int ida)
        {
            var inventario = new VInventarioAlmacenRepository().GetArticuloxalmacen(ida);
            return inventario;

        } // Fin getProductoAlmacen

        public IEnumerable<SelectListItem> getPresentacioninicial()
        {
            var inventario = new VInventarioAlmacenRepository().Getpresentacioninicial();
            return inventario;

        }
        // GET: MovInterAlmacen/Create
        public ActionResult Create()

        {
            AlmacenContext dba = new AlmacenContext();
            AlmacenistaContext dbt = new AlmacenistaContext();
            MovInterAlmacen elemento = new MovInterAlmacen();
            elemento.FechaMovimiento = DateTime.Now;

            //Almacen de Salida
            var almacenS = dba.Almacenes.ToList();
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

            ////Articulos por Almacen
            //string cadenasql = "Select * from dbo.GetProductoPorAlmacen(" + ida + ") where Descripcion like '" + cadbusqueda + "'%' or Presentacion like '" + cadbusqueda + "'%' ";
            //List<VInventarioAlmacen> listProdAlm = dbv.Database.SqlQuery<VInventarioAlmacen>(cadenasql).ToList();
            //ViewBag.datosProductoAlmacen = listProdAlm;
            //int IDCaracteristica = ViewBag.datosProductoAlmacen.IDCAracteristica;
            //int IDArticulo = ViewBag.datosProductoAlmacen.IDArticulo;

            //Almacen de Entrada
            var almacenE = dba.Almacenes.ToList();
            List<SelectListItem> liaE = new List<SelectListItem>();
            liaE.Add(new SelectListItem { Text = "--Selecciona un Almacen --", Value = "0" });
            foreach (var a in almacenE)
            {
                liaE.Add(new SelectListItem { Text = a.IDAlmacen + " | " + a.CodAlm + " | " + a.Descripcion, Value = a.IDAlmacen.ToString() });

            }
            ViewBag.datosAlmacenE = liaE;
            //Trabajador que entrega
            var trabajadorS = dbt.Almacenistas.ToList().OrderBy(s => s.Nombre);
            List<SelectListItem> litS = new List<SelectListItem>();
            litS.Add(new SelectListItem { Text = "--Selecciona un Almacenista--", Value = "0" });
            foreach (var ts in trabajadorS)
            {
                litS.Add(new SelectListItem { Text = ts.Nombre, Value = ts.IDA.ToString() });
                ViewBag.datosTrabajadorS = litS;
            }

            //Trabajador que recibe
            var trabajadorE = dbt.Almacenistas.ToList().OrderBy(s => s.Nombre);
            List<SelectListItem> litE = new List<SelectListItem>();
            litE.Add(new SelectListItem { Text = "--Selecciona un Almacenista--", Value = "0" });
            foreach (var te in trabajadorE)
            {
                litE.Add(new SelectListItem { Text = te.Nombre, Value = te.IDA.ToString() });
                ViewBag.datosTrabajadorE = litE;
            }


            return View(elemento);
        }

        // POST: MovInterAlmacen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MovInterAlmacen movInterAlmacen, int? idalmacen)
        {
            int idcar = movInterAlmacen.IDCaracteristica;
            VCaracteristicaContext dbc = new VCaracteristicaContext();
            ClsDatoEntero idart = dbc.Database.SqlQuery<ClsDatoEntero>("Select Articulo_IDArticulo as Dato from dbo.Caracteristica where ID = " + idcar + "").ToList()[0];
            ClsDatoEntero idpres = dbc.Database.SqlQuery<ClsDatoEntero>("Select IDPresentacion as Dato from dbo.Caracteristica where ID = " + idcar + "").ToList()[0];
            //DateTime fechamov = DateTime, movInterAlmacen.FechaMovimiento);
            string fecha2 = movInterAlmacen.FechaMovimiento.ToString("yyyy/MM/dd");

            //ViewBag.idalmacen = idalmacen;
            //VInventarioAlmacenContext dbia = new VInventarioAlmacenContext();
            //List<VInventarioAlmacen> invalm = dbia.Database.SqlQuery<VInventarioAlmacen>("SELECT * FROM [dbo].[VInventarioAlmacen]  where IDAlmacen = " + idalmacen + " order by Articulo desc").ToList();
            //ViewBag.datosProductoAlmacen = invalm;


            //// ViewBag.datosProductoAlmacen = listProdAlm;
            //int IDCaracteristica = ViewBag.datosProductoAlmacen.IDCAracteristica;
            //int IDArticulo = ViewBag.datosProductoAlmacen.IDArticulo;

            if (ModelState.IsValid)
            {
                //db.MovInterAlmacenes.Add(movInterAlmacen);  
                //db.SaveChanges();
                //Grava el movimiento de E/S del Almacen en tabla  [dbo].[MovInterAlmacen]
                string comando = "INSERT INTO [dbo].[MovInterAlmacen](FechaMovimiento, IDAlmacenS ,IDArticulo ,IDCaracteristica, Lote, Cantidad, IDTrabajadorS ,IDTrabajadorE, IDAlmacenE, Observacion) values(SYSDATETIME(), " + movInterAlmacen.IDAlmacenS + ", " + idart.Dato + ", " + movInterAlmacen.IDCaracteristica + ", '" + movInterAlmacen.Lote + "', " + movInterAlmacen.Cantidad + ", " + movInterAlmacen.IDTrabajadorS + ", " + movInterAlmacen.IDTrabajadorE + ", " + movInterAlmacen.IDAlmacenE + ", '" + movInterAlmacen.Observacion + "')";
                db.Database.ExecuteSqlCommand(comando);
                ClsDatoEntero idmovalm = dbc.Database.SqlQuery<ClsDatoEntero>("Select max(IDMovimiento) as Dato from dbo.MovInterAlmacen").ToList()[0];
                ClsDatoDecimal exiS = dbc.Database.SqlQuery<ClsDatoDecimal>("select (Existencia - " + movInterAlmacen.Cantidad + ") as Dato from [dbo].[VInventarioAlmacen] where IDAlmacen=" + movInterAlmacen.IDAlmacenS + " and IDArticulo = " + idart.Dato + " and IDCaracteristica = " + movInterAlmacen.IDCaracteristica + "").ToList()[0];


                //Grava el movimiento de Salida del Almacen
                string comando1 = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + movInterAlmacen.IDCaracteristica + "," + idpres.Dato + "," + idart.Dato + ",'Movimiento entre almacenes', " + movInterAlmacen.Cantidad + ", 'Mov Almacen'," + idmovalm.Dato + ",'" + movInterAlmacen.Lote + "', " + movInterAlmacen.IDAlmacenS + ", 'S', " + exiS.Dato + ", '" + movInterAlmacen.Observacion + "', SYSDATETIMEOFFSET ( ))";
                db.Database.ExecuteSqlCommand(comando1);

                //Actualiza la existencia del articulo que salio del almacen
                string comando2 = "update dbo.InventarioAlmacen set Existencia= " + exiS.Dato + " where [IDAlmacen]=" + movInterAlmacen.IDAlmacenS + " and [IDArticulo]= " + idart.Dato + " and [IDCaracteristica]= " + movInterAlmacen.IDCaracteristica + "";
                db.Database.ExecuteSqlCommand(comando2);
                ClsDatoDecimal disp = dbc.Database.SqlQuery<ClsDatoDecimal>("select (Existencia-Apartado) as Dato from [dbo].[VInventarioAlmacen] where IDAlmacen=" + movInterAlmacen.IDAlmacenS + " and IDArticulo = " + idart.Dato + " and IDCaracteristica = " + movInterAlmacen.IDCaracteristica + "").ToList()[0];
                string comando3 = "update dbo.InventarioAlmacen set Disponibilidad= " + disp.Dato + " where [IDAlmacen]=" + movInterAlmacen.IDAlmacenS + " and [IDArticulo]= " + idart.Dato + " and [IDCaracteristica]= " + movInterAlmacen.IDCaracteristica + "";
                db.Database.ExecuteSqlCommand(comando3);

                try
                {
                    //Actualiza la existencia del articulo que entro al almacen
                    InventarioAlmacen datoExiste = dbc.Database.SqlQuery<InventarioAlmacen>("select * from [dbo].[InventarioAlmacen] where IDAlmacen=" + movInterAlmacen.IDAlmacenE + " and IDArticulo = " + idart.Dato + " and IDCaracteristica = " + movInterAlmacen.IDCaracteristica + "").ToList()[0];

                    ClsDatoDecimal exiE = dbc.Database.SqlQuery<ClsDatoDecimal>("select (Existencia + " + movInterAlmacen.Cantidad + ") as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + movInterAlmacen.IDAlmacenE + " and IDArticulo = " + idart.Dato + " and IDCaracteristica = " + movInterAlmacen.IDCaracteristica + "").ToList()[0];
                    string comando4 = "update dbo.InventarioAlmacen set Existencia=" + exiE.Dato + " where [IDAlmacen]=" + movInterAlmacen.IDAlmacenE + " and [IDArticulo]= " + idart.Dato + " and [IDCaracteristica]= " + movInterAlmacen.IDCaracteristica + "";
                    db.Database.ExecuteSqlCommand(comando4);
                    ClsDatoDecimal dispE = dbc.Database.SqlQuery<ClsDatoDecimal>("select (Existencia-Apartado) as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + movInterAlmacen.IDAlmacenE + " and IDArticulo = " + idart.Dato + " and IDCaracteristica = " + movInterAlmacen.IDCaracteristica + "").ToList()[0];
                    string comando5 = "update dbo.InventarioAlmacen set Disponibilidad= " + dispE.Dato + " where [IDAlmacen]=" + movInterAlmacen.IDAlmacenE + " and [IDArticulo]= " + idart.Dato + " and [IDCaracteristica]= " + movInterAlmacen.IDCaracteristica + "";
                    db.Database.ExecuteSqlCommand(comando5);
                    //Grava el movimiento de Entrada al Almacen
                    string comando6 = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + movInterAlmacen.IDCaracteristica + "," + idpres.Dato + "," + idart.Dato + ",'Movimiento entre almacenes', " + movInterAlmacen.Cantidad + ", 'Mov Almacen'," + idmovalm.Dato + ",'" + movInterAlmacen.Lote + "', " + movInterAlmacen.IDAlmacenE + ", 'E', " + exiE.Dato + ", '" + movInterAlmacen.Observacion + "', SYSDATETIMEOFFSET ( ))";
                    db.Database.ExecuteSqlCommand(comando6);


                }
                catch (Exception err)
                {
                    string comando7 = "Insert into dbo.InventarioAlmacen (IDAlmacen, IDArticulo, IDCAracteristica, Existencia, PorLlegar, Apartado, Disponibilidad) values (" + movInterAlmacen.IDAlmacenE + ", " + idart.Dato + ", " + movInterAlmacen.IDCaracteristica + ", " + movInterAlmacen.Cantidad + ",0,0, " + movInterAlmacen.Cantidad + ")";
                    db.Database.ExecuteSqlCommand(comando7);
                    //Grava el movimiento de Entrada al Almacen
                    string comando8 = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + movInterAlmacen.IDCaracteristica + "," + idpres.Dato + "," + idart.Dato + ",'Movimiento entre almacenes', " + movInterAlmacen.Cantidad + ", 'Mov Almacen'," + idmovalm.Dato + ",'" + movInterAlmacen.Lote + "', " + movInterAlmacen.IDAlmacenE + ", 'E', " + movInterAlmacen.Cantidad + ", '" + movInterAlmacen.Observacion + "', SYSDATETIMEOFFSET ( ))";
                    db.Database.ExecuteSqlCommand(comando8);
                    string error = err.Message;
                }


                return RedirectToAction("Index");
            }



            return View(movInterAlmacen);
        }



        

        // GET: MovInterAlmacen/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovInterAlmacen movInterAlmacen = db.MovInterAlmacenes.Find(id);
            if (movInterAlmacen == null)
            {
                return HttpNotFound();
            }
            return View(movInterAlmacen);
        }

        // POST: MovInterAlmacen/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDMovimiento,FechaMovimiento,IDAlmacenS,IDArticulo,IDCaracteristica,Lote,Cantidad,IDTrabajadorS,IDTrabajadorE,IDAlmacenE,Observacion")] MovInterAlmacen movInterAlmacen)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movInterAlmacen).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movInterAlmacen);
        }

        // GET: MovInterAlmacen/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MovInterAlmacen movInterAlmacen = db.MovInterAlmacenes.Find(id);
            if (movInterAlmacen == null)
            {
                return HttpNotFound();
            }
            return View(movInterAlmacen);
        }

        // POST: MovInterAlmacen/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MovInterAlmacen movInterAlmacen = db.MovInterAlmacenes.Find(id);
            db.MovInterAlmacenes.Remove(movInterAlmacen);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

       
        public ActionResult ARMovInterAlPorFecha()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ARMovInterAlPorFecha(ReporteMovInteralPorFecha modelo, FormCollection coleccion)
        {

            string FI = modelo.fechaini.Year.ToString() + "-" + modelo.fechaini.Month.ToString() + "-" + modelo.fechaini.Day.ToString();
            string FF = modelo.fechafin.Year.ToString() + "-" + modelo.fechafin.Month.ToString() + "-" + modelo.fechafin.Day.ToString();


            string cual = coleccion.Get("Enviar");
            if (cual == "Generar reporte")
            {

                ReporteMovInteralPorFecha report = new ReporteMovInteralPorFecha();
                //byte[] abytes = report.PrepareReport(DateTime.Parse("2019-07-01"), DateTime.Parse("2019-07-30"), idcliente);
                byte[] abytes = report.PrepareReport(modelo.fechaini, modelo.fechafin);
                return File(abytes, "application/pdf");
            }
            else
            {

                //Listado de datos
                string cadena = "select * from VMovInterAlmacen where FechaMovimiento >= '" + FI + "' and FechaMovimiento <='" + FF + "'";
                var movimientos = db.Database.SqlQuery<VMovInterAlmacen>(cadena).ToList();
                var mov = movimientos;

                //VMovInterAlmacenContext dbc = new VMovInterAlmacenContext();
                //var movimiento = dbc.VMovInterAlmacenes.ToList();
                ExcelPackage Ep = new ExcelPackage();
                var Sheet = Ep.Workbook.Worksheets.Add("MovimientoInterAlmacen");

                int row = 1;
                //Fijar la fuente para A1:Q1
                Sheet.Cells["A1:L1"].Style.Font.Size = 20;
                Sheet.Cells["A1:L1"].Style.Font.Name = "Calibri";
                Sheet.Cells["A1:L1"].Style.Font.Bold = true;
                Sheet.Cells["A1:L1"].Style.Font.Color.SetColor(Color.DarkBlue);
                Sheet.Cells["A1:L1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                Sheet.Cells["A1"].RichText.Add("Movimiento entre almacenes");
                Sheet.Cells["A1:L1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

                row = 2;
                Sheet.Cells[string.Format("A2", row)].Value = "Fecha inicial";
                Sheet.Cells[string.Format("B2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("B2", row)].Value = FI;
                Sheet.Cells[string.Format("D2", row)].Value = "Fecha Final";
                Sheet.Cells[string.Format("E2", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("E2", row)].Value = FF;

                row = 3;
                Sheet.Cells["A3:L3"].Style.Font.Name = "Calibri";
                Sheet.Cells["A3:L3"].Style.Font.Size = 12;
                Sheet.Cells["A3:L3"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
                Sheet.Cells["A3:L3"].Style.Font.Bold = true;
                Sheet.Cells["A3:L3"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                Sheet.Cells["A3:L3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                Sheet.Cells["A3"].RichText.Add("ID");
                Sheet.Cells["B3"].RichText.Add("FechaMovimiento");
                Sheet.Cells["C3"].RichText.Add("Almacen de Salida");
                Sheet.Cells["D3"].RichText.Add("Cref");
                Sheet.Cells["E3"].RichText.Add("Descripción");
                Sheet.Cells["F3"].RichText.Add("Presentación");
                Sheet.Cells["G3"].RichText.Add("Lote");
                Sheet.Cells["H3"].RichText.Add("Cantidad");
                Sheet.Cells["I3"].RichText.Add("Entrego");
                Sheet.Cells["J3"].RichText.Add("Almacen de Entrada");
                Sheet.Cells["K3"].RichText.Add("Recibio");
                Sheet.Cells["L3"].RichText.Add("Observación");

                row = 4;

                foreach (var item in mov)
                {
                    Sheet.Cells[string.Format("A{0}", row)].Value = item.IDMovimiento;
                    Sheet.Cells[string.Format("C{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.FechaMovimiento;
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.AlmacenSalida;
                    Sheet.Cells[string.Format("D{0}", row)].Value = item.Cref;
                    Sheet.Cells[string.Format("E{0}", row)].Value = item.Descripcion;
                    Sheet.Cells[string.Format("F{0}", row)].Value = item.Presentacion;
                    Sheet.Cells[string.Format("G{0}", row)].Value = item.Lote;
                    Sheet.Cells[string.Format("H{0}", row)].Value = item.Cantidad;
                    Sheet.Cells[string.Format("I{0}", row)].Value = item.Entrego;
                    Sheet.Cells[string.Format("J{0}", row)].Value = item.AlmacenSalida;
                    Sheet.Cells[string.Format("K{0}", row)].Value = item.Recibio;
                    Sheet.Cells[string.Format("L{0}", row)].Value = item.Observacion;

                    row++;
                }
                Sheet.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelMovInterAlmacen.xlsx");
                Response.BinaryWrite(Ep.GetAsByteArray());
                Response.End();
                return Redirect("/blah");
                //return View(modelo);
            }
            //return Redirect("index");
        }




        ///////////////////////INTERALMACEN LOTES
        ///
        public ActionResult InterAlmacenLote(FormCollection coleccion)
        {
            Clslotemp lotemp = new Clslotemp();
            string codigo = "";
            //string Lote = "";
            try
            {
                codigo = coleccion.Get("codigo").ToString();
            }
            catch (Exception err)
            {
                try
                {
                    codigo = coleccion.Get("LoteI").ToString();

                    lotemp = db.Database.SqlQuery<Clslotemp>(" Select * from clslotemp where loteinterno='" + codigo + "'").ToList().FirstOrDefault();

                }
                catch (Exception error)
                {

                }

            }
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            ViewBag.Datos = 0;
            ViewBag.AlmacenP = "";
            ViewBag.Lote = "";
            //ViewBag.IDMateriaP = "";

            string g = coleccion.Get("GuardarMovimiento");
            string trabajadors = coleccion.Get("IDTrabajadorS");
            string trabajadore = coleccion.Get("IDTrabajadorE");
            string AlmacenE = coleccion.Get("IDAlmacenE");
            string observacion = coleccion.Get("Observacion");
            int IDAlmacenEntrada = 0;
            try
            {
                IDAlmacenEntrada = int.Parse(AlmacenE.ToString());
            }
            catch (Exception err)
            {

            }
            string matePrima = "";
            try
            {
                matePrima = coleccion.Get("MateriaP");
            }
            catch (Exception err)
            {

            }
            int IDMateriaPrima = 0;
            try
            {
                IDMateriaPrima = int.Parse(matePrima);
            }
            catch (Exception err)
            {
                try
                {
                    IDMateriaPrima = lotemp.ID;
                }
                catch (Exception error)
                {

                }
            }

            if (g == "Grabar")
            {

                lotemp = db.Database.SqlQuery<Clslotemp>("Select * from clslotemp where id='" + IDMateriaPrima + "'").ToList().FirstOrDefault();

                Caracteristica carateristica = new ValeSalidaContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + lotemp.IDCaracteristica).ToList().FirstOrDefault();
                try
                {
                    string cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia- " + lotemp.MetrosDisponibles + ") WHERE IDAlmacen = " + lotemp.IDAlmacen + " and IDCaracteristica =" + lotemp.IDCaracteristica + "";
                    db.Database.ExecuteSqlCommand(cadenapro);
                    db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia = 0 WHERE existencia<0 and IDAlmacen = " + lotemp.IDAlmacen + " and IDCaracteristica = " + lotemp.IDCaracteristica);
                    db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad=(existencia-apartado) where IDAlmacen=" + lotemp.IDAlmacen + " and idcaracteristica=" + lotemp.IDCaracteristica);

                }
                catch (Exception err)
                {

                }
                try
                {
                    InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == lotemp.IDAlmacen && s.IDCaracteristica == lotemp.IDCaracteristica).FirstOrDefault();

                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + lotemp.IDArticulo + ",'Movimiento de lotes entre almacenes',";
                    cadenam += lotemp.MetrosDisponibles + ",'Movimiento de lotes entre almacenes'," + lotemp.ID + ",'" + lotemp.LoteInterno + "',";
                    cadenam += lotemp.IDAlmacen + ",'S'," + invO.Existencia + ",'', CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";

                    db.Database.ExecuteSqlCommand(cadenam);
                }
                catch (Exception err)
                {

                }

                /////////////////////////
                ///ENTRADA AL NUEVO ALMACEN
                ///
                try
                {
                    InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == IDAlmacenEntrada && s.IDCaracteristica == lotemp.IDCaracteristica).FirstOrDefault();
                    if (invO == null)
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO inventarioAlmacen (IDAlmacen,IDArticulo,IDCaracteristica,Existencia, PorLlegar,Apartado,Disponibilidad) values ("
                               + IDAlmacenEntrada + "," + lotemp.IDArticulo + "," + carateristica.ID + "," + lotemp.MetrosDisponibles + ",0,0," + lotemp.MetrosDisponibles + ")");
                    }
                    else
                    {
                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Existencia = 0 WHERE existencia<0 and IDAlmacen = " + IDAlmacenEntrada + " and IDCaracteristica = " + lotemp.IDCaracteristica);

                        string cadenapro = "UPDATE dbo.InventarioAlmacen SET Existencia = (Existencia+ " + lotemp.MetrosDisponibles + ") WHERE IDAlmacen = " + IDAlmacenEntrada + " and IDCaracteristica =" + lotemp.IDCaracteristica + "";
                        db.Database.ExecuteSqlCommand(cadenapro);
                        db.Database.ExecuteSqlCommand("UPDATE dbo.InventarioAlmacen SET Disponibilidad=(existencia-apartado) where IDAlmacen=" + IDAlmacenEntrada + " and idcaracteristica=" + lotemp.IDCaracteristica);

                    }
                    ////movimiento clslotemp
                    ///
                    try
                    {
                        db.Database.ExecuteSqlCommand("UPDATE dbo.clslotemp SET idalmacen=" + IDAlmacenEntrada + " where id=" + IDMateriaPrima);

                    }
                    catch (Exception err )
                    {

                    }
                }
                catch (Exception err)
                {

                }
                try
                {
                    InventarioAlmacen invO = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == IDAlmacenEntrada && s.IDCaracteristica == lotemp.IDCaracteristica).FirstOrDefault();

                    string cadenam = "INSERT INTO [dbo].[MovimientoArticulo] ([Fecha],[Id],[IDPresentacion],[Articulo_IDArticulo],[Accion],[cantidad],[Documento],[NoDocumento],[Lote],[IDAlmacen],[TipoOperacion],[acumulado],[observacion],[Hora], Usuario) VALUES ";
                    cadenam += " (getdate(), " + carateristica.ID + "," + carateristica.IDPresentacion + "," + lotemp.IDArticulo + ",'Movimiento de lotes entre almacenes',";
                    cadenam += lotemp.MetrosDisponibles + ",'Movimiento de lotes entre almacenes'," + lotemp.ID + ",'" + lotemp.LoteInterno + "',";
                    cadenam += IDAlmacenEntrada + ",'E'," + invO.Existencia + ",'', CONVERT (time, SYSDATETIMEOFFSET())," + usuario + ")";

                    db.Database.ExecuteSqlCommand(cadenam);
                }
                catch (Exception err)
                {

                }
                Articulo articulo = new ArticuloContext().Articulo.Find(lotemp.IDArticulo);
                db.Database.ExecuteSqlCommand("delete from carritoLote where  usuario=" + usuario);
                return RedirectToAction("Index", "InventarioAlmacen", new { SearchString = articulo.Cref });
            }
            else
            {





                bool existe = false;


                try
                {

                    lotemp = db.Database.SqlQuery<Clslotemp>("Select * from clslotemp where MetrosDisponibles>0 and loteinterno='" + codigo + "'").ToList().FirstOrDefault();

                    if (lotemp != null)
                    {
                        existe = true;
                    }
                }
                catch (Exception err)
                {

                }




                try
                {


                    Clslotemp inventariompxcb = new Clslotemp();
                    inventariompxcb = db.Database.SqlQuery<Clslotemp>("Select * from Clslotemp where LoteInterno='" + codigo + "' ").ToList().FirstOrDefault();

                    ViewBag.IDMateriaP = inventariompxcb.ID;


                    new InventarioAlmacenContext().Database.ExecuteSqlCommand("insert into [CarritoLote] (IDMateriaP,Lote,IDAlmacen, MetrosDisponibles, usuario ) values ('" + inventariompxcb.ID + "','" + codigo + "','" + inventariompxcb.IDAlmacen + "'," + inventariompxcb.MetrosDisponibles + "," + usuario + ")");


                }
                catch (Exception err)
                {
                }
                try
                {
                    List<CarritoLote> Lista = db.Database.SqlQuery<CarritoLote>("select * from [dbo].[CarritoLote] where usuario='" + usuario + "'").ToList();
                    CarritoLote lote = new CarritoContext().CarritoLotes.Where(s => s.usuario == usuario).FirstOrDefault();
                    Almacen almacenes = new AlmacenContext().Almacenes.Find(lote.IDAlmacen);
                    int cuenta = Lista.Count();
                    ViewBag.Datos = cuenta;
                    ViewBag.AlmacenP = almacenes.Descripcion;
                    ViewBag.Lote = lote.Lote;
                    ViewBag.MDisponibles = lote.MetrosDisponibles;



                }
                catch (Exception err)
                {

                }

                AlmacenContext dba = new AlmacenContext();
                AlmacenistaContext dbt = new AlmacenistaContext();
                MovInterAlmacen elemento = new MovInterAlmacen();
                elemento.FechaMovimiento = DateTime.Now;




                //Almacen de Entrada
                var almacenE = dba.Almacenes.ToList();
                List<SelectListItem> liaE = new List<SelectListItem>();
                liaE.Add(new SelectListItem { Text = "--Selecciona un Almacen --", Value = "0" });
                foreach (var a in almacenE)
                {
                    liaE.Add(new SelectListItem { Text = a.IDAlmacen + " | " + a.CodAlm + " | " + a.Descripcion, Value = a.IDAlmacen.ToString() });

                }
                ViewBag.datosAlmacenE = liaE;
                //Trabajador que entrega
                var trabajadorS = dbt.Almacenistas.ToList().OrderBy(s => s.Nombre);
                List<SelectListItem> litS = new List<SelectListItem>();
                litS.Add(new SelectListItem { Text = "--Selecciona un Almacenista--", Value = "0" });
                foreach (var ts in trabajadorS)
                {
                    litS.Add(new SelectListItem { Text = ts.Nombre, Value = ts.IDA.ToString() });
                    ViewBag.datosTrabajadorS = litS;
                }

                //Trabajador que recibe
                var trabajadorE = dbt.Almacenistas.ToList().OrderBy(s => s.Nombre);
                List<SelectListItem> litE = new List<SelectListItem>();
                litE.Add(new SelectListItem { Text = "--Selecciona un Almacenista--", Value = "0" });
                foreach (var te in trabajadorE)
                {
                    litE.Add(new SelectListItem { Text = te.Nombre, Value = te.IDA.ToString() });
                    ViewBag.datosTrabajadorE = litE;
                }
                return View(elemento);
            }





        }


        ///////////////movimiento presentaciones
        ///
        // GET: MovInterAlmacen/Create
        public ActionResult CreateMovPresentaciones()

        {
            AlmacenContext dba = new AlmacenContext();
            AlmacenistaContext dbt = new AlmacenistaContext();
            MovInterPresentacion elemento = new MovInterPresentacion();
            elemento.FechaMovimiento = DateTime.Now;

            //Almacen de Salida
            var almacenS = dba.Almacenes.ToList();
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

            ViewBag.Caracacteristicas2 = car;
            //Almacen de Entrada
            var almacenE = dba.Almacenes.ToList();
            List<SelectListItem> liaE = new List<SelectListItem>();
            liaE.Add(new SelectListItem { Text = "--Selecciona un Almacen --", Value = "0" });
            foreach (var a in almacenE)
            {
                liaE.Add(new SelectListItem { Text = a.IDAlmacen + " | " + a.CodAlm + " | " + a.Descripcion, Value = a.IDAlmacen.ToString() });

            }
            ViewBag.datosAlmacenE = liaE;
            ViewBag.InventarioList2 = getProductoAlmacen(0);



            return View(elemento);
        }

        // POST: MovInterAlmacen/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMovPresentaciones(MovInterPresentacion movInterAlmacen, int? idalmacen)
        {
            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int Usuario = userid.Select(s => s.UserID).FirstOrDefault();
            bool existe = false;
            try
            {
                InventarioAlmacen inventario = new AlmacenContext().Database.SqlQuery<InventarioAlmacen>("select*from InventarioAlmacen where IDCaracteristica=" + movInterAlmacen.IDCaracteristica2 + " and IDAlmacen=" + movInterAlmacen.IDAlmacenE).ToList().FirstOrDefault();

                if (inventario != null)
                {
                    existe = true;
                }
            }
            catch (Exception err)
            {

            }
            int idcar = movInterAlmacen.IDCaracteristica;
            VCaracteristicaContext dbc = new VCaracteristicaContext();
            ClsDatoEntero idart = dbc.Database.SqlQuery<ClsDatoEntero>("Select Articulo_IDArticulo as Dato from dbo.Caracteristica where ID = " + idcar + "").ToList().FirstOrDefault();
            //ClsDatoEntero idpres = dbc.Database.SqlQuery<ClsDatoEntero>("Select IDPresentacion as Dato from dbo.Caracteristica where ID = " + idcar + "").ToList().FirstOrDefault();
            //DateTime fechamov = DateTime, movInterAlmacen.FechaMovimiento);
            Caracteristica caracteristica1 = new AlmacenContext().Database.SqlQuery<Caracteristica>("select*from Caracteristica where ID=" + movInterAlmacen.IDCaracteristica).ToList().FirstOrDefault();
            Caracteristica caracteristica2 = new AlmacenContext().Database.SqlQuery<Caracteristica>("select*from Caracteristica where ID=" + movInterAlmacen.IDCaracteristica2).ToList().FirstOrDefault();

            string fecha2 = movInterAlmacen.FechaMovimiento.ToString("yyyy/MM/dd");


            try
            {
                string comando = "INSERT INTO [dbo].[MovInterPresentacion](FechaMovimiento, IDAlmacenS ,IDArticulo ,IDCaracteristica, Lote, Cantidad, IDAlmacenE, IDCaracteristica2, Observacion,Usuario) " +
                    "values(SYSDATETIME(), " + movInterAlmacen.IDAlmacenS + ", " + movInterAlmacen.IDArticulo + ", " + movInterAlmacen.IDCaracteristica + ", '" + movInterAlmacen.Lote + "', " + movInterAlmacen.Cantidad + ", "
                    + movInterAlmacen.IDAlmacenE + "," + movInterAlmacen.IDCaracteristica2 + ",'" + movInterAlmacen.Observacion + "'," + Usuario  + ")";
                db.Database.ExecuteSqlCommand(comando);

                ClsDatoEntero idmovalm = dbc.Database.SqlQuery<ClsDatoEntero>("Select max(IDMovimiento) as Dato from dbo.MovInterAlmacen").ToList().FirstOrDefault();
                ClsDatoDecimal exiS = dbc.Database.SqlQuery<ClsDatoDecimal>("select (Existencia - " + movInterAlmacen.Cantidad + ") as Dato from [dbo].[VInventarioAlmacen] where IDAlmacen=" + movInterAlmacen.IDAlmacenS + " and IDArticulo = " + movInterAlmacen.IDArticulo + " and IDCaracteristica = " + movInterAlmacen.IDCaracteristica + "").ToList().FirstOrDefault();


                //Grava el movimiento de Salida del Almacen
                string comando1 = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE(),"
                    + movInterAlmacen.IDCaracteristica + "," + caracteristica1.IDPresentacion + "," + movInterAlmacen.IDArticulo + ",'Movimiento presentación entre almacenes', " + movInterAlmacen.Cantidad + ", 'Mov Almacen'," + idmovalm.Dato + ",'" + movInterAlmacen.Lote + "', " + movInterAlmacen.IDAlmacenS + ", 'S', " + exiS.Dato + ", '" + movInterAlmacen.Observacion + "', SYSDATETIMEOFFSET ( ))";
                db.Database.ExecuteSqlCommand(comando1);

                string comando2 = "update dbo.InventarioAlmacen set Existencia= " + exiS.Dato + " where [IDAlmacen]=" + movInterAlmacen.IDAlmacenS + "  and [IDCaracteristica]= " + movInterAlmacen.IDCaracteristica + "";
                db.Database.ExecuteSqlCommand(comando2);


                ClsDatoDecimal disp = dbc.Database.SqlQuery<ClsDatoDecimal>("select (Existencia-Apartado) as Dato from [dbo].[VInventarioAlmacen] where IDAlmacen=" + movInterAlmacen.IDAlmacenS + "  and IDCaracteristica = " + movInterAlmacen.IDCaracteristica + "").ToList().FirstOrDefault();
                string comando3 = "update dbo.InventarioAlmacen set Disponibilidad= " + disp.Dato + " where [IDAlmacen]=" + movInterAlmacen.IDAlmacenS + " and [IDArticulo]= " + idart.Dato + " and [IDCaracteristica]= " + movInterAlmacen.IDCaracteristica + "";
                db.Database.ExecuteSqlCommand(comando3);

                try
                {
                    if (existe)
                    {
                        ClsDatoDecimal exiE = dbc.Database.SqlQuery<ClsDatoDecimal>("select (Existencia + " + movInterAlmacen.Cantidad + ") as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + movInterAlmacen.IDAlmacenE + " and IDCaracteristica = " + movInterAlmacen.IDCaracteristica2 + "").ToList().FirstOrDefault();
                        string comando4 = "update dbo.InventarioAlmacen set Existencia=" + exiE.Dato + " where [IDAlmacen]=" + movInterAlmacen.IDAlmacenE + " and [IDCaracteristica]= " + movInterAlmacen.IDCaracteristica2 + "";
                        db.Database.ExecuteSqlCommand(comando4);
                        ClsDatoDecimal dispE = dbc.Database.SqlQuery<ClsDatoDecimal>("select (Existencia-Apartado) as Dato from [dbo].[InventarioAlmacen] where IDAlmacen=" + movInterAlmacen.IDAlmacenE + " and IDCaracteristica = " + movInterAlmacen.IDCaracteristica2 + "").ToList().FirstOrDefault();
                        string comando5 = "update dbo.InventarioAlmacen set Disponibilidad= " + dispE.Dato + " where [IDAlmacen]=" + movInterAlmacen.IDAlmacenE + "  and [IDCaracteristica]= " + movInterAlmacen.IDCaracteristica2 + "";
                        db.Database.ExecuteSqlCommand(comando5);
                        //Grava el movimiento de Entrada al Almacen
                        string comando6 = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + movInterAlmacen.IDCaracteristica2 + "," + caracteristica2.IDPresentacion + "," + caracteristica2.Articulo_IDArticulo + ",'Movimiento presentación entre almacenes', " + movInterAlmacen.Cantidad + ", 'Mov Almacen'," + idmovalm.Dato + ",'" + movInterAlmacen.Lote + "', " + movInterAlmacen.IDAlmacenE + ", 'E', " + exiE.Dato + ", '" + movInterAlmacen.Observacion + "', SYSDATETIMEOFFSET ( ))";
                        db.Database.ExecuteSqlCommand(comando6);
                    }
                    else
                    {
                        string comando7 = "Insert into dbo.InventarioAlmacen (IDAlmacen, IDArticulo, IDCAracteristica, Existencia, PorLlegar, Apartado, Disponibilidad) values (" + movInterAlmacen.IDAlmacenE + ", " + caracteristica2.Articulo_IDArticulo + ", " + movInterAlmacen.IDCaracteristica2 + ", " + movInterAlmacen.Cantidad + ",0,0, " + movInterAlmacen.Cantidad + ")";
                        db.Database.ExecuteSqlCommand(comando7);
                        //Grava el movimiento de Entrada al Almacen
                        string comando8 = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + movInterAlmacen.IDCaracteristica2 + "," + caracteristica2.IDPresentacion + "," + caracteristica2.Articulo_IDArticulo + ",'Movimiento presentación entre almacenes', " + movInterAlmacen.Cantidad + ", 'Mov Almacen'," + idmovalm.Dato + ",'" + movInterAlmacen.Lote + "', " + movInterAlmacen.IDAlmacenE + ", 'E', " + movInterAlmacen.Cantidad + ", '" + movInterAlmacen.Observacion + "', SYSDATETIMEOFFSET ( ))";
                        db.Database.ExecuteSqlCommand(comando8);
                    }


                }
                catch (Exception err)
                {
                    string comando7 = "Insert into dbo.InventarioAlmacen (IDAlmacen, IDArticulo, IDCAracteristica, Existencia, PorLlegar, Apartado, Disponibilidad) values (" + movInterAlmacen.IDAlmacenE + ", " + idart.Dato + ", " + movInterAlmacen.IDCaracteristica2 + ", " + movInterAlmacen.Cantidad + ",0,0, " + movInterAlmacen.Cantidad + ")";
                    db.Database.ExecuteSqlCommand(comando7);
                    //Grava el movimiento de Entrada al Almacen
                    string comando8 = "insert into[dbo].[MovimientoArticulo]([Fecha],[Id],[IDPresentacion], [Articulo_IDArticulo], [Accion], [cantidad], [Documento],[NoDocumento], [Lote], [IDAlmacen], [TipoOperacion], [acumulado], [observacion], [Hora]) values(GETDATE()," + movInterAlmacen.IDCaracteristica2 + "," + caracteristica2.IDPresentacion + "," + caracteristica2.Articulo_IDArticulo + ",'Movimiento presentación entre almacenes', " + movInterAlmacen.Cantidad + ", 'Mov Almacen'," + idmovalm.Dato + ",'" + movInterAlmacen.Lote + "', " + movInterAlmacen.IDAlmacenE + ", 'E', " + movInterAlmacen.Cantidad + ", '" + movInterAlmacen.Observacion + "', SYSDATETIMEOFFSET ( ))";
                    db.Database.ExecuteSqlCommand(comando8);
                    string error = err.Message;
                }
                return RedirectToAction("IndexPresentaciones");
            }
            catch (Exception err)
            {

            }



            return View(movInterAlmacen);
        }
        public ActionResult getJsonCaracteristicaArticuloAlmacen1(int id)
        {
            int idalmacen = int.Parse(Session["Almacen"].ToString());
            var inventario = new RepositoryPresentaciones().GetListapresentaciones(id, idalmacen);

            Session["Articulo"] = id;
            return Json(inventario, JsonRequestBehavior.AllowGet); ;

        }
        public ActionResult getJsonCaracteristicaArticuloAlmacen2(int id, FormCollection collection)
        {
            int articulo = int.Parse(Session["Articulo"].ToString());
            int IDAlmacenE = id;
            var inventario = new RepositoryPresentaciones().GetListapresentaciones2(articulo, IDAlmacenE);
            return Json(inventario, JsonRequestBehavior.AllowGet); ;

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////7


        public ViewResult IndexPresentaciones(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //return View(dbv.VMovInterAlmacenes.ToList());
            ViewBag.CurrentSort = sortOrder;
            ViewBag.IDMovimientoSortParm = String.IsNullOrEmpty(sortOrder) ? "IDMovimiento" : "";
            ViewBag.FechaMovimientoSortParm = sortOrder == "FechaMovimiento" ? "date_desc" : "Date";
            ViewBag.AlmacenSalidaSortParm = String.IsNullOrEmpty(sortOrder) ? "AlmacenSalida_desc" : "AlmacenSalida";
            ViewBag.CrefSortParm = String.IsNullOrEmpty(sortOrder) ? "Cref" : "Cref";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.PresentacionSortParm = String.IsNullOrEmpty(sortOrder) ? "Presentacion" : "Presentacion";
            ViewBag.LoteSortParm = String.IsNullOrEmpty(sortOrder) ? "Lote" : "Lote";
            ViewBag.CantidadSortParm = String.IsNullOrEmpty(sortOrder) ? "Cantidad" : "Cantidad";
            ViewBag.EntregoSortParm = String.IsNullOrEmpty(sortOrder) ? "Entrego" : "Entrego";
            ViewBag.AlmacenEntradaSortParm = String.IsNullOrEmpty(sortOrder) ? "AlmacenEntrada" : "AlmacenEntrada";
            ViewBag.RecibioSortParm = String.IsNullOrEmpty(sortOrder) ? "Usuario" : "Usuario";

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

            //Paginación
            var elementos = from s in dbv.vMovInterPresentaciones
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.IDMovimiento.ToString().Contains(searchString) || s.FechaMovimiento.ToString().Contains(searchString) || s.AlmacenSalida.Contains(searchString) || s.Cref.Contains(searchString) || s.Descripcion.Contains(searchString) || s.Lote.Contains(searchString) || s.Cantidad.ToString().Contains(searchString)  || s.AlmacenEntrada.Contains(searchString) || s.Usuario.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "IDMovimiento":
                    elementos = elementos.OrderByDescending(s => s.IDMovimiento);
                    break;
                case "FechaMovimiento":
                    elementos = elementos.OrderBy(s => s.FechaMovimiento);
                    break;
                case "date_desc":
                    elementos = elementos.OrderBy(s => s.FechaMovimiento);
                    break;
                case "AlmacenSalida":
                    elementos = elementos.OrderBy(s => s.AlmacenSalida);
                    break;
                case "Cref":
                    elementos = elementos.OrderBy(s => s.Cref);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "Presentacion":
                    elementos = elementos.OrderBy(s => s.Presentacion);
                    break;
                case "Lote":
                    elementos = elementos.OrderBy(s => s.Lote);
                    break;
                case "Cantidad":
                    elementos = elementos.OrderBy(s => s.Cantidad);
                    break;
              
                case "AlmacenEntrada":
                    elementos = elementos.OrderBy(s => s.AlmacenEntrada);
                    break;
                case "Usuario":
                    elementos = elementos.OrderBy(s => s.Usuario);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.IDMovimiento);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbv.VMovInterAlmacenes.OrderByDescending(e => e.IDMovimiento).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25", Selected = true },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todos", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 25);
            ViewBag.psize = pageSize;

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

    }
}
public class RepositoryPresentaciones
{
    public IEnumerable<SelectListItem> GetListapresentaciones(int id, int idalmacen)
    {
        using (var context = new ArticuloContext())
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            string cadena = "select * from VInventarioAlmacen where IDArticulo=" + id + " and Existencia >0 and IdAlmacen=" + idalmacen;
            List<VInventarioAlmacen> caracteristicas = context.Database.SqlQuery<VInventarioAlmacen>(cadena).ToList();

            foreach (VInventarioAlmacen cara in caracteristicas)
            {
                
                Caracteristica caract = new ArticuloContext().Database.SqlQuery<Caracteristica>("select*from caracteristica where id="+ cara.IDCaracteristica).ToList().FirstOrDefault();
                var descripciontip2 = new SelectListItem()
                {
                    
                    Value = cara.IDCaracteristica.ToString(),
                    Text = "NP: "+caract.IDPresentacion + " | " +cara.Presentacion + " | Existencia " + cara.Existencia
                };
                lista.Insert(0, descripciontip2);

            }
            var descripciontip = new SelectListItem()
            {
                Value = "0",
                Text = "--- Selecciona una presentacion ---"
            };
            lista.Insert(0, descripciontip);
            return new SelectList(lista, "Value", "Text");
        }
    }
    public IEnumerable<SelectListItem> GetListapresentaciones2(int idC, int idalmacen)
    {
        using (var context = new ArticuloContext())
        {
            List<SelectListItem> lista = new List<SelectListItem>();
            string cadena = "select * from VInventarioAlmacen where IDArticulo=" + idC + "  and IdAlmacen=" + idalmacen;
            List<VInventarioAlmacen> caracteristicas = context.Database.SqlQuery<VInventarioAlmacen>(cadena).ToList();

            foreach (VInventarioAlmacen cara in caracteristicas)
            {
                Caracteristica caract = new ArticuloContext().Database.SqlQuery<Caracteristica>("select*from caracteristica where id=" + cara.IDCaracteristica).ToList().FirstOrDefault();
                var descripciontip2 = new SelectListItem()
                {

                    Value = cara.IDCaracteristica.ToString(),
                    Text = "NP: " + caract.IDPresentacion + " | " + cara.Presentacion + " | Existencia " + cara.Existencia
                };
           
                lista.Insert(0, descripciontip2);

            }
            var descripciontip = new SelectListItem()
            {
                Value = "0",
                Text = "--- Selecciona una presentacion ---"
            };
            lista.Insert(0, descripciontip);
            return new SelectList(lista, "Value", "Text");
        }
    }
}