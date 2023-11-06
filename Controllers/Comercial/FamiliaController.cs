
using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
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
using SIAAPI.Models.CartaPorte;

namespace SIAAPI.Controllers.Comercial
{
    public class FamiliaController : Controller
    {

        [Authorize(Roles = "Administrador,Facturacion,Gerencia,Ventas,Sistemas,Almacenista,Comercial,Compras,AdminProduccion")]
        //// GET: Familia
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            VFamiliaContext db = new VFamiliaContext();
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "CCodFam" : "CCodFam";
            ViewBag.DescripcionSortParm = String.IsNullOrEmpty(sortOrder) ? "Descripcion" : "Descripcion";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación

            var elementos = from s in db.VFamilias
                            select s;

            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.CCodFam.Contains(searchString) || s.Descripcion.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "CCodFam":
                    elementos = elementos.OrderBy(s => s.CCodFam);
                    break;
                case "Descripcion":
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.Descripcion);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.VFamilias.OrderBy(e => e.IDFamilia).Count(); // Total number of elements

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


        public ActionResult Create()
        {
            var ListaDescripcion = new ProductoServicioRepository().GetProductosoServicios();

            ViewBag.ListaDescripcion = ListaDescripcion;
            TipoArticuloContext dba = new TipoArticuloContext();
            ViewBag.IDTipoArticulo = new SelectList(dba.TipoArticulo, "IDTipoArticulo", "Descripcion");

            var ListaProdSTCC = new ProductoSTCCRepository().GetProductosSTCC();
            ViewBag.ListaProdSTCC = ListaProdSTCC;

            return View();
        }

        [HttpPost]
        public ActionResult Create(Familia elemento)
        {

            try
            {
                var db = new FamiliaContext();

                db.Familias.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {

                TipoArticuloContext dba = new TipoArticuloContext();
                ViewBag.IDTipoArticulo = new SelectList(dba.TipoArticulo, "IDTipoArticulo", "Descripcion");
                var ListaDescripcion = new ProductoServicioRepository().GetProductosoServicios();
                ViewBag.ListaDescripcion = ListaDescripcion;

                var ListaProdSTCC = new ProductoSTCCRepository().GetProductosSTCC();
                ViewBag.ListaProdSTCC = ListaProdSTCC;

                return View();
            }
        }


        public ActionResult Details(int id)
        {
            VFamiliaContext db = new VFamiliaContext();
            var lista = db.VFamilias.Single(m => m.IDFamilia == id);
            return View(lista);
        }


        public ActionResult Edit(int id)
        {
            //var elemento = db.ModeloProcesos.Single(m => m.IDModeloProceso == id);
            //return View(elemento);
            FamiliaContext db = new FamiliaContext();
            var elemento = db.Familias.Single(m => m.IDFamilia == id);

            //Familia elementooriginal = db.Familias.Single(m => m.IDFamilia == id);
            var ListaDescripcion = new ProductoServicioRepository().GetProductosoServicios();
            ViewBag.ListaDescripcion = ListaDescripcion;

            var ListaProdSTCC = new ProductoSTCCRepository().GetProductosSTCC();
            ViewBag.ListaProdSTCC = ListaProdSTCC;

            Familia elementooriginal = db.Familias.Find(id);
            if (elementooriginal == null)
            {
                return HttpNotFound();
            }

            //c_ClaveProdSTCCContext dbSTCC = new c_ClaveProdSTCCContext();
            //ViewBag.IDClaveSTCC = new SelectList(dbSTCC.produc, "IDClaveSTCC", "Descripcion", elementooriginal.IDClaveSTCC);

            TipoArticuloContext dba = new TipoArticuloContext();
            ViewBag.IDTipoArticulo = new SelectList(dba.TipoArticulo, "IDTipoArticulo", "Descripcion", elementooriginal.IDTipoArticulo);
            return View(elementooriginal);


        }

        // POST: ModeloProceso/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                FamiliaContext db = new FamiliaContext();
                var elemento = db.Familias.Single(m => m.IDFamilia == id);
                TipoArticuloContext dba = new TipoArticuloContext();
                ViewBag.IDTipoArticulo = new SelectList(dba.TipoArticulo, "IDTipoArticulo", "Descripcion", elemento.IDTipoArticulo);

                c_ClaveProdSTCCContext dbSTCC = new c_ClaveProdSTCCContext();
                ViewBag.IDClaveSTCC = new SelectList(dbSTCC.produc, "IDClaveSTCC", "Descripcion", elemento.IDClaveSTCC);

                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(elemento);
            }
            catch
            {
                var ListaDescripcion = new ProductoServicioRepository().GetProductosoServicios();
                ViewBag.ListaDescripcion = ListaDescripcion;
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            VFamiliaContext db = new VFamiliaContext();
            var elemento = db.VFamilias.Single(m => m.IDFamilia == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            return View(elemento);
        }

        // POST: ModeloProceso/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, VFamilia collection)
        {
            try
            {
                // TODO: Add delete logic here
                var db = new FamiliaContext();
                var elemento = db.Familias.Single(m => m.IDFamilia == id);
                db.Familias.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

        public ActionResult EditAtributo(int id, string SearchString = "")
        {
            System.Web.HttpContext.Current.Session["idFamilia"] = id;

            //    var cadena = new FamiliaRepository().getAtributosJsonSchemaFJ(id);
            //ViewBag.JsonA = cadena;
            AtributodeFamiliaContext db = new AtributodeFamiliaContext();
            var lista = from e in db.AtributodeFamilias
                        where e.IDFamilia == id
                        orderby e.IDAtributo
                        select e;
            //return View(elemento);

            var listadevalores = new AtributoFamiliaRepository().GetTipo();
            ViewBag.lista = listadevalores;
            ViewBag.SearchString = SearchString;
            return View(lista);
        }

        public ActionResult CreateAtributo()
        {
            AtributodeFamilia atributo = new AtributodeFamilia();
            atributo.LongitudMin = 1;
            atributo.LongitudMax = 100;
            var listadevalores = new AtributoFamiliaRepository().GetTipo();
            ViewBag.lista = listadevalores;

            atributo.IDFamilia = Int32.Parse(System.Web.HttpContext.Current.Session["idFamilia"].ToString());
            return View(atributo);
        }

        [HttpPost]
        public ActionResult CreateAtributo(AtributodeFamilia elemento)
        {
            try
            {
                AtributodeFamiliaContext db = new AtributodeFamiliaContext();
                Int32 idfamilia = Int32.Parse(System.Web.HttpContext.Current.Session["idFamilia"].ToString());
                elemento.IDFamilia = idfamilia;


                db.AtributodeFamilias.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("EditAtributo", new { id = elemento.IDFamilia });
            }
            catch (Exception err)
            {
                var listadevalores = new AtributoFamiliaRepository().GetTipo();
                ViewBag.lista = listadevalores;
                return View();
            }
        }

        public ActionResult EditarAtributo(int id, string SearchString)
        {

            AtributodeFamiliaContext db = new AtributodeFamiliaContext();
            var listadevalores = new AtributoFamiliaRepository().GetTipo();
            ViewBag.lista = listadevalores;
            ViewBag.IDFamilia = id;
            var elementooriginal = db.AtributodeFamilias.Single(m => m.IDAtributo == id);
            ViewBag.SearchString = SearchString;
            return View(elementooriginal);


        }

        // POST: ModeloProceso/Edit/5
        [HttpPost]
        public ActionResult EditarAtributo(AtributodeFamilia Elementoactualizado, string SearchString = "")

        {

            try
            {
                AtributodeFamiliaContext db = new AtributodeFamiliaContext();
                var elemento = db.AtributodeFamilias.Single(m => m.IDAtributo == Elementoactualizado.IDAtributo);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("EditAtributo", new { id = Elementoactualizado.IDFamilia, SearchString = SearchString });
                }
                return View(Elementoactualizado);
            }
            catch
            {
                var listadevalores = new AtributoFamiliaRepository().GetTipo();
                ViewBag.lista = listadevalores;

                return View();
            }
        }

        public ActionResult DeleteAtributo(int id)
        {
            //var elemento = db.ModeloProcesos.Single(m => m.IDModeloProceso == id);
            //return View(elemento);
            AtributodeFamiliaContext db = new AtributodeFamiliaContext();

            var elementooriginal = db.AtributodeFamilias.Single(m => m.IDAtributo == id);

            return View(elementooriginal);


        }

        [HttpPost]
        public ActionResult DeleteAtributo(int id, AtributodeFamilia collection)
        {
            try
            {
                // TODO: Add delete logic here
                var db = new AtributodeFamiliaContext();
                var elemento = db.AtributodeFamilias.Single(m => m.IDAtributo == id);
                db.AtributodeFamilias.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("EditAtributo", new { id = elemento.IDFamilia });

            }
            catch
            {
                return View();
            }
        }

        ///////////////////////////////////////////////////////////////////////////Niveles de Ganancia//////////////////////////////////////////////////////////////////////////////////////////
        FamiliaContext db = new FamiliaContext();

        public ActionResult NivelesGanancia(int? id)
        {

            Familia familia = new FamiliaContext().Familias.Find(id);
            List<NivelesGanancia> niveles = new FamiliaContext().Database.SqlQuery<NivelesGanancia>("select * from NivelesGanancia where IDFamilia=" + id + "").ToList();
            ViewBag.id = id;





            ClsDatoEntero countRangoP = db.Database.SqlQuery<ClsDatoEntero>("select count(idNivel) as Dato from NivelesGanancia where IDFamilia =" + id + "").ToList().FirstOrDefault();
            int nivel = 1;
            try
            {
                ClsDatoEntero cuantosniveles = db.Database.SqlQuery<ClsDatoEntero>("select MAX(Nivel) as Dato from NivelesGanancia where IDFamilia =" + id + " group by IDFamilia").ToList().FirstOrDefault();
                nivel = cuantosniveles.Dato + 1;
            }
            catch (Exception err)
            {

            }

            decimal ranginf = 1;
            try
            {
                ClsDatoDecimal cuantosniveles = db.Database.SqlQuery<ClsDatoDecimal>("select MAX(RangSup) as Dato from NivelesGanancia where IDFamilia =" + id + " group by IDFamilia").ToList().FirstOrDefault();
                ranginf = cuantosniveles.Dato + 1;
            }
            catch (Exception err)
            {

            }

            ViewBag.countRangoP = countRangoP.Dato;
            ViewBag.nivel = nivel;
            ViewBag.rangoinferior = ranginf;
            ViewBag.rangosuperior = 999999;

            return PartialView(niveles);
        }



        [HttpPost]

        public ActionResult InsertarGanancia(int? idfamilia, int nivel, decimal? ranginf, decimal? rangsup, decimal? porcentaje)
        {
            Familia familia = db.Familias.Find(idfamilia);
            try
            {

                db.Database.ExecuteSqlCommand("insert into NivelesGanancia(IDFamilia, nivel, Ranginf, RangSup, Porcentaje) values (" + idfamilia + ", " + nivel + "," + ranginf + "," + rangsup + "," + porcentaje + ")");


            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
            return RedirectToAction("NivelesGanancia", new { id = idfamilia });
        }


        [HttpPost]
        public JsonResult EditGanancia(int id, int nivel, decimal rangi, decimal rangs, decimal porcentaje)
        {
            try
            {
                string cadena = "update [dbo].[NivelesGanancia] set [RangInf]=" + rangi + ", Nivel=" + nivel + ", [RangSup]='" + rangs + "' , Porcentaje='" + porcentaje + "' where IDNivel=" + id;
                db.Database.ExecuteSqlCommand(cadena);


                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }
        [HttpPost]
        public JsonResult DeleteitemGanancia(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("delete from [dbo].[NivelesGanancia] where [idNivel]='" + id + "'");
                return Json(true);

            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }




        public ActionResult CreateAtributoHE()
        {
            AtributosdeHE atributo = new AtributosdeHE();
            atributo.LongitudMin = 1;
            atributo.LongitudMax = 100;
            var listadevalores = new AtributosdeHERepository().GetTipo();
            ViewBag.lista = listadevalores;

            atributo.IDFamilia = Int32.Parse(System.Web.HttpContext.Current.Session["idFamilia"].ToString());
            return View(atributo);
        }

        [HttpPost]
        public ActionResult CreateAtributoHE(AtributosdeHE elemento)
        {
            try
            {
                AtributosdeHEContext db = new AtributosdeHEContext();
                Int32 idfamilia = Int32.Parse(System.Web.HttpContext.Current.Session["idFamilia"].ToString());
                elemento.IDFamilia = idfamilia;


                db.AtributosdeHEs.Add(elemento);
                db.SaveChanges();
                return RedirectToAction("EditAtributoHE", new { id = elemento.IDFamilia });
            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
                var listadevalores = new AtributosdeHERepository().GetTipo();
                ViewBag.lista = listadevalores;
                return View();
            }
        }

        public ActionResult EditAtributoHE(int id)
        {
            System.Web.HttpContext.Current.Session["idFamilia"] = id;

            //    var cadena = new FamiliaRepository().getAtributosJsonSchemaFJ(id);
            //ViewBag.JsonA = cadena;
            AtributosdeHEContext db = new AtributosdeHEContext();
            var lista = from e in db.AtributosdeHEs
                        where e.IDFamilia == id
                        orderby e.IDAtributo
                        select e;
            //return View(elemento);

            var listadevalores = new AtributosdeHERepository().GetTipo();
            ViewBag.lista = listadevalores;

            return View(lista);
        }
        public ActionResult EditarAtributoHE(int id)
        {

            AtributosdeHEContext db = new AtributosdeHEContext();
            var listadevalores = new AtributosdeHERepository().GetTipo();
            ViewBag.lista = listadevalores;
            ViewBag.IDFamilia = id;
            var elementooriginal = db.AtributosdeHEs.Single(m => m.IDAtributo == id);

            return View(elementooriginal);


        }

        // POST: ModeloProceso/Edit/5
        [HttpPost]
        public ActionResult EditarAtributoHE(AtributosdeHE Elementoactualizado)
        {
            try
            {
                AtributosdeHEContext db = new AtributosdeHEContext();
                var elemento = db.AtributosdeHEs.Single(m => m.IDAtributo == Elementoactualizado.IDAtributo);
                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
                    return RedirectToAction("EditAtributoHE", new { id = Elementoactualizado.IDFamilia });
                }
                return View(Elementoactualizado);
            }
            catch
            {
                var listadevalores = new AtributosdeHERepository().GetTipo();
                ViewBag.lista = listadevalores;

                return View();
            }
        }

        public ActionResult DeleteAtributoHE(int id)
        {
            //var elemento = db.ModeloProcesos.Single(m => m.IDModeloProceso == id);
            //return View(elemento);
            AtributosdeHEContext db = new AtributosdeHEContext();

            var elementooriginal = db.AtributosdeHEs.Single(m => m.IDAtributo == id);

            return View(elementooriginal);


        }

        [HttpPost]
        public ActionResult DeleteAtributoHE(int id, AtributosdeHE collection)
        {
            try
            {
                // TODO: Add delete logic here
                var db = new AtributosdeHEContext();
                var elemento = db.AtributosdeHEs.Single(m => m.IDAtributo == id);
                db.AtributosdeHEs.Remove(elemento);
                db.SaveChanges();
                return RedirectToAction("EditAtributoHE", new { id = elemento.IDFamilia });

            }
            catch
            {
                return View();
            }
        }
        //public ActionResult ListaFamAtrib()
        //{
        //    VFamiliaContext db = new VFamiliaContext();
        //    var famiatribs = db.VFamilias.ToList();
        //    ReporteListaFamAtrib report = new ReporteListaFamAtrib();
        //    report.ListaDatosBD_VFamilia = famiatribs;
        //    byte[] abytes = report.PrepareReport();
        //    return File(abytes, "application/pdf", "ReporteFamiliasAtributos.pdf");
        //}

        public void GenerarExcelFamAtrib()
        {
            //Listado de datos
            VFamiliaContext db = new VFamiliaContext();
            var famiatribs = db.VFamilias.ToList();
            ExcelPackage Ep = new ExcelPackage();
            var Sheet = Ep.Workbook.Worksheets.Add("Familias");

            int row = 1;
            //Fijar la fuente para A1:Q1
            Sheet.Cells["A1:F1"].Style.Font.Size = 20;
            Sheet.Cells["A1:F1"].Style.Font.Name = "Calibri";
            Sheet.Cells["A1:F1"].Style.Font.Bold = true;
            Sheet.Cells["A1:F1"].Style.Font.Color.SetColor(Color.DarkBlue);
            Sheet.Cells["A1:F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
            Sheet.Cells["A1"].RichText.Add("Listado de Familias de Artículos");
            Sheet.Cells["A1:F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Hair;

            row = 2;
            Sheet.Cells["A2:F2"].Style.Font.Name = "Calibri";
            Sheet.Cells["A2:F2"].Style.Font.Size = 12;
            Sheet.Cells["A2:F2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;
            Sheet.Cells["A2:F2"].Style.Font.Bold = true;
            Sheet.Cells["A2:F2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:F2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);


            Sheet.Cells["B2"].RichText.Add("Clave");
            Sheet.Cells["A2"].RichText.Add("Código Familia");
            Sheet.Cells["C2"].RichText.Add("Descripción");
            Sheet.Cells["D2"].RichText.Add("Clave SAT");
            Sheet.Cells["E2"].RichText.Add("Producto SAT");
            Sheet.Cells["F2"].RichText.Add("Factor Mínimo de Ganancia");

            row = 3;
            foreach (var item in famiatribs)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDFamilia;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.CCodFam;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Descripcion;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.ClaveSat;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.ProductoSat;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.FactorMinimoGanancia;
                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteExcelFamilia.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }



    }
}