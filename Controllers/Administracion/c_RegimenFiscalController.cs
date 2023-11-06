
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
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_RegimenFiscalController : Controller
    {
        private c_RegimenFiscalContext db = new c_RegimenFiscalContext();
        // GET: c_RegimenFiscal
      
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //var lista =  from e in db.c_RegimenFiscales
            //             orderby e.IDRegimenFiscal
            //             select e;
            //return View(lista);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveRegimenFiscal" : "ClaveRegimenFiscal";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.c_RegimenFiscales
                            select s;
            
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveRegimenFiscal.ToString().Contains(searchString) || s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveProdServ":
                    elementos = elementos.OrderBy(s => s.IDRegimenFiscal);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.IDRegimenFiscal);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_RegimenFiscales.OrderBy(e => e.IDRegimenFiscal).Count(); // Total number of elements

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
        }

        // GET: c_RegimenFiscal/Details/5
      
        public ActionResult Details(int id)
        {
            var elemento = db.c_RegimenFiscales.Single(m => m.IDRegimenFiscal == id);
            return View(elemento);
        }

        [HttpPost]
       
        [ValidateAntiForgeryToken ]
        public ActionResult Details(int id, c_ClaveProductoServicio collection)
        {
            var elemento = db.c_RegimenFiscales.Single(m => m.IDRegimenFiscal == id);
            return View(elemento);
        }

        // GET: c_RegimenFiscal/Create
      
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_RegimenFiscal/Create
        [HttpPost]
       
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_RegimenFiscal elemento)
        {
            try
            {
                var db = new c_RegimenFiscalContext();
                    db.c_RegimenFiscales.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: c_RegimenFiscal/Edit/5
       
        public ActionResult Edit(int id)
        {
            var elemento = db.c_RegimenFiscales.Single(m => m.IDRegimenFiscal == id);
            return View(elemento);
        }


        // POST: c_RegimenFiscal/Edit/5
       
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.c_RegimenFiscales.Single(m => m.IDRegimenFiscal == id);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return RedirectToAction(elemento);
            }
            catch
            {
                return View();
            }
        }
       
        private ActionResult RedirectToAction(c_RegimenFiscal elemento)
        {
            throw new NotImplementedException();
        }

        // GET: /Delete/5
      
        public ActionResult Delete(int id)
        {
            var elemento = db.c_RegimenFiscales.Single(m => m.IDRegimenFiscal == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }


        // POST: /Delete/5
       
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.c_RegimenFiscales.Single(m => m.IDRegimenFiscal == id);
                db.c_RegimenFiscales.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
       
        public ActionResult ListadoRegimenFiscal()
        {
            c_RegimenFiscalContext dba = new c_RegimenFiscalContext();

            var regimenfiscal = dba.c_RegimenFiscales.ToList();
            ReporteRegimenFiscal report = new ReporteRegimenFiscal();
            report.Regimen = regimenfiscal;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteRegimenFiscal.pdf");
        }

        public void GenerarExcelRegimenFiscal()
        {
            //Listado de datos
            c_RegimenFiscalContext dba = new c_RegimenFiscalContext();
            var regimenfiscal = dba.c_RegimenFiscales.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("RegimenFiscal");
            // en la fila1 formateo las celdas y coloco el título de la hoja
            // RichText permite agregar las propiedades de tipo de letra: color, negrita, etc.
            int row = 1;
            //Fijar la fuente para EL RANGO DE CELDAS A1:B1
            Sheet.Cells["A1:B1"].Style.Font.Size = 20;
            Sheet.Cells["A1:B1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:B1"].Style.Font.Bold = true;
            Sheet.Cells["A1:B1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1:B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;
            Sheet.Cells["A1"].RichText.Add("Catalogo: Régimen Fiscal");

            row = 2;
            //Fijar la fuente para EL RANGO DE CELDAS A2:B2
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 12;
            Sheet.Cells["A2:B2"].Style.Font.Bold = true;
            Sheet.Cells["A2:B2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:B2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            //Sheet.Cells["A1"].Value = "Clave";
            //Sheet.Cells["B1"].Value = "Descripción";
            Sheet.Cells["A2"].RichText.Add("Clave");
            Sheet.Cells["B2"].RichText.Add("Descripción");
            Sheet.Cells["A2:B2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            row = 3;
            foreach (var item in regimenfiscal)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ClaveRegimenFiscal;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Descripcion;


                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelRegimenFiscal.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }

}