using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using SIAAPI.Models.CartaPorte;
using SIAAPI.Reportes;

namespace SIAAPI.Controllers.CartaPorte
{
    public class c_ConfigAutotransporteController : Controller
    {
        private c_ConfigAutotransporteDBContext db = new c_ConfigAutotransporteDBContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveNom" : "";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripción" : "Descripción";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "NoEjes" : "NoEjes";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "NoLlantas" : "NoLlantas";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Fecha_In" : "Fecha_In";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Fecha_Fin" : "Fecha_Fin";
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
            var elementos = from s in db.ConfigAutotransporte
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.ClaveNom.Contains(searchString) || s.Descripcion.Contains(searchString) || s.NoEjes.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveNom":
                    elementos = elementos.OrderBy(s => s.ClaveNom);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "NoEjes":
                    elementos = elementos.OrderBy(s => s.NoEjes);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveNom);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.ConfigAutotransporte.OrderBy(e => e.IDConfAutoT).Count(); // Total number of elements

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

            //return View(elementos.ToPagedList(pageNumber, pageSize));
            return View(elementos.ToPagedList(pageNumber, pageSize));
            //Paginación
        }

        // GET: c_ConfigAutotransporte
        //public ActionResult Index()
        //{
        //    return View(db.c_ConfigAutotransporte.ToList());
        //}

        // GET: c_ConfigAutotransporte/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ConfigAutotransporte c_ConfigAutotransporte = db.ConfigAutotransporte.Find(id);
            if (c_ConfigAutotransporte == null)
            {
                return HttpNotFound();
            }
            return View(c_ConfigAutotransporte);
        }

        // GET: c_ConfigAutotransporte/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_ConfigAutotransporte/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ClaveNom,Descripcion,NoEjes,NoLlantas,Fecha_In,Fecha_Fin")] c_ConfigAutotransporte c_ConfigAutotransporte)
        {
            if (ModelState.IsValid)
            {
                db.ConfigAutotransporte.Add(c_ConfigAutotransporte);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_ConfigAutotransporte);
        }

        // GET: c_ConfigAutotransporte/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.ConfigAutotransporte.Single(m => m.IDConfAutoT == id);
            return View(elemento);
            
        }

        // POST: c_ConfigAutotransporte/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.ConfigAutotransporte.Single(m => m.IDConfAutoT == id);
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

        // GET: c_ConfigAutotransporte/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_ConfigAutotransporte c_ConfigAutotransporte = db.ConfigAutotransporte.Find(id);
            if (c_ConfigAutotransporte == null)
            {
                return HttpNotFound();
            }
            return View(c_ConfigAutotransporte);
        }

        // POST: c_ConfigAutotransporte/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_ConfigAutotransporte c_ConfigAutotransporte = db.ConfigAutotransporte.Find(id);
            db.ConfigAutotransporte.Remove(c_ConfigAutotransporte);
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

        public void GenerarExcelConfig()
        {
            //Listado de datos
            var config = db.ConfigAutotransporte.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("ConfiguracionAutotransportre");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:G1"].Style.Font.Size = 20;
            Sheet.Cells["A1:G1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:G1"].Style.Font.Bold = true;
            Sheet.Cells["A1:G1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:G1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado Configuración Autotransporte Federal");
            Sheet.Cells["A1:G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:G2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:G2"].Style.Font.Size = 12;
            Sheet.Cells["A2:G2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:G2"].Style.Font.Bold = true;

            Sheet.Cells["B2"].RichText.Add("Clave");
            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("No. Ejes");
            Sheet.Cells["E2"].RichText.Add("No. Llantas");
            Sheet.Cells["F2"].RichText.Add("Fecha Inicio");
            Sheet.Cells["G2"].RichText.Add("Fecha Fin");

            row = 3;
            foreach (var item in config)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDConfAutoT;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClaveNom;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.NoEjes;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.NoLlantas;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.Fecha_In;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.Fecha_Fin;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelConfigAuto.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

        public ActionResult ListadoConfigAuto()
        {
            var config = db.ConfigAutotransporte.ToList();
            Reportec_ConfigAutotransporte report = new Reportec_ConfigAutotransporte();
            report.ConfigAuto = config;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteConfigAuto.pdf");
        }

    }

}
