
using PagedList;
using SIAAPI.Models.Administracion;

using SIAAPI.Reportes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using OfficeOpenXml;
using System.Drawing;


namespace SIAAPI.Controllers.Administracion
{
    public class c_BancoController : Controller
    {
      
        private c_BancoContext db = new c_BancoContext();
        // GET: c_Banco
        //public ActionResult Index()
        //{
        //var lista =  from e in db.c_Bancos
        //            orderby e.IDBanco
        //            select e;
        //return View(lista);
        //}
        [Authorize(Roles = "Administrador,Sistemas,Facturacion,Gerencia")]
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "ClaveBanco" : "";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "Nombre";
            ViewBag.RazoneSortParm = String.IsNullOrEmpty(sortOrder) ? "RazonSocial" : "RazonSocial";
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
            var elementos = from s in db.c_Bancos
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.ClaveBanco.Contains(searchString) || s.Nombre.Contains(searchString) || s.RazonSocial.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "ClaveBanco":
                    elementos = elementos.OrderBy(s => s.ClaveBanco);
                    break;
                case "Nombre":
                    elementos = elementos.OrderBy(s => s.Nombre);
                    break;
                case "RazonSocial":
                    elementos = elementos.OrderBy(s => s.RazonSocial);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.ClaveBanco);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.c_Bancos.OrderBy(e => e.IDBanco).Count(); // Total number of elements

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


        // GET: c_Banco/Details/5
        [Authorize(Roles = "Administrador")]

        public ActionResult Details(int id)
        {
            var elemento = db.c_Bancos.Single(m => m.IDBanco == id);
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

        // POST: c_Banco/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, c_Banco collection)
        {
            var elemento = db.c_Bancos.Single(m => m.IDBanco == id);
            return View(elemento);
        }

        // GET: c_Banco/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: c_Banco/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(c_Banco elemento)
        {
            try
            {
                // TODO: Add insert logic here

                var db = new c_BancoContext();
                db.c_Bancos.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: c_Banco/Edit/5
        public ActionResult Edit(int id)
        {
            var elemento = db.c_Bancos.Single(m => m.IDBanco == id);
            return View(elemento);
        }

        // POST: c_Banco/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var elemento = db.c_Bancos.Single(m => m.IDBanco == id);
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
            var elemento = db.c_Bancos.Single(m => m.IDBanco == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }


        // POST: a/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var elemento = db.c_Bancos.Single(m => m.IDBanco == id);
                db.c_Bancos.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }


        public ActionResult ListadoBancos()
        {
            c_BancoContext dba = new c_BancoContext();
            var bancos = dba.c_Bancos.ToList();
            ReporteBancos report = new ReporteBancos();
            report.Bancos = bancos;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteBancos.pdf");
        }

        //public ActionResult GenerarExcelBanco()
        //{
        //    //Listado de datos
        //    c_BancoContext dba = new c_BancoContext();
        //    var bancos = dba.c_Bancos.ToList();

        //    StringBuilder builder = new StringBuilder();
        //    // Agregamos el encabezado
        //    builder.Append("Listado de bancos").Append(";");
        //    builder.Append("\n");

        //    foreach (var item in bancos)
        //    {
        //        builder.Append(item.IDBanco).Append(";")
        //        .Append(item.ClaveBanco).Append(";")
        //        .Append(item.Nombre).Append(";")
        //        .Append(item.RazonSocial).Append(";")
        //        .Append(item.RFC).Append(";");
        //        builder.Append("\n");// agregamos una nueva fila 
        //    }


        //        // Lo convertimos con UTF8 para mostrar los acentos correctamente.
        //        var excelBytes = Encoding.UTF8.GetBytes(builder.ToString());
        //        var excelConUT8Encoding = Encoding.UTF8.GetPreamble().Concat(excelBytes).ToArray();

        //        // guardamos el contenido del archivo en la ruta especificada
        //        var rutaExcel = Server.MapPath("~/App_Data/excel.csv");
        //        System.IO.File.WriteAllBytes(rutaExcel, excelConUT8Encoding); 
        //return File(rutaExcel, "text/csv", "Bancos.csv");
        //}


        public void GenerarExcelBanco()
        {
            //Listado de datos
            c_BancoContext dba = new c_BancoContext();
            var bancos = dba.c_Bancos.ToList();
            ExcelPackage Ep = new ExcelPackage();

            var Sheet = Ep.Workbook.Worksheets.Add("Bancos");

            Sheet.Cells["A1"].Value = "ID";
            Sheet.Cells["B1"].Value = "Clave";
            Sheet.Cells["C1"].Value = "Nombre";
            Sheet.Cells["D1"].Value = "Razon Social";
            Sheet.Cells["E1"].Value = "RFC";
            int row = 2;
            foreach (var item in bancos)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDBanco;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ClaveBanco;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Nombre;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.RazonSocial;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.RFC;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelBanco.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }

    }
}
