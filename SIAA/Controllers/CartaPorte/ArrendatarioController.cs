using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SIAAPI.Models.CartaPorte;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using SIAAPI.ViewModels.CartaPorte;
using System.Net;


namespace SIAAPI.Controllers.CartaPorte
{
    public class ArrendatarioController : Controller
    {
        private ArrendatarioContext db = new ArrendatarioContext();
        private VArrendatarioContext dbv = new VArrendatarioContext();
        // GET: Arrendatario
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "RFCArrendatario" : "RFCArrendatario";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "NombreArrendatario" : "NombreArrendatario";
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
            var elementos = from s in db.Arrendatario
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {

                elementos = elementos.Where(s => s.RFCArrendatario.Contains(searchString) || s.NombreArrendatario.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "RFCArrendatario":
                    elementos = elementos.OrderBy(s => s.RFCArrendatario);
                    break;
                case "NombreArrendatario":
                    elementos = elementos.OrderBy(s => s.NombreArrendatario);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.NombreArrendatario);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Arrendatario.OrderBy(e => e.IDArrendatario).Count(); // Total number of elements

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

        // GET: Arrendatario/Details/5
        public ActionResult Details(int id)
        {
            var elemento = db.Database.SqlQuery<VArrendatario>("select * from VArrendatario where IDArrendatario = " + id).ToList().FirstOrDefault();
            ViewBag.data = elemento;
            return View(elemento);
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


        // GET: Arrendatario/Create
        public ActionResult Create()
        {

            Arrendatario elemento = new Arrendatario();
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

            var Colonias = new c_ColoniaContext().Database.SqlQuery<c_Colonia>("select * from c_Colonia").ToList();
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

        // POST: Arrendatario/Create
        [HttpPost]
        public ActionResult Create(Arrendatario elementonuevo, FormCollection collection)
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
            elementonuevo.c_Colonia = ccol.Dato;
            if (elementonuevo.Activo == false)
            {
                elementonuevo.Activo =true;
            }

            try
            {
                // TODO: Add insert logic here

                db.Arrendatario.Add(elementonuevo);
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

                var Colonias = new c_ColoniaContext().Database.SqlQuery<c_Colonia>("select * from c_Colonia").ToList();
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

        // GET: Arrendatario/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Arrendatario elemento = db.Arrendatario.Find(id);
            if (elemento == null)
            {
                return HttpNotFound();
            }
            PaisesContext p = new PaisesContext();
            //ClsDatoEntero idpais = db.Database.SqlQuery<ClsDatoEntero>("select IDpais as Dato from paises where c_pais='" + elemento.ResidenciaFiscal+ "'").ToList().FirstOrDefault();
            //int paisres = idpais.Dato;
            //ViewBag.IDFormapago = new SelectList(db.c_FormaPagos, "IDFormaPago", "Descripcion", clientes.IDFormapago);
            //ViewBag.ResidenciaFiscal = new SelectList(p.Paises, "c_Pais", "Pais", elemento.ResidenciaFiscal);
            ViewBag.IDPais = new SelectList(p.Paises, "idPais", "Pais", elemento.IDPais);
            ViewBag.IDEstado = new PaisesRepository().GetEstadoPorPaisSelec(elemento.IDPais, elemento.IDEstado);
            ViewBag.IDMunicipio = new PaisesRepository().GetMunicipioPorEstadoSelect(elemento.IDEstado, elemento.IDMunicipio);
            ViewBag.IDLocalidad = new PaisesRepository().GetLocalidadPorEstadoSelec(elemento.IDEstado, elemento.IDLocalidad);
            ViewBag.IDColonia = new PaisesRepository().GetColoniaPorCPSelec(elemento.CP, elemento.IDColonia);



            return View(elemento);
        }

        // POST: Arrendatario/Edit/5
        [HttpPost]
        public ActionResult Edit(Arrendatario elemento)
        {
            PaisesContext p = new PaisesContext();
            int id = elemento.IDArrendatario;
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
                ArrendatarioContext db = new ArrendatarioContext();
                string query = "update Arrendatario set [RFCArrendatario] = '" + elemento.RFCArrendatario + "' ,[NombreArrendatario] = '" + elemento.NombreArrendatario + "' ,[NumRegIdTribArrendatario] = '" + elemento.NumRegIdTribArrendatario + "' ,[Calle]  = '" + elemento.Calle + "' ,[NumExt]  = '" + elemento.NumExt + "', [NumInt] = '" + elemento.NumInt + "', [IDPais]  = " + elemento.IDPais + ", [c_Pais] = '" + pais + "' ,[IDEstado]  = " + elemento.IDEstado + ", [c_Estado] = '" + estado + "', [IDMunicipio] = " + elemento.IDMunicipio + ", [c_Municipio] = '" + municipio + "',[IDLocalidad] = " + elemento.IDLocalidad + ",[c_Localidad] = '" + localidad + "',[IDColonia]  = " + elemento.IDColonia + ",[c_Colonia] = '" + colonia + "',[CP] = '" + elemento.CP + "',[Referencia] = '" + elemento.Referencia + "',[Activo] = '" + elemento.Activo + "' where IDArrendatario = " + id;
                db.Database.ExecuteSqlCommand(query);

                //if (TryUpdateModel(elementonuevo))
                //{
                //    db.SaveChanges();

                //}
                return RedirectToAction("Index");
            }
            catch
            {
                Arrendatario elementon = db.Arrendatario.Find(id);
                //ViewBag.ResidenciaFiscal = new SelectList(p.Paises, "c_Pais", "Pais", elementon.ResidenciaFiscal);
                ViewBag.IDPais = new SelectList(p.Paises, "idPais", "Pais", elementon.IDPais);
                ViewBag.IDEstado = new PaisesRepository().GetEstadoPorPaisSelec(elementon.IDPais, elementon.IDEstado);
                ViewBag.IDMunicipio = new PaisesRepository().GetMunicipioPorEstadoSelect(elementon.IDEstado, elementon.IDMunicipio);
                ViewBag.IDLocalidad = new PaisesRepository().GetLocalidadPorEstadoSelec(elementon.IDEstado, elementon.IDLocalidad);
                ViewBag.IDColonia = new PaisesRepository().GetColoniaPorCPSelec(elementon.CP, elementon.IDColonia);

                return View(elementon);
            }
        }

        // GET: Arrendatario/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Arrendatario/Delete/5
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
