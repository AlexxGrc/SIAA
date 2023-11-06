using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using SIAAPI.Models.CartaPorte;
using SIAAPI.Models.Administracion;
using SIAAPI.Controllers.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.CartaPorte;
using System.Net;

namespace SIAAPI.Controllers.CartaPorte
{
    public class PropietarioController : Controller
    {
        private PropietarioContext db = new PropietarioContext();
        private VPropietarioContext dbv = new VPropietarioContext();
        // GET: Propietario
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "RFCPropietario" : "RFCPropietario";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "NombrePropietario" : "NombrePropietario";
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
            var elementos = from s in db.Propietario
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {

                elementos = elementos.Where(s => s.RFCPropietario.Contains(searchString) || s.NombrePropietario.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "RFCPropietario":
                    elementos = elementos.OrderBy(s => s.RFCPropietario);
                    break;
                case "NombrePropietario":
                    elementos = elementos.OrderBy(s => s.NombrePropietario);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.NombrePropietario);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Propietario.OrderBy(e => e.IDPropietario).Count(); // Total number of elements

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

        // GET: Propietario/Details/5
        public ActionResult Details(int id)
        {
            var elemento = db.Database.SqlQuery<VPropietario>("select * from vpropietario where IDPropietario = " + id).ToList().FirstOrDefault();
            ViewBag.data = elemento;
            //var elemento = dbv.VPropietario.Single(m => m.IDPropietario == id);
            //if (elemento == null)
            //{
            //    return NotFound();
            //}
            return View(elemento);
            //return View();
        }

        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }


        public ActionResult getJsonEstadoPorPais(int id)
        {
            var estado = new PaisesRepository().GetEstadoPorPais(id);
            return Json(estado, JsonRequestBehavior.AllowGet);

        }

        public IEnumerable<SelectListItem> getEstadoPorPais(int idp)
        {
            var estado = new PaisesRepository().GetEstadoPorPais(idp);
            return estado;

        }

        public ActionResult getJsonMunicipioPorEstado(int id)
        {
            var estado = new PaisesRepository().GetMunicipioPorEstado(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getMunicipioPorEstado(int idp)
        {
            var estado = new PaisesRepository().GetMunicipioPorEstado(idp);
            return estado;

        }

        public ActionResult getJsonLocalidadPorEstado(int id)
        {
            var estado = new PaisesRepository().GetLocalidadPorEstado(id);
            return Json(estado, JsonRequestBehavior.AllowGet); ;

        }

        public IEnumerable<SelectListItem> getLocalidadPorEstado(int idp)
        {
            var estado = new PaisesRepository().GetLocalidadPorEstado(idp);
            return estado;

        }
        public IEnumerable<SelectListItem> getEstadoPorPaisSelec(int idp, int ide)
        {
            var estado = new PaisesRepository().GetEstadoPorPaisSelec(idp, ide);
            return estado;

        }
        public IEnumerable<SelectListItem> getMunicipioPorEstadoSelec(int ide, int idm)
        {
            var estado = new PaisesRepository().GetMunicipioPorEstadoSelect(ide, idm);
            return estado;

        }
        public IEnumerable<SelectListItem> getLocalidadPorEstadoSelec(int ide, int idl)
        {
            var estado = new PaisesRepository().GetLocalidadPorEstadoSelec(ide, idl);
            return estado;

        }

        public IEnumerable<SelectListItem> getColoniaPorCPSelec(string CP, int idc)
        {
            var estado = new PaisesRepository().GetColoniaPorCPSelec(CP, idc);
            return estado;

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



        // GET: Propietario/Create
        public ActionResult Create()
        {

            Propietario elemento = new Propietario();
            PaisesContext p = new PaisesContext();
            ViewBag.ResidenciaFiscal = new SelectList(p.Paises, "c_Pais", "Pais");
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

        // POST: Propietario/Create
        [HttpPost]
        public ActionResult Create(Propietario elementonuevo, FormCollection coleccion)
        {
            ClsDatoString cpais = db.Database.SqlQuery<ClsDatoString>("select c_pais as Dato from paises where IDPais=" + elementonuevo.IDPais).ToList().FirstOrDefault();
            elementonuevo.c_Pais = cpais.Dato;
            ClsDatoString cedo = db.Database.SqlQuery<ClsDatoString>("select c_estado as Dato from estados where IDEstado=" + elementonuevo.IDEstado).ToList().FirstOrDefault();
            elementonuevo.c_Estado = cedo.Dato;
            ClsDatoString cmpio = db.Database.SqlQuery<ClsDatoString>("select c_Municipio as Dato from c_Municipio where IDMunicipio= " + elementonuevo.IDMunicipio).ToList().FirstOrDefault();
            elementonuevo.c_Municipio = cmpio.Dato;
            ClsDatoString cloc = db.Database.SqlQuery<ClsDatoString>("select C_Localidad as Dato from c_Localidad where IDLocalidad= " + elementonuevo.IDLocalidad).ToList().FirstOrDefault();
            elementonuevo.c_Localidad = cloc.Dato;
            ClsDatoString ccol = db.Database.SqlQuery<ClsDatoString>("select C_Colonia as Dato from c_Colonia where IDColonia = " + elementonuevo.IDColonia).ToList().FirstOrDefault();
            elementonuevo.c_Colonia= ccol.Dato;
            if (elementonuevo.Activo == false)
            {
                elementonuevo.Activo = true;
            }

            try
            {
                // TODO: Add insert logic here

                db.Propietario.Add(elementonuevo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                Propietario elemento = new Propietario();
                PaisesContext p = new PaisesContext();
                ViewBag.ResidenciaFiscal = new SelectList(p.Paises, "c_Pais", "Pais");
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
        }

        // GET: Propietario/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Propietario elemento = db.Propietario.Find(id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            PaisesContext p = new PaisesContext();
            //ClsDatoEntero idpais = db.Database.SqlQuery<ClsDatoEntero>("select IDpais as Dato from paises where c_pais='" + elemento.ResidenciaFiscal+ "'").ToList().FirstOrDefault();
            //int paisres = idpais.Dato;
            //ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", clientes.IDFormapago);
            ViewBag.ResidenciaFiscal = new SelectList(p.Paises, "c_Pais", "Pais", elemento.ResidenciaFiscal);
            ViewBag.IDPais = new SelectList(p.Paises, "idPais", "Pais", elemento.IDPais);
            ViewBag.IDEstado = new PaisesRepository().GetEstadoPorPaisSelec(elemento.IDPais, elemento.IDEstado);
            ViewBag.IDMunicipio = new PaisesRepository().GetMunicipioPorEstadoSelect(elemento.IDEstado, elemento.IDMunicipio);
            ViewBag.IDLocalidad = new PaisesRepository().GetLocalidadPorEstadoSelec(elemento.IDEstado, elemento.IDLocalidad);
            ViewBag.IDColonia = new PaisesRepository().GetColoniaPorCPSelec(elemento.CP, elemento.IDColonia);



            return View(elemento);
        }

        // POST: Propietario/Edit/5
        [HttpPost]
        public ActionResult Edit(Propietario elemento)
        {
            PaisesContext p = new PaisesContext();
            int id = elemento.IDPropietario;
            //string RFC = colleccion.Get("RFCPropietario").ToString();
            //string Nom = colleccion.Get("NombrePropietario").ToString();
            ClsDatoString cpais = db.Database.SqlQuery<ClsDatoString>("select c_pais as Dato from paises where IDPais=" + elemento.IDPais).ToList().FirstOrDefault();
            var pais = cpais.Dato;
            ClsDatoString cedo = db.Database.SqlQuery<ClsDatoString>("select c_estado as Dato from estados where IDEstado=" + elemento.IDEstado).ToList().FirstOrDefault();
            var estado = cedo.Dato;
            ClsDatoString cmpio = db.Database.SqlQuery<ClsDatoString>("select c_Municipio as Dato from c_Municipio where IDMunicipio= " + elemento.IDMunicipio).ToList().FirstOrDefault();
            var municipio = cmpio.Dato;
            ClsDatoString cloc = db.Database.SqlQuery<ClsDatoString>("select C_Localidad as Dato from c_Localidad where IDLocalidad= " + elemento.IDLocalidad).ToList().FirstOrDefault();
            var localidad = cloc.Dato;
            ClsDatoString ccol = db.Database.SqlQuery<ClsDatoString>("select C_Colonia as Dato from c_Colonia where IDColonia = " + elemento.IDColonia).ToList().FirstOrDefault();
            var colonia = ccol.Dato;

            try
            {
                PropietarioContext db = new PropietarioContext();
                string query = "update propietario set [RFCPropietario] = '" + elemento.RFCPropietario + "' ,[NombrePropietario] = '" + elemento.NombrePropietario + "' ,[NumRegIdTribPropietario] = '" + elemento.NumRegIdTribPropietario + "' ,[Calle]  = '" + elemento.Calle + "' ,[NumExt]  = '" + elemento.NumExt + "', [NumInt] = '" + elemento.NumInt + "',[ResidenciaFiscal]  = '" + elemento.ResidenciaFiscal + "', [IDPais]  = " + elemento.IDPais + ", [c_Pais] = '" + pais + "' ,[IDEstado]  = " + elemento.IDEstado + ", [c_Estado] = '" + estado + "', [IDMunicipio] = " + elemento.IDMunicipio + ", [c_Municipio] = '" + municipio + "',[IDLocalidad] = " + elemento.IDLocalidad + ",[c_Localidad] = '" + localidad + "',[IDColonia]  = " + elemento.IDColonia + ",[c_Colonia] = '" + colonia + "',[CP] = '" + elemento.CP + "',[Referencia] = '" + elemento.Referencia + "',[Activo] = '" + elemento.Activo + "' where IDPropietario = " + id;
                db.Database.ExecuteSqlCommand(query);

                //if (TryUpdateModel(elementonuevo))
                //{
                //    db.SaveChanges();

                //}
                return RedirectToAction("Index");
            }
            catch
            {
                Propietario elementon = db.Propietario.Find(id);
                ViewBag.ResidenciaFiscal = new SelectList(p.Paises, "c_Pais", "Pais", elementon.ResidenciaFiscal);
                ViewBag.IDPais = new SelectList(p.Paises, "idPais", "Pais", elementon.IDPais);
                ViewBag.IDEstado = new PaisesRepository().GetEstadoPorPaisSelec(elementon.IDPais, elementon.IDEstado);
                ViewBag.IDMunicipio = new PaisesRepository().GetMunicipioPorEstadoSelect(elementon.IDEstado, elementon.IDMunicipio);
                ViewBag.IDLocalidad = new PaisesRepository().GetLocalidadPorEstadoSelec(elementon.IDEstado, elementon.IDLocalidad);
                ViewBag.IDColonia = new PaisesRepository().GetColoniaPorCPSelec(elementon.CP, elementon.IDColonia);

                return View(elementon);
            }
        }

        // GET: Propietario/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Propietario/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


    }
}

