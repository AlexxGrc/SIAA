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
    public class c_MaterialPeligrosoController : Controller
    {
        private c_MaterialPeligrosoDBContext db = new c_MaterialPeligrosoDBContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveMatPSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveMatP" : "ClaveMatP";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaseDiv" : "ClaseDiv";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "PeligSec" : "PeligSec";
            ViewBag.EnvaseSortParm = String.IsNullOrEmpty(sortOrder) ? "GrupoEmb_Env" : "GrupoEmb_Env";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "DispEsp" : "DispEsp";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "CantLim" : "CantLim";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "CantExp" : "CantExp";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Embalaje" : "Embalaje";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "RIG" : "RIG";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Cisternas" : "Cisternas";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "Contenedores" : "Contenedores";
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
            var elementos = from s in db.MaterialP
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.ClaveMatP.Contains(searchString) || s.Descripcion.Contains(searchString) || s.ClaseDiv.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveNom":
                    elementos = elementos.OrderBy(s => s.ClaveMatP);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                case "NoEjes":
                    elementos = elementos.OrderBy(s => s.ClaseDiv);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveMatP);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.MaterialP.OrderBy(e => e.IDMatP).Count(); // Total number of elements

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

        // GET: c_MaterialPeligroso/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_MaterialPeligroso c_MaterialPeligroso = db.MaterialP.Find(id);
            if (c_MaterialPeligroso == null)
            {
                return HttpNotFound();
            }
            return View(c_MaterialPeligroso);
        }

        // GET: c_MaterialPeligroso/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_MaterialPeligroso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_MaterialPeligroso c_MaterialPeligroso)
        {
            if (ModelState.IsValid)
            {
                db.MaterialP.Add(c_MaterialPeligroso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(c_MaterialPeligroso);
        }

        // GET: c_MaterialPeligroso/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.MaterialP.Single(m => m.IDMatP == id);
            return View(elemento);
        }

        // POST: c_MaterialPeligroso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection formCollection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.MaterialP.Single(m => m.IDMatP == id);
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

        // GET: c_MaterialPeligroso/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            c_MaterialPeligroso c_MaterialPeligroso = db.MaterialP.Find(id);
            if (c_MaterialPeligroso == null)
            {
                return HttpNotFound();
            }
            return View(c_MaterialPeligroso);
        }

        // POST: c_MaterialPeligroso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            c_MaterialPeligroso c_MaterialPeligroso = db.MaterialP.Find(id);
            db.MaterialP.Remove(c_MaterialPeligroso);
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


        public void MaterialPeligrosoE()
        {
            //Listado de datos
            var bancos = db.MaterialP.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("MaterialPeligroso");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:O1"].Style.Font.Size = 20;
            Sheet.Cells["A1:O1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:O1"].Style.Font.Bold = true;
            Sheet.Cells["A1:O1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:O1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado Material Peligroso");
            Sheet.Cells["A1:O1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:O2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:O2"].Style.Font.Size = 12;
            Sheet.Cells["A2:O2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:O2"].Style.Font.Bold = true;

            Sheet.Cells["B2"].RichText.Add("Clave material peligroso");
            Sheet.Cells["A2"].RichText.Add("ID");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("Clase Div.");
            Sheet.Cells["E2"].RichText.Add("Peligro sec.");
            Sheet.Cells["F2"].RichText.Add("Grupo emb/env");
            Sheet.Cells["G2"].RichText.Add("Disp Esp.");
            Sheet.Cells["H2"].RichText.Add("Cant. lim.");
            Sheet.Cells["I2"].RichText.Add("Cant. exp.");
            Sheet.Cells["J2"].RichText.Add("Embalaje");
            Sheet.Cells["K2"].RichText.Add("RIG");
            Sheet.Cells["L2"].RichText.Add("Cisternas");
            Sheet.Cells["M2"].RichText.Add("Contenedores");
            Sheet.Cells["N2"].RichText.Add("Fecha_In");
            Sheet.Cells["O2"].RichText.Add("Fecha_Fin");


            row = 3;
            foreach (var item in bancos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDMatP;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClaveMatP;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaseDiv;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.PeligSec;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.GrupoEmb_Env;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.DispEsp;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.CantLim;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.CantExp;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.Embalaje;
                Sheet.Cells[string.Format("K{0}", row)].Value = item.RIG;
                Sheet.Cells[string.Format("L{0}", row)].Value = item.Cisternas;
                Sheet.Cells[string.Format("M{0}", row)].Value = item.Contenedores;
                Sheet.Cells[string.Format("G{0}", row)].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                Sheet.Cells[string.Format("N{0}", row)].Value = item.Fecha_In;
                Sheet.Cells[string.Format("O{0}", row)].Value = item.Fecha_Fin;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelMatP.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult ListadoMatP()
        {
            var MP = db.MaterialP.ToList();
            Reportec_MaterialPeligroso report = new Reportec_MaterialPeligroso();
            report.Material = MP;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteMatP.pdf");
        }
    }
}

