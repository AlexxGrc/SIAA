using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using PagedList;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas, Compras")]
    public class EstadosController : Controller
    {
        private EstadosContext db = new EstadosContext();
        private VEstadoContext dbe = new VEstadoContext();

        // GET: Estados
       
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            //var estados = db.Estados.Include(e => e.paises);
            //return View(estados.ToList());
            ViewBag.CurrentSort = sortOrder;
            ViewBag.EstadoSortParm = String.IsNullOrEmpty(sortOrder) ? "Estado" : "";
            ViewBag.PaisSortParm = String.IsNullOrEmpty(sortOrder) ? "Pais" : "Pais";
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
            var elementos = from s in dbe.VEstados
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Estado.Contains(searchString) || s.Pais.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Estado":
                    elementos = elementos.OrderBy(s => s.Estado);
                    break;
                case "Pais":
                    elementos = elementos.OrderBy(s => s.Pais);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Estado);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = dbe.VEstados.OrderBy(e => e.IDEstado).Count(); // Total number of elements

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


        // GET: Estados/Details/5
        public ActionResult Details(int id)
        {
            var elemento = dbe.VEstados.Single(m => m.IDEstado == id);
            if (elemento == null)
            {
                return NotFound();
            }
            return View(elemento);
        }

        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        // POST: Estados/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, VEstado collection)
        {
            var elemento = dbe.VEstados.Single(m => m.IDEstado == id);
            return View(elemento);
        }
        // GET: Estados/Create
        public ActionResult Create()
        {
            //Paises
            var datosPaises = db.Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> listaPais = new List<SelectListItem>();
            listaPais.Add(new SelectListItem { Text = "--Selecciona un País--", Value = "0" });
            foreach (var a in datosPaises)
            {
                listaPais.Add(new SelectListItem { Text = a.Codigo + " | " + a.Pais, Value = a.IDPais.ToString() });

            }
            ViewBag.IDPais = listaPais;
            return View();
        }

        // POST: Estados/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDEstado,Estado,IDPais")] Estados estados)
        {
            if (ModelState.IsValid)
            {
                db.Estados.Add(estados);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //Paises
            var datosPaises = db.Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> listaPais = new List<SelectListItem>();
            listaPais.Add(new SelectListItem { Text = "--Selecciona un Pais--", Value = "0" });
            foreach (var a in datosPaises)
            {
                listaPais.Add(new SelectListItem { Text = a.Codigo + " | " + a.Pais, Value = a.IDPais.ToString() });

            }
            ViewBag.IDPais = listaPais;
            //ViewBag.IDPais = new SelectList(db.Paises, "IDPais", "Codigo", estados.IDPais);
            return View(estados);
        }

        // GET: Estados/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Estados estados = db.Estados.Find(id);
            if (estados == null)
            {
                return HttpNotFound();
            }
            //Paises
            var datosPaises = db.Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> listaPais = new List<SelectListItem>();
            listaPais.Add(new SelectListItem { Text = "--Selecciona un Pais--", Value = "0" });
            foreach (var a in datosPaises)
            {
                listaPais.Add(new SelectListItem { Text = a.Codigo + " | " + a.Pais, Value = a.IDPais.ToString() });

            }
            ViewBag.IDPais = listaPais;
            //ViewBag.IDPais = new SelectList(db.Paises, "IDPais", "Codigo", estados.IDPais);
            return View(estados);
        }

        // POST: Estados/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDEstado,Estado,IDPais")] Estados estados)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estados).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDPais = new SelectList(db.Paises, "IDPais", "Codigo", estados.IDPais);
            return View(estados);
        }

        // GET: Estados/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VEstado estados = dbe.VEstados.Find(id);
            if (estados == null)
            {
                return HttpNotFound();
            }
            return View(estados);
        }

        // POST: Estados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Estados estados = db.Estados.Find(id);
            db.Estados.Remove(estados);
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
        public ActionResult ListadoEstados()
        {
            VEstadoContext dba = new VEstadoContext();
            var estados = dba.VEstados.ToList();
            ReporteEstados report = new ReporteEstados();
            report.estados = estados;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteEstados.pdf");
        }


        public void GenerarExcelEstados()
        {
            //Listado de datos
            VEstadoContext dba = new VEstadoContext();
            var claveU = dba.VEstados.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("Estados");
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
            Sheet.Cells["A1"].RichText.Add("Catalogo: Estados");

            row = 2;
            //Fijar la fuente para EL RANGO DE CELDAS A2:B2
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 12;
            Sheet.Cells["A2:B2"].Style.Font.Bold = true;
            Sheet.Cells["A2:B2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:B2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            //Sheet.Cells["A1"].Value = "Clave";
            //Sheet.Cells["B1"].Value = "Descripción";
            Sheet.Cells["A2"].RichText.Add("País");
            Sheet.Cells["B2"].RichText.Add("Estados");
            Sheet.Cells["A2:B2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            row = 3;
            foreach (var item in claveU)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.Pais;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Estado;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteEstados.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
    }
}
