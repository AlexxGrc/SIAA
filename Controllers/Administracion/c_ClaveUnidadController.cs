
using PagedList;
using SIAAPI.Models.Administracion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;



namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class c_ClaveUnidadController : Controller
    {
        private c_ClaveUnidadContext db = new c_ClaveUnidadContext();
        // GET: c_ClaveUnidad
      
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //var lista = from e in db.c_ClaveUnidades
            //            orderby e.IDClaveUnidad
            //            select e;
            //return View(lista);
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveProdServ" : "ClaveProdServ";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "Nombre";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.c_ClaveUnidades
                           
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.ClaveUnidad.Contains(searchString) || s.Nombre.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveProdServ":
                    elementos = elementos.OrderBy(s => s.ClaveUnidad);
                    break;
                case "Nombre":
                    elementos = elementos.OrderBy(s => s.Nombre);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Nombre);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_ClaveUnidades.OrderBy(e => e.IDClaveUnidad).Count(); // Total number of elements

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

        // GET: c_ClaveUnidad/Details/5
      
        public ActionResult Details(int id)
        {
            var elemento = db.c_ClaveUnidades.Single(m => m.IDClaveUnidad == id);
            return View(elemento);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
       

        public ActionResult Details(int id, FormCollection collection)
        {
            var elemento = db.c_ClaveUnidades.Single(m => m.IDClaveUnidad == id);
            return View(elemento);

        }

        // GET: c_ClaveUnidad/Create
    
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_ClaveUnidad/Create
      
        [HttpPost]
        public ActionResult Create(c_ClaveUnidad elemento)
        {
            try
            {
                // TODO: Add insert logic here

                var db = new c_ClaveUnidadContext();
                db.c_ClaveUnidades.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception error)
            {
                return View();
            }
        }

        // GET: c_ClaveUnidad/Edit/5
     
        public ActionResult Edit(int id)
        {
            var elemento = db.c_ClaveUnidades.Single(m => m.IDClaveUnidad == id);
            return View(elemento);

        }

        // POST: c_ClaveUnidad/Edit/5
      
        [HttpPost]
        public ActionResult Edit(int id, c_ClaveUnidad collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.c_ClaveUnidades.Single(m => m.IDClaveUnidad == id);
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

        // GET: /Delete/5
       
        public ActionResult Delete(int id)
        {
            var elemento = db.c_ClaveUnidades.Single(m => m.IDClaveUnidad == id);
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
                var elemento = db.c_ClaveUnidades.Single(m => m.IDClaveUnidad == id);
                db.c_ClaveUnidades.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

       
       
        public ActionResult ListadoClaveUnidad()
        {
            c_ClaveUnidadContext dba = new c_ClaveUnidadContext();
            var clv = dba.c_ClaveUnidades.ToList();
            ReporteClaveUnidad report = new ReporteClaveUnidad();
            report.claveUnidad = clv;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteClaveUnidad.pdf");
        }


        public void GenerarExcelClaveUnidad()
        {
            //Listado de datos
            c_ClaveUnidadContext dba = new c_ClaveUnidadContext();
            var claveU = dba.c_ClaveUnidades.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("ClaveUnidad");
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
            Sheet.Cells["A1"].RichText.Add("Catalogo: Clave Unidad");

            row = 2;
            //Fijar la fuente para EL RANGO DE CELDAS A2:B2
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 12;
            Sheet.Cells["A2:B2"].Style.Font.Bold = true;
            Sheet.Cells["A2:B2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:B2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            //Sheet.Cells["A1"].Value = "Clave";
            //Sheet.Cells["B1"].Value = "Descripción";
            Sheet.Cells["A2"].RichText.Add("Clave Unidad");
            Sheet.Cells["B2"].RichText.Add("Nombre");
            Sheet.Cells["A2:B2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            row = 3;
            foreach (var item in claveU)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.ClaveUnidad;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteClaveUnidadExcel.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
