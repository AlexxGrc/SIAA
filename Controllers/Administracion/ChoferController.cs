
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
using SIAAPI.Models.CartaPorte;

namespace SIAAPI.Controllers.Administracion
{
    public class ChoferController : Controller
    {
        // GET: Chofer

        ChoferesContext db = new ChoferesContext();
        //public ActionResult Index()
        //{
        //    var listadechoferes = db.Choferes.ToList();
        //    return View(listadechoferes);
        //}
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Nombre" : "";


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
            var elementos = from s in db.Choferes
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {


                elementos = elementos.Where(s => s.Nombre.Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Nombre":
                    elementos = elementos.OrderBy(s => s.Nombre);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.IDChofer);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Choferes.OrderBy(e => e.IDChofer).Count(); // Total number of elements

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


        public ActionResult Create()
        {
            Chofer elemento = new Chofer();
            //Paises
            var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> liP = new List<SelectListItem>();
            liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
            foreach (var a in datosPaises)
            {
                liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });

            }
            ViewBag.ListPais = liP;
            ViewBag.ListEstado = getEstadoPorPais(0);
            ViewBag.ListMunicipio = getMunicipioPorEstado(0);
            ViewBag.ListLocalidad = getLocalidadPorEstado(0);

            var Colonias = new c_ColoniaContext().Database.SqlQuery<c_Colonia>("select*from c_Colonia").ToList();
            List<SelectListItem> liAC = new List<SelectListItem>();
            liAC.Add(new SelectListItem { Text = "Selecciona una Colonia", Value = "0" });
            foreach (var a in Colonias)
            {

                liAC.Add(new SelectListItem { Text = a.NomAsentamiento, Value = a.IDColonia.ToString() });

            }
            ViewBag.IDColonia = liAC;
            var colonias = new SIAAPI.Models.CartaPorte.c_ColoniaContext().Database.SqlQuery<SIAAPI.Models.CartaPorte.c_Colonia>("select * from c_Colonia").ToList();
            ViewBag.Colonias = colonias;

            return View(elemento);
        }

        [HttpPost]
        public ActionResult Create(Chofer elementonuevo, FormCollection coleccion)
        {
            try
            {
                // TODO: Add insert logic here

                db.Choferes.Add(elementonuevo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
                List<SelectListItem> liP = new List<SelectListItem>();
                liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
                foreach (var a in datosPaises)
                {
                    if (elementonuevo.IDPais == a.IDPais)
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString(), Selected = true });
                    }
                    else
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });
                    }


                }
                ViewBag.ListPais = liP;
                ViewBag.ListEstado = getEstadoPorPais(elementonuevo.IDPais);
                ViewBag.ListMunicipio = getMunicipioPorEstado(elementonuevo.IDEstado);
                ViewBag.ListLocalidad = getLocalidadPorEstado(elementonuevo.IDEstado);

                var Colonias = new c_ColoniaContext().Database.SqlQuery<c_Colonia>("select*from c_Colonia").ToList();
                List<SelectListItem> liAC = new List<SelectListItem>();
                liAC.Add(new SelectListItem { Text = "Selecciona una Colonia", Value = "0" });
                foreach (var a in Colonias)
                {
                    if (elementonuevo.IDColonia == a.IDColonia)
                    {
                        liAC.Add(new SelectListItem { Text = a.NomAsentamiento, Value = a.IDColonia.ToString(), Selected = true });

                    }
                    else
                    {
                        liAC.Add(new SelectListItem { Text = a.NomAsentamiento, Value = a.IDColonia.ToString() });

                    }

                }
                ViewBag.IDColonia = liAC;
                return View(elementonuevo);
            }
        }
        public JsonResult getColonias(string buscar)
        {
            ////buscar = buscar.Remove(buscar.Length - 1);
            var Colonias = new c_ColoniaContext().Database.SqlQuery<c_Colonia>("select*from c_Colonia where C_CodigoPostal='" + buscar + "'").ToList();

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (c_Colonia art in Colonias)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.NomAsentamiento;
                elemento.Value = art.IDColonia.ToString();
                opciones.Add(elemento);
            }
            //var colonias = new SIAAPI.Models.CartaPorte.c_ColoniaContext().Database.SqlQuery<SIAAPI.Models.CartaPorte.c_Colonia>("select * from c_Colonia where C_CodigoPostal='" + buscar + "'").ToList();
            //ViewBag.Colonias = colonias;
            return Json(opciones, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getLocalidades(string buscar)
        {
            ////buscar = buscar.Remove(buscar.Length - 1);
            var Localidades = new c_LocalidadContext().Database.SqlQuery<c_Localidad>("  select l.* from c_localidad as l inner join estados as e on e.c_estado=l.c_estado where e.idestado='" + buscar + "'").ToList();

            // Populate DropDownList
            List<SelectListItem> opciones = new List<SelectListItem>();

            foreach (c_Localidad art in Localidades)
            {
                SelectListItem elemento = new SelectListItem();
                elemento.Text = art.Descripcion;
                elemento.Value = art.IDLocalidad.ToString();
                opciones.Add(elemento);
            }
            return Json(opciones, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            Chofer elemento = db.Choferes.Find(id);
            return View(elemento);
        }


        public ActionResult Edit(int id)
        {
            Chofer elementonuevo = db.Choferes.Find(id);
            var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> liP = new List<SelectListItem>();
            liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
            foreach (var a in datosPaises)
            {
                if (elementonuevo.IDPais == a.IDPais)
                {
                    liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString(), Selected = true });
                }
                else
                {
                    liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });
                }


            }
            ViewBag.ListPais = liP;
            ViewBag.IDEstado = getEstadoPorPaisSelec(elementonuevo.IDPais, elementonuevo.IDEstado);
            ViewBag.IDMunicipio = getMunicipioPorEstadoSelec(elementonuevo.IDEstado, elementonuevo.IDMunicipio);
            ViewBag.IDLocalidad = getLocalidadPorEstadoSelec(elementonuevo.IDEstado, elementonuevo.IDLocalidad);


            ViewBag.IDColonia = getColoniaPorCPSelec(elementonuevo.CP, elementonuevo.IDColonia);
            return View(elementonuevo);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collectioo)
        {
            Chofer elemento = db.Choferes.Find(id);
            try
            {

                if (TryUpdateModel(elemento))
                {


                    string update = "update Chofer set RFC='" + elemento.RFC + "', Nombre='" + elemento.Nombre + "', NoLicencia='" + elemento.NoLicencia + "'," +
                        "Activo='" + elemento.Activo + "', Calle='" + elemento.Calle + "',NumExt='" + elemento.NumExt + "', NumInt='" + elemento.NumInt + "',IDPais='" + elemento.IDPais + "', IDEstado='" + elemento.IDEstado + "'," +
                        " IDMunicipio='" + elemento.IDMunicipio + "', IDLocalidad='" + elemento.IDLocalidad + "', IDColonia='" + elemento.IDColonia + "', CP='" + elemento.CP + "', Referencia='" + elemento.Referencia + "' where idchofer=" + id;
                    //db.SaveChanges();
                    db.Database.ExecuteSqlCommand(update);
                    return RedirectToAction("Index");
                }
                return View(elemento);
            }
            catch
            {

                var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
                List<SelectListItem> liP = new List<SelectListItem>();
                liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
                foreach (var a in datosPaises)
                {
                    if (elemento.IDPais == a.IDPais)
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString(), Selected = true });
                    }
                    else
                    {
                        liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });
                    }


                }
                ViewBag.ListPais = liP;
                ViewBag.IDEstado = getEstadoPorPaisSelec(elemento.IDPais, elemento.IDEstado);
                ViewBag.IDMunicipio = getMunicipioPorEstadoSelec(elemento.IDEstado, elemento.IDMunicipio);
                ViewBag.IDLocalidad = getLocalidadPorEstadoSelec(elemento.IDEstado, elemento.IDLocalidad);


                ViewBag.IDColonia = getColoniaPorCPSelec(elemento.CP, elemento.IDColonia);
                return View(elemento);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var elemento = db.Choferes.Find(id);
                db.Choferes.Remove(elemento);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult ListadoChoferes()
        {
            ChoferesContext dba = new ChoferesContext();

            var chofer = dba.Choferes.ToList();
            ReporteChoferes report = new ReporteChoferes();
            report.choferes = chofer;
            byte[] abytes = report.PrepareReport();
            return File(abytes, "applicacion/pdf", "ReporteChoferes.pdf");
        }

        public void GenerarExcelChoferes()
        {
            //Listado de datos
            ChoferesContext dba = new ChoferesContext();
            var chof = dba.Choferes.ToList();
            ExcelPackage Ep = new ExcelPackage();



            ///////////////////////////////////////////////////////////
            var Sheet = Ep.Workbook.Worksheets.Add("Choferes");
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
            Sheet.Cells["A1"].RichText.Add("Catalogo: Choferes");

            row = 2;
            //Fijar la fuente para EL RANGO DE CELDAS A2:B2
            Sheet.Cells.Style.Font.Name = "Calibri";
            Sheet.Cells.Style.Font.Size = 12;
            Sheet.Cells["A2:C2"].Style.Font.Bold = true;
            Sheet.Cells["A2:C2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Sheet.Cells["A2:C2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            //Sheet.Cells["A1"].Value = "Clave";
            //Sheet.Cells["B1"].Value = "Descripción";
            Sheet.Cells["A2"].RichText.Add("Id");
            Sheet.Cells["B2"].RichText.Add("Nombre");
            Sheet.Cells["C2"].RichText.Add("Activo");
            Sheet.Cells["A2:C2"].Style.Border.Bottom.Style = ExcelBorderStyle.Double;

            row = 3;
            foreach (var item in chof)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.IDChofer;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.Nombre;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.Activo;

                if (item.Activo == false)
                {
                    Sheet.Cells[string.Format("C{0}", row)].Value = "No";
                }
                else
                {
                    Sheet.Cells[string.Format("C{0}", row)].Value = "Si";
                }

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "ReporteChoferes.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }


        /////////////NUEVO
        ///

        public ActionResult getJsonEstadoPorPais(int id)
        {
            var estado = new Repositorys().GetEstadoPorPais(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getEstadoPorPais(int idp)
        {
            var estado = new Repositorys().GetEstadoPorPais(idp);
            return estado;

        }

        public ActionResult getJsonMunicipioPorEstado(int id)
        {
            var estado = new Repositorys().GetMunicipioPorEstado(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getMunicipioPorEstado(int idp)
        {
            var estado = new Repositorys().GetMunicipioPorEstado(idp);
            return estado;

        }

        public ActionResult getJsonLocalidadPorEstado(int id)
        {
            var estado = new Repositorys().GetLocalidadPorEstado(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getLocalidadPorEstado(int idp)
        {
            var estado = new Repositorys().GetLocalidadPorEstado(idp);
            return estado;

        }
        public IEnumerable<SelectListItem> getEstadoPorPaisSelec(int idp, int ide)
        {
            var estado = new Repositorys().GetEstadoPorPaisSelec(idp, ide);
            return estado;

        }
        public IEnumerable<SelectListItem> getMunicipioPorEstadoSelec(int ide, int idm)
        {
            var estado = new Repositorys().GetMunicipioPorEstadoSelect(ide, idm);
            return estado;

        }
        public IEnumerable<SelectListItem> getLocalidadPorEstadoSelec(int ide, int idl)
        {
            var estado = new Repositorys().GetLocalidadPorEstadoSelec(ide, idl);
            return estado;

        }

        public IEnumerable<SelectListItem> getColoniaPorCPSelec(string CP, int idc)
        {
            var estado = new Repositorys().GetColoniaPorCPSelec(CP, idc);
            return estado;

        }
    }

    public class Repositorys
    {
        public IEnumerable<SelectListItem> GetEstadoPorPais(int idpais)
        {
            List<SelectListItem> lista;
            if (idpais == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un país primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = "Select * from [dbo].[Estados] where [IDPais] =" + idpais + " ";

                lista = context.Database.SqlQuery<Estados>(cadenasql).ToList().OrderBy(n => n.Estado).Select(n => new SelectListItem
                {
                    Value = n.IDEstado.ToString(),
                    Text = n.Estado
                }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona un Estado"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

        public IEnumerable<SelectListItem> GetEstadoPorPaisSelec(int idpais, int idestado)
        {
            List<SelectListItem> lista;
            if (idpais == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un país primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = "Select * from dbo.Estados where IDEstado= " + idestado + " union Select * from dbo.Estados where IDEstado<> " + idestado + " and IDPais =" + idpais + " ";
                lista = context.Database.SqlQuery<Estados>(cadenasql).ToList()

                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDEstado.ToString(),
                             Text = n.Estado
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona un estado"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetMunicipioPorEstado(int idestado)
        {
            List<SelectListItem> lista;
            if (idestado == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un Estado primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = "  select m.* from c_municipio as m inner join estados as e on e.c_estado=m.c_estado where e.idestado= " + idestado + " ";
                lista = context.Database.SqlQuery<c_Municipio>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDMunicipio.ToString(),
                             Text = n.Descripcion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona un Municipio"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

        public IEnumerable<SelectListItem> GetMunicipioPorEstadoSelect(int idestado, int idmunicipio)
        {
            List<SelectListItem> lista;
            if (idestado == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un Estado primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = "  select m.* from c_municipio as m inner join estados as e on e.c_estado=m.c_estado where e.idestado= " + idestado + "" +
                    "union Select mu.* from dbo.c_municipio  as mu inner join estados as es on es.c_estado=mu.c_estado where mu.idmunicipio<> " + idmunicipio + " and es.idestado =" + idestado + "";
                lista = context.Database.SqlQuery<c_Municipio>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDMunicipio.ToString(),
                             Text = n.Descripcion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona un Municipio"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

        public IEnumerable<SelectListItem> GetLocalidadPorEstado(int idestado)
        {
            List<SelectListItem> lista;
            if (idestado == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un Estado primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = " select l.* from c_localidad as l inner join estados as e on e.c_estado=l.c_estado where e.idestado=" + idestado + " ";
                lista = context.Database.SqlQuery<c_Localidad>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDLocalidad.ToString(),
                             Text = n.Descripcion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona una Localidad"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }
        public IEnumerable<SelectListItem> GetLocalidadPorEstadoSelec(int idestado, int idlocalidad)
        {
            List<SelectListItem> lista;
            if (idestado == 0)
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Elige un Estado primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = " select l.* from c_localidad as l inner join estados as e on e.c_estado=l.c_estado where e.idestado=" + idestado + " " +
                     "union Select lo.* from dbo.c_localidad  as lo inner join estados as es on es.c_estado=lo.c_estado where lo.idlocalidad<> " + idlocalidad + " and es.idestado =" + idestado + "";

                lista = context.Database.SqlQuery<c_Localidad>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDLocalidad.ToString(),
                             Text = n.Descripcion
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona una Localidad"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }
        public IEnumerable<SelectListItem> GetColoniaPorCPSelec(string CP, int idcolonia)
        {
            List<SelectListItem> lista;
            if (CP == "")
            {
                lista = new List<SelectListItem>();
                lista.Add(new SelectListItem() { Value = "0", Text = "Ingresa CP primero" });
                return (lista);
            }
            using (var context = new EstadosContext())
            {
                string cadenasql = " select c.* from c_Colonia as c where c.c_codigopostal='" + CP.Trim() + "'" +
                     "union Select * from dbo.c_colonia where idcolonia<> " + idcolonia + " and c_codigopostal ='" + CP.Trim() + "'";

                lista = context.Database.SqlQuery<c_Colonia>(cadenasql).ToList()
                    .OrderBy(n => n.NomAsentamiento)
                        .Select(n =>
                         new SelectListItem
                         {
                             Value = n.IDColonia.ToString(),
                             Text = n.NomAsentamiento
                         }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "Selecciona una Colonia"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }

    }
}