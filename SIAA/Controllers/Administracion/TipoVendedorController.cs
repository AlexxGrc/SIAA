
using PagedList;
using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador,Gerencia, Comercial,Sistemas,AdminProduccion, Compras, Almacenista, Ventas, Produccion")]
    public class TipoVendedorController : Controller
    {
        private TipoVendedorContext db = new TipoVendedorContext();
        // GET: TipoVendedor
      
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : sortOrder;
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.TipoVendedores
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.DescripcionVendedor.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.DescripcionVendedor);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.DescripcionVendedor);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.TipoVendedores.OrderBy(e => e.IDTipoVendedor).Count(); // Total number of elements

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

            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }
        //public ActionResult Index()
        //{
        //    var lista = from e in db.TipoVendedores
        //                orderby e.IDTipoVendedor
        //                select e;
        //    return View(lista);
        //}

        // GET: TipoVendedor/Details/5
      
        public ActionResult Details(int id)
        {
            var elemento = db.TipoVendedores.Single(m => m.IDTipoVendedor == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

        // GET: TipoVendedor/Create
      
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoVendedor/Create
       
        [HttpPost]
        public ActionResult Create(TipoVendedor elemento)
        {
            try
            {
                // TODO: Add insert logic here

               // var db = new TipoVendedorContext();
                db.TipoVendedores.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TipoVendedor/Edit/5
       
        public ActionResult Edit(int id)
        {
            var elemento = db.TipoVendedores.Single(m => m.IDTipoVendedor == id);
            return View(elemento);
        }

        // POST: TipoVendedor/Edit/5
      
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                var elemento = db.TipoVendedores.Single(m => m.IDTipoVendedor== id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(elemento);
            }
            catch
            {
                return View();
            }
        }

        // GET: TipoVendedor/Delete/5
       
        public ActionResult Delete(int id)
        {
            var elemento = db.TipoVendedores.Single(m => m.IDTipoVendedor == id);
            ViewBag.Mensaje = "";
            return View(elemento);
        }

        // POST: TipoVendedor/Delete/5
      
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var db = new TipoVendedorContext();
                var elemento = db.TipoVendedores.Single(m => m.IDTipoVendedor == id);
                db.TipoVendedores.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch(Exception err)
            {
                ViewBag.Mensaje = err.InnerException.InnerException.Message.Contains("FK") ? "No puedo eliminarlo por que siendo usado " : err.InnerException.InnerException.Message.ToString()  ;
                return View();
            }
        }
      
        public ActionResult ListaTipoVendedor()
        {
            TipoVendedorContext db = new TipoVendedorContext();
            var tipovendedor = db.TipoVendedores.ToList();
            RprtLstTipoVendedor report = new RprtLstTipoVendedor();
            report.ListaDatosBD_TipoVendedor = tipovendedor;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "application/pdf", "ReporteTiposVendedor.pdf");
        }

        public void GenerarExcelTipoVend()
        {
            //Listado de datos
            TipoVendedorContext db = new TipoVendedorContext();
            var tipovendedor = db.TipoVendedores.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("TipoVendedores");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:B1"].Style.Font.Size = 20;
            Sheet.Cells["A1:B1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:B1"].Style.Font.Bold = true;
            Sheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Tipo de Vendedores");
            Sheet.Cells["A1:B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:B2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:B2"].Style.Font.Size = 12;
            Sheet.Cells["A2:B2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:B2"].Style.Font.Bold = true;
            Sheet.Cells["A2:B2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:B2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["B2"].RichText.Add("Descripción");


            row = 3;
            foreach (var item in tipovendedor)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDTipoVendedor;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.DescripcionVendedor;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelTipoVendedor.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
