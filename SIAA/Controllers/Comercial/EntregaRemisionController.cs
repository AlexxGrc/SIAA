using PagedList;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using SIAAPI.Reportes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Comercial
{
    public class EntregaRemisionController : Controller
    {
        EntregaRemisionesContext db = new EntregaRemisionesContext();
        RemisionContext BD = new RemisionContext();
        // GET: EntregaRemision
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.ClaveSortParm = String.IsNullOrEmpty(sortOrder) ? "Chofer" : "Chofer";
            ViewBag.NombreSortParm = String.IsNullOrEmpty(sortOrder) ? "Ruta" : "Ruta";
            // Not sure here
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            // Pass filtering string to view in order to maintain filtering when paging
            ViewBag.SearchString = searchString;
            //Paginación
            var elementos = from s in db.VEntregas
                            select s;
            //Busqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                elementos = elementos.Where(s => s.Chofer.Contains(searchString) || s.Ruta.Contains(searchString) || s.ID.ToString().Contains(searchString));
            }

            //Ordenacion

            switch (sortOrder)
            {
                case "Chofer":
                    elementos = elementos.OrderBy(s => s.Chofer);
                    break;
                case "Ruta":
                    elementos = elementos.OrderBy(s => s.Ruta);
                    break;
                default:
                    elementos = elementos.OrderByDescending(s => s.ID);
                    break;
            }

            //Paginación
            // DROPDOWNLIST FOR UPDATING PAGE SIZE
            int count = db.EntregaRemisiones.OrderBy(e => e.ID).Count(); // Total number of elements

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



            var lista = db.VEntregas.ToList();
            List<VEntregaR> listAux = new List<VEntregaR>();

            foreach (var e in (IEnumerable<VEntregaR>)lista)
            {
                bool exist = listAux.Exists(
                    delegate (VEntregaR entrega)
                    {
                        return (entrega.Fecha == e.Fecha) && (entrega.Chofer == e.Chofer) && (entrega.Ruta == e.Ruta);
                    });

                if (!exist)
                {
                    listAux.Add(e);
                }
            }
            //return View(listAux);
            return View(elementos.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult CreateEntrega(int IDEntrega = 0)
        {
            ViewBag.IDEntrega = IDEntrega;
            try
            {
                EntregaRemision entrega = new EntregaRemisionesContext().Database.SqlQuery<EntregaRemision>("select*from entregaremision where id=" + IDEntrega).ToList().FirstOrDefault();

                ViewBag.Mensaje = "";

                ViewBag.Rutas = new RutaRepository().GetRuta(entrega.IDRuta);
                ViewBag.Choferes = new ChoferRepository().GetChofer(entrega.IDChofer);
                ViewBag.Fecha = Convert.ToString(entrega.Fecha);
                ViewBag.FechaV = entrega.Fecha;
                return View();
            }
            catch (Exception)
            {

            }
            ViewBag.Fecha = "";
            ViewBag.Rutas = new RutaRepository().GetRuta();
            ViewBag.Choferes = new ChoferRepository().GetChofer();
            ViewBag.Mensaje = "";
            ViewBag.Lista = null;
            return View();
        }

        [HttpPost]
        public ActionResult CreateEntrega(EntregaRemision elemento, FormCollection collection)
        {
            ViewBag.Mensaje = "";
            bool vienedeUna = false;
            int Factura = 0;
            try
            {
                Factura = int.Parse(collection.Get("Factura"));
            }
            catch (Exception)
            {

            }
            int IDEntregaR = 0;
            try
            {
                IDEntregaR = int.Parse(collection.Get("ID"));
            }
            catch (Exception)
            {

            }
            if (Factura == 0)
            {
                ViewBag.Mensaje = "No ingreso numero de factura";
            }

            Chofer chofer = new ChoferesContext().Choferes.Find(elemento.IDChofer);
            Ruta ruta = new RutasContext().Rutas.Find(elemento.IDRuta);
            ViewBag.fecha = DateTime.Now;
            ViewBag.chofer = chofer.IDChofer;
            ViewBag.ruta = ruta.IDRuta;
            try
            {
                EncfacturasSaldos enc = new EncfacturasSaldosContext().Database.SqlQuery<EncfacturasSaldos>("select*from EncfacturasSaldos where numero=" + Factura + " and serie='B'").ToList().FirstOrDefault();

                //rastrear remisiones
                string[] numeroP = enc.Prefactura.Split(' ');
                int NumPre = int.Parse(numeroP[1]);

                EncPrefactura prefactura = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("select*from encprefactura where numero=" + NumPre).ToList().FirstOrDefault();
                SIAAPI.clasescfdi.ClsRastreaDA rastrea = new SIAAPI.clasescfdi.ClsRastreaDA();
                List<SIAAPI.clasescfdi.NodoTrazo> nodos = rastrea.getDocumentoAnterior("Prefactura", prefactura.IDPrefactura, "Encabezado");
                if (nodos.Count() > 0)
                {
                    DetEntregaRemision fac = new EntregaRemisionesContext().Database.SqlQuery<DetEntregaRemision>("select*from DetEntregaRemision where idfactura=" + enc.ID).FirstOrDefault();
                    if (fac != null)
                    {
                        EntregaRemision entrega = new EntregaRemisionesContext().Database.SqlQuery<EntregaRemision>("select*from entregaremision where id=" + fac.IDEntregaR).ToList().FirstOrDefault();

                        ViewBag.Rutas = new RutaRepository().GetRuta(entrega.IDRuta);
                        ViewBag.Choferes = new ChoferRepository().GetChofer(entrega.IDChofer);
                        ViewBag.Fecha = entrega.Fecha;
                        ViewBag.Mensaje = "Factura existente";
                        return View();
                    }
                    else
                    {
                        if (IDEntregaR == 0)
                        {
                            string cadena = "insert into EntregaRemision (Fecha,IDRuta,IDChofer,status) values" +
                                "(SYSDATETIME()," + elemento.IDRuta + "," + elemento.IDChofer + ",'Activo')";
                            db.Database.ExecuteSqlCommand(cadena);
                        }
                        else
                        {
                            vienedeUna = true;
                        }

                    }

                }
                int IDEntrega = 0;
                List<EntregaRemision> numero;
                numero = db.Database.SqlQuery<EntregaRemision>("SELECT * FROM [dbo].[EntregaRemision] WHERE ID = (SELECT MAX(ID) from EntregaRemision)").ToList();
                IDEntrega = numero.Select(s => s.ID).FirstOrDefault();
                if (vienedeUna)
                {
                    IDEntrega = IDEntregaR;
                }
                foreach (SIAAPI.clasescfdi.NodoTrazo nodo in nodos)
                {

                    EncRemision remision = new RemisionContext().EncRemisiones.Find(nodo.ID);
                    if (remision == null)
                    {
                        ViewBag.Mensaje = "Remisión inexistente";
                    }
                    try
                    {
                        int idremisioncount = db.Database.SqlQuery<int>("SELECT count(IDRemision) from DetEntregaRemision WHERE IDRemision = " + nodo.ID).FirstOrDefault();
                        if (idremisioncount == 0)
                        {

                            string cadena = "insert into DetEntregaRemision (identregar,idremision,entregado, idfactura) values" +
                                "(" + IDEntrega + "," + nodo.ID + ",'0'," + enc.ID + ")";
                            db.Database.ExecuteSqlCommand(cadena);
                            ViewBag.Mensaje = "";
                        }
                        else
                        {
                            ViewBag.Mensaje = "La remisión ingresada ya contiene una ruta";
                        }

                    }
                    catch
                    {
                        ViewBag.Rutas = new RutaRepository().GetRuta();
                        ViewBag.Choferes = new ChoferRepository().GetChofer();

                        ViewBag.Mensaje = ViewBag.Mensaje;
                        return View();
                    }

                }
                ViewBag.IDEntrega = IDEntrega;

                ViewBag.Mensaje = "";

                return RedirectToAction("CreateEntrega", new { IDEntrega = IDEntrega });
            }
            catch (Exception err)
            {
                ViewBag.Mensaje = "";

            }

            ViewBag.Mensaje = "";



            return RedirectToAction("Index");

        }


        public ActionResult EditEntrega(int IDEntrega)
        {
            ViewBag.IDEntrega = IDEntrega;
            try
            {
                EntregaRemision entrega = new EntregaRemisionesContext().Database.SqlQuery<EntregaRemision>("select*from entregaremision where id=" + IDEntrega).ToList().FirstOrDefault();

                ViewBag.Mensaje = "";

                ViewBag.Rutas = new RutaRepository().GetRuta(entrega.IDRuta);
                ViewBag.Choferes = new ChoferRepository().GetChofer(entrega.IDChofer);
                ViewBag.Fecha = Convert.ToString(entrega.Fecha);
                ViewBag.FechaV = entrega.Fecha;
                return View();
            }
            catch (Exception err)
            {

            }
            ViewBag.Fecha = "";
            ViewBag.Rutas = new RutaRepository().GetRuta();
            ViewBag.Choferes = new ChoferRepository().GetChofer();
            ViewBag.Mensaje = "";
            ViewBag.Lista = null;
            return View();

        }

        [HttpPost]
        public ActionResult EditEntrega(EntregaRemision elemento, FormCollection collection)
        {
            ViewBag.Mensaje = "";
            bool vienedeUna = false;
            int Factura = 0;
            try
            {
                Factura = int.Parse(collection.Get("Factura"));
            }
            catch (Exception err)
            {

            }
            int IDEntregaR = 0;
            try
            {
                IDEntregaR = int.Parse(collection.Get("ID"));
            }
            catch (Exception)
            {

            }
            if (Factura == 0)
            {
                ViewBag.Mensaje = "No ingreso numero de factura";
            }

            Chofer chofer = new ChoferesContext().Choferes.Find(elemento.IDChofer);
            Ruta ruta = new RutasContext().Rutas.Find(elemento.IDRuta);
            ViewBag.fecha = DateTime.Now;
            ViewBag.chofer = chofer.IDChofer;
            ViewBag.ruta = ruta.IDRuta;
            try
            {
                EncfacturasSaldos enc = new EncfacturasSaldosContext().Database.SqlQuery<EncfacturasSaldos>("select*from EncfacturasSaldos where numero=" + Factura + " and serie='B'").ToList().FirstOrDefault();

                //rastrear remisiones
                string[] numeroP = enc.Prefactura.Split(' ');
                int NumPre = int.Parse(numeroP[1]);

                EncPrefactura prefactura = new PrefacturaContext().Database.SqlQuery<EncPrefactura>("select*from encprefactura where numero=" + NumPre).ToList().FirstOrDefault();
                SIAAPI.clasescfdi.ClsRastreaDA rastrea = new SIAAPI.clasescfdi.ClsRastreaDA();
                List<SIAAPI.clasescfdi.NodoTrazo> nodos = rastrea.getDocumentoAnterior("Prefactura", prefactura.IDPrefactura, "Encabezado");
                if (nodos.Count() > 0)
                {
                    DetEntregaRemision fac = new EntregaRemisionesContext().Database.SqlQuery<DetEntregaRemision>("select*from DetEntregaRemision where idfactura=" + enc.ID).FirstOrDefault();
                    if (fac != null)
                    {
                        EntregaRemision entrega = new EntregaRemisionesContext().Database.SqlQuery<EntregaRemision>("select*from entregaremision where id=" + fac.IDEntregaR).ToList().FirstOrDefault();

                        ViewBag.Rutas = new RutaRepository().GetRuta(entrega.IDRuta);
                        ViewBag.Choferes = new ChoferRepository().GetChofer(entrega.IDChofer);
                        ViewBag.Fecha = entrega.Fecha;
                        ViewBag.Mensaje = "Factura existente";
                        return View();
                    }
                    else
                    {
                        if (IDEntregaR == 0)
                        {
                            string cadena = "insert into EntregaRemision (Fecha,IDRuta,IDChofer,status) values" +
                                "(SYSDATETIME()," + elemento.IDRuta + "," + elemento.IDChofer + ",'Activo')";
                            db.Database.ExecuteSqlCommand(cadena);
                        }
                        else
                        {
                            vienedeUna = true;
                        }

                    }

                }
                int IDEntrega = 0;
                List<EntregaRemision> numero;
                numero = db.Database.SqlQuery<EntregaRemision>("SELECT * FROM [dbo].[EntregaRemision] WHERE ID = (SELECT MAX(ID) from EntregaRemision)").ToList();
                IDEntrega = numero.Select(s => s.ID).FirstOrDefault();
                if (vienedeUna)
                {
                    IDEntrega = IDEntregaR;
                }
                foreach (SIAAPI.clasescfdi.NodoTrazo nodo in nodos)
                {

                    EncRemision remision = new RemisionContext().EncRemisiones.Find(nodo.ID);
                    if (remision == null)
                    {
                        ViewBag.Mensaje = "Remisión inexistente";
                    }
                    try
                    {
                        int idremisioncount = db.Database.SqlQuery<int>("SELECT count(IDRemision) from DetEntregaRemision WHERE IDRemision = " + nodo.ID).FirstOrDefault();
                        if (idremisioncount == 0)
                        {

                            string cadena = "insert into DetEntregaRemision (identregar,idremision,entregado, idfactura) values" +
                                "(" + IDEntrega + "," + nodo.ID + ",'0'," + enc.ID + ")";
                            db.Database.ExecuteSqlCommand(cadena);
                            ViewBag.Mensaje = "";
                        }
                        else
                        {
                            ViewBag.Mensaje = "La remisión ingresada ya contiene una ruta";
                        }

                    }
                    catch
                    {
                        ViewBag.Rutas = new RutaRepository().GetRuta();
                        ViewBag.Choferes = new ChoferRepository().GetChofer();

                        ViewBag.Mensaje = ViewBag.Mensaje;
                        return View();
                    }

                }
                ViewBag.IDEntrega = IDEntrega;

                ViewBag.Mensaje = "";

                return RedirectToAction("EditEntrega", new { IDEntrega = IDEntrega });
            }
            catch (Exception err)
            {
                ViewBag.Mensaje = "";

            }

            ViewBag.Mensaje = "";



            return RedirectToAction("Index");

        }
        public ActionResult DetEntrega(DateTime fecha, string chofer, string ruta)
        {
            int idChofer = 0;
            try
            {
                ClsDatoEntero clientecapturado = db.Database.SqlQuery<ClsDatoEntero>("select IDChofer AS dato from Chofer where  NOMBRE='" + chofer + "'").ToList().FirstOrDefault();
                idChofer = clientecapturado.Dato;
            }

            catch
            {

            }

            int idRuta = 0;

            try
            {
                ClsDatoEntero clientecapturado = db.Database.SqlQuery<ClsDatoEntero>("select IDRuta AS dato from Ruta where  DESCRIPCION='" + ruta + "'").ToList().FirstOrDefault();
                idRuta = clientecapturado.Dato;
            }

            catch
            {

            }
            string ff = fecha.ToString("yyyy-MM-dd");
            string listaR = "SELECT r.IDRemision, R.FECHA,R.Subtotal, R.IVA, R.Total, R.IDCliente, C.Nombre as Cliente, R.Entrega FROM EncRemision R, EntregaRemision E, Clientes C where R.IDCliente=C.IDCliente and R.IDRemision=E.IDRemision  and cast(e.FECHA as date) like '%" + ff + "%' and E.IDChofer= " + idChofer + "and E.IDRuta= " + idRuta;
            var lista = BD.Database.SqlQuery<EncRemisionEntrega>(listaR).ToList();
            ViewBag.Lista = lista;

            return View(lista);
        }

        //public ActionResult Details(DateTime fecha, string chofer, string ruta)
        //{
        //    int idChofer = 0;

        //    try
        //    {
        //        ClsDatoEntero clientecapturado = db.Database.SqlQuery<ClsDatoEntero>("select IDChofer AS dato from Chofer where  NOMBRE='" + chofer + "'").ToList()[0];
        //        idChofer = clientecapturado.Dato;
        //    }

        //    catch
        //    {

        //    }

        //    int idRuta = 0;

        //    try
        //    {
        //        ClsDatoEntero clientecapturado = db.Database.SqlQuery<ClsDatoEntero>("select IDRuta AS dato from Ruta where  DESCRIPCION='" + ruta + "'").ToList()[0];
        //        idRuta  = clientecapturado.Dato;
        //    }

        //    catch
        //    {

        //    }

        //    ViewBag.Chofer = chofer;
        //    ViewBag.Ruta = ruta;
        //    ViewBag.Fecha = fecha;
        //    string ff = fecha.ToString("yyyy-MM-dd");

        //    string listaR = "SELECT r.IDRemision, R.FECHA,R.Subtotal, R.IVA, R.Total, R.IDCliente, C.Nombre as Cliente, R.Entrega FROM EncRemision R, EntregaRemision E, Clientes C where R.IDCliente=C.IDCliente and R.IDRemision=E.IDRemision  and cast(e.FECHA as date) like '%" + ff + "%' and E.IDChofer= " + idChofer + "and E.IDRuta= " + idRuta;
        //    var lista = BD.Database.SqlQuery<EncRemisionEntrega>(listaR).ToList();
        //    return View(lista);
        //}

        public ActionResult Details(int IDEntrega)
        {

            EntregaRemision entrega = new EntregaRemisionesContext().EntregaRemisiones.Find(IDEntrega);
            Ruta ruta = new RutasContext().Rutas.Find(entrega.IDRuta);
            Chofer chofer = new ChoferesContext().Choferes.Find(entrega.IDChofer);
            ViewBag.Chofer = chofer.Nombre;
            ViewBag.Ruta = ruta.Descripcion;
            string listaR = "select*from VDetEntregaRemision where identregar=" + IDEntrega;
            var lista = BD.Database.SqlQuery<VDetEntregaRemision>(listaR).ToList();
            if (lista.Count() == 0)
            {
                listaR = "select*from VDetEntregaRemisionSF where identregar=" + IDEntrega;
                lista = BD.Database.SqlQuery<VDetEntregaRemision>(listaR).ToList();

            }
            return View(lista);
        }

        public ActionResult FinalizarEntrega(int IDEntrega)
        {

            try
            {
                string finaenca = "update entregaremision set fechafinalizacion=SYSDATETIME(), status='Finalizado' where id=" + IDEntrega;
                string finadet = "update DetEntregaRemision set entregado='1' where identregar=" + IDEntrega;
                db.Database.ExecuteSqlCommand(finaenca);
                db.Database.ExecuteSqlCommand(finadet);
            }
            catch (Exception err)
            {

            }

            return RedirectToAction("Index");
        }


        public void Imprimir(int IDEntrega, Empresa em)
        {
            EntregaRemisionesContext db = new EntregaRemisionesContext();
            EmpresaContext dbe = new EmpresaContext();
            var empresa = dbe.empresas.Single(m => m.IDEmpresa == 2);



            System.Drawing.Image logoempresa = byteArrayToImage(empresa.Logo);

            try
            {

                EntregaRemision entregaRemision = new EntregaRemisionesContext().EntregaRemisiones.Find(IDEntrega);
                CreaEntregaPDF documento = new CreaEntregaPDF(logoempresa, entregaRemision.ID, empresa);

            }
            catch (Exception err)
            {
                string mensajederror = err.Message;
            }
            RedirectToAction("Index");

        }

        public System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
        {
            System.Drawing.Image returnImage = null;
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn, 0, byteArrayIn.Length);
                ms.Write(byteArrayIn, 0, byteArrayIn.Length);
                returnImage = System.Drawing.Image.FromStream(ms, true);//Exception occurs here
            }
            catch { }
            return returnImage;

        }


        [HttpPost]
        public JsonResult DeleteRemision(int id)
        {
            try
            {

                db.Database.ExecuteSqlCommand("delete from [dbo].[EntregaRemision] where idRemision=" + id);
                return Json(true);
            }
            catch (Exception err)
            {
                return Json(500, err.Message);
            }
        }


    }

    public class Combos
    {

        public IEnumerable<SelectListItem> GetRuta(int IDRuta)
        {
            using (var context = new RutasContext())
            {
                List<SelectListItem> lista = context.Rutas.AsNoTracking()
                .Where(n => n.IDRuta == IDRuta)
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDRuta.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Ruta---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }

        }
        public IEnumerable<SelectListItem> GetChofer(int IDChofer)
        {
            using (var context = new ChoferesContext())
            {
                List<SelectListItem> lista = context.Choferes.AsNoTracking()
                    .Where(n => n.IDChofer == IDChofer)
                    .OrderBy(n => n.Nombre)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDChofer.ToString(),
                            Text = n.Nombre
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un Chofer---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }
    }
}

