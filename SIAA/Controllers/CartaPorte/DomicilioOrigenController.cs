using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.CartaPorte;

namespace SIAAPI.Controllers.CartaPorte
{
    public class DomicilioOrigenController : Controller
    {
        private OrigenContext db = new OrigenContext();

        // GET: DomicilioOrigens
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.RFCSortParm = String.IsNullOrEmpty(sortOrder) ? "RFCRemitente" : "RFCRemitente";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "NombreRemitente" : "NombreRemitente";
            ViewBag.CalleSortParm = String.IsNullOrEmpty(sortOrder) ? "Calle" : "Calle";
            ViewBag.NumExtSortParm = String.IsNullOrEmpty(sortOrder) ? "NumExt" : "NumExt";
            ViewBag.NumIntSortParm = String.IsNullOrEmpty(sortOrder) ? "NumInt" : "NumInt";
            ViewBag.PaisSortParm = String.IsNullOrEmpty(sortOrder) ? "IDPais" : "IDPais";
            ViewBag.EstadoSortParm = String.IsNullOrEmpty(sortOrder) ? "IDEstado" : "IDEstado";
            ViewBag.MunicipioSortParm = String.IsNullOrEmpty(sortOrder) ? "IDMunicipio" : "IDMunicipio";
            ViewBag.LocalidadSortParm = String.IsNullOrEmpty(sortOrder) ? "IDLocalidad" : "IDLocalidad";
            ViewBag.ColoniaSortParm = String.IsNullOrEmpty(sortOrder) ? "IDColonia" : "IDColonia";
            ViewBag.CPSortParm = String.IsNullOrEmpty(sortOrder) ? "CP" : "CP";


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
            var elementos = from s in db.Origen
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                //elementos = elementos.Where(s => s.ClaveBanco.ToUpper().Contains(searchString.ToUpper()));

                elementos = elementos.Where(s => s.Calle.Contains(searchString) || s.RFCRemitente.Contains(searchString) || s.NombreRemitente.Contains(searchString) ||
                s.CP.Contains(searchString) || s.c_Pais.Contains(searchString) || s.c_Estado.Contains(searchString) || s.c_Localidad.Contains(searchString) || s.c_Municipio.Contains(searchString) || s.c_Colonia.Contains(searchString));

            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Calle":
                    elementos = elementos.OrderBy(s => s.Calle);
                    break;
                case "RFCRemitente":
                    elementos = elementos.OrderBy(s => s.RFCRemitente);
                    break;
                case "NombreRmitente":
                    elementos = elementos.OrderBy(s => s.NombreRemitente);
                    break;
                case "CP":
                    elementos = elementos.OrderBy(s => s.CP);
                    break;
                case "c_Pais":
                    elementos = elementos.OrderBy(s => s.c_Pais);
                    break;
                case "c_Estado":
                    elementos = elementos.OrderBy(s => s.c_Estado);
                    break;
                case "c_Municipio":
                    elementos = elementos.OrderBy(s => s.c_Municipio);
                    break;
                case "c_Localidad":
                    elementos = elementos.OrderBy(s => s.c_Localidad);
                    break;
                case "c_Colonia":
                    elementos = elementos.OrderBy(s => s.c_Colonia);
                    break;
                default:
                    elementos = elementos.OrderBy(s => s.RFCRemitente);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.Origen.OrderBy(e => e.IDOrigen).Count(); // Total number of elements

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

            //return View(db.origen.ToList());
        }

        // GET: DomicilioOrigens/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Origen domicilioOrigen = db.Origen.Find(id);
            if (domicilioOrigen == null)
            {
                return HttpNotFound();
            }
            return View(domicilioOrigen);
        }

        // GET: DomicilioOrigens/Create
        public ActionResult Create()
        {
            //Paises
            var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> liP = new List<SelectListItem>();
            liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
            foreach (var a in datosPaises)
            {
                liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });

            }
            ViewBag.ListPais = liP;
            ViewBag.ListEstado = new PaisesRepository().GetEstadoPorPais(0);
            ViewBag.ListMunicipio = new PaisesRepository().GetMunicipioPorEstado(0);
            ViewBag.ListLocalidad = new PaisesRepository().GetLocalidadPorEstado(0);

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

            //ViewBag.IDCliente = Session["IDCliente"];
            OrigenContext db = new OrigenContext();
            Origen entrega = new Origen();

            //Models.Administracion.EstadosContext dbe = new Models.Administracion.EstadosContext();
            //ViewBag.IDEstado = new SelectList(dbe.Estados, "IDEstado", "Estado");

            //entrega.IDCliente = ViewBag.IDCliente;

            return View(entrega);
        }

        // POST: DomicilioOrigens/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Origen domicilioOrigen)
        {
            var datosPaises = new PaisesContext().Paises.OrderBy(i => i.Pais).ToList();
            List<SelectListItem> liP = new List<SelectListItem>();
            liP.Add(new SelectListItem { Text = "Selecciona un Pais", Value = "0" });
            foreach (var a in datosPaises)
            {
                liP.Add(new SelectListItem { Text = a.Pais, Value = a.IDPais.ToString() });

            }
            ViewBag.ListPais = liP;
            ViewBag.ListEstado = new PaisesRepository().GetEstadoPorPais(0);
            ViewBag.ListMunicipio = new PaisesRepository().GetMunicipioPorEstado(0);
            ViewBag.ListLocalidad = new PaisesRepository().GetLocalidadPorEstado(0);

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

            try
            {
                Paises clavePais = new PaisesContext().Paises.Find(domicilioOrigen.IDPais);
                Models.Administracion.Estados estados = new Models.Administracion.EstadosContext().Estados.Find(domicilioOrigen.IDEstado);
                c_Municipio municipio = new c_MunicipioContext().municipio.Find(domicilioOrigen.IDMunicipio);
                c_Localidad localidad = new c_LocalidadContext().Localidad.Find(domicilioOrigen.IDLocalidad);
                c_Colonia colonia = new c_ColoniaContext().colonias.Find(domicilioOrigen.IDColonia);

                //Int32 iid = Int32.Parse(Session["IDCliente"].ToString());
                //domicilioOrigen.IDCliente = iid;
                domicilioOrigen.IDEmpresa = 2;
                domicilioOrigen.c_Pais = clavePais.c_Pais;
                domicilioOrigen.c_Estado = estados.c_Estado;
                domicilioOrigen.c_Municipio = municipio.C_Municipio;
                domicilioOrigen.c_Localidad = localidad.C_Localidad;
                domicilioOrigen.c_Colonia = colonia.C_Colonia;
                

                string cadena = "insert into[dbo].[Origen]([IDEmpresa],[RFCRemitente],[NombreRemitente],[Calle],[NumExt],[NumInt],[IDPais],[c_Pais],[IDEstado],[c_Estado],[IDMunicipio],[c_Municipio],[IDLocalidad],[c_Localidad],[IDColonia],[c_Colonia],[CP],[Referencia],[Activo])";
                cadena = cadena + "values (" + domicilioOrigen.IDEmpresa + ", '" + domicilioOrigen.RFCRemitente + "', '" + domicilioOrigen.NombreRemitente + "','" + domicilioOrigen.Calle + "', '" + domicilioOrigen.NumExt + "','" + domicilioOrigen.NumInt + "'," + domicilioOrigen.IDPais + ",'" + domicilioOrigen.c_Pais + "', " + domicilioOrigen.IDEstado + ", '" + domicilioOrigen.c_Estado + "'," + domicilioOrigen.IDMunicipio + ",'" + domicilioOrigen.c_Municipio + "'," + domicilioOrigen.IDLocalidad + ",'" + domicilioOrigen.c_Localidad + "'," + domicilioOrigen.IDColonia + ",'" + domicilioOrigen.c_Colonia + "','" + domicilioOrigen.CP + "','" + domicilioOrigen.Referencia + "','" + domicilioOrigen.Activo +"')";
                db.Database.ExecuteSqlCommand(cadena);


                //db.Entregas.Add(entrega);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception err)
            {
                string mensaje = err.Message;
                //Models.Administracion.EstadosContext dbe = new Models.Administracion.EstadosContext();
                //ViewBag.IDEstado = new SelectList(dbe.Estados, "IDEstado", "Estado");
                return View();
            }
        }

        // GET: DomicilioOrigens/Edit/5
        public ActionResult Edit(int id)
        {
            Origen elementonuevo = db.Origen.Find(id);

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

        // POST: DomicilioOrigens/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Origen domicilioOrigen)
        {
            Origen elemento = db.Origen.Find(domicilioOrigen.IDOrigen);
            try
            {

                if (TryUpdateModel(elemento))
                {
                    db.SaveChanges();
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

        // GET: DomicilioOrigens/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Origen domicilioOrigen = db.Origen.Find(id);
            if (domicilioOrigen == null)
            {
                return HttpNotFound();
            }
            return View(domicilioOrigen);
        }

        // POST: DomicilioOrigens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Origen domicilioOrigen = db.Origen.Find(id);
            db.Origen.Remove(domicilioOrigen);
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

    }
}
