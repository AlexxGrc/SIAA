using PagedList;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.Models.Produccion;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Produccion
{
    public class MesaControlController : Controller
    {
        // GET: MesaControl
        private VOrdenProduccionPedidoContext db = new VOrdenProduccionPedidoContext();
        public ActionResult Index(string currentFilter, string searchString, int? page, int? PageSize, FormCollection coleccion, int? id = 0,String Estado = "")
        {
            int pageNumber = 1;
            int pageSize = 10;
            int count = 0;

          
            EstadoOrdenContext dbes = new EstadoOrdenContext();

            List<SelectListItem> estad = new List<SelectListItem>();
            SelectListItem nuevoestado = new SelectListItem();
            nuevoestado.Value = "En conflicto";
            nuevoestado.Text = "En conflicto";
            nuevoestado.Selected = true;
            estad.Add(nuevoestado);
             nuevoestado = new SelectListItem();
            nuevoestado.Value = "En Revision";
            nuevoestado.Text = "En Revision";

            estad.Add(nuevoestado);
            nuevoestado = new SelectListItem();
            nuevoestado.Value = "Lista";
            nuevoestado.Text = "Lista";

            estad.Add(nuevoestado);

            estad.Insert(0, (new SelectListItem { Text = "Ver estados", Value = "" }));
            ViewBag.Estados = estad;
         
           


           
            List<VOrdenProduccionPedido> elementos = db.Database.SqlQuery<VOrdenProduccionPedido>("select * from VOrdenProduccionPedido op where op.EstadoOrden = 'Conflicto' or op.EstadoOrden = 'Lista'  or op.EstadoOrden = 'En Revision' order by IDOrden desc").ToList();

            ViewBag.Conteo = elementos.Count();

            ViewBag.EnConflicto = elementos.Where(s => s.EstadoOrden == "Conflicto").Count();
            ViewBag.EnRevision = elementos.Where(s => s.EstadoOrden == "En Revision").Count();
            ViewBag.Listas = elementos.Where(s => s.EstadoOrden == "Lista").Count();


            if (Estado != "" )
            {
                 elementos = elementos.Where(op => op.EstadoOrden == Estado).ToList();
                //Paginación
                // DROPDOWNLIST FOR UPDATING PAGE SIZE
                ViewBag.Estado = Estado;

            }
            
           
           if (id != 0)
            {
                elementos = elementos.Where(op => op.IDOrden == id).ToList();
            }
           

         if (!string.IsNullOrEmpty(searchString))
                {

                elementos = elementos.Where(op => op.Cliente.Contains(searchString) || op.Descripcion.Contains(searchString) || op.Clave.Contains(searchString)  || op.Presentacion.Contains(searchString)).ToList();
               


            }

            count = elementos.Count(); // Total number of elements
            ViewBag.Conteo = count;
            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50"},
                new SelectListItem { Text = "100", Value = "100", Selected = true  },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            pageNumber = (page ?? 1);
            pageSize = (PageSize ?? 100);
            ViewBag.page = pageNumber;
            ViewBag.psize = pageSize;
            return View(elementos.ToPagedList(pageNumber, pageSize));
        }



        public ActionResult EnRevision(int IDOrden, int page, int PageSize)
        {
            OrdenProduccionContext dbpro = new OrdenProduccionContext();
            OrdenProduccionContext dbce = new OrdenProduccionContext();
            OrdenProduccion orden = new OrdenProduccionContext().OrdenesProduccion.Find(IDOrden);
            EncPedido pedido = new PedidoContext().EncPedidos.Find(orden.IDPedido);
            var detallepedido = new PedidoContext().DetPedido.Where(s => s.IDPedido == orden.IDPedido && s.IDArticulo == orden.IDArticulo && s.Caracteristica_ID == orden.IDCaracteristica).FirstOrDefault();

            string estadoActual = orden.EstadoOrden;
            string cadena1 = " select * from dbo.OrdenProduccion where [IDOrden]= " + IDOrden + "";
            OrdenProduccion ordenp = dbpro.Database.SqlQuery<OrdenProduccion>(cadena1).ToList().FirstOrDefault();
            ViewBag.OrdenP = ordenp;
            ViewBag.IDOrden = IDOrden;

            try
            {

                User userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList().FirstOrDefault();

                int AlmacenViene = 0;


                string estadoOrden = "En Revision";

                Caracteristica carateristica = new WorkinProcessContext().Database.SqlQuery<Caracteristica>("select * from caracteristica where id=" + ordenp.IDCaracteristica).ToList().FirstOrDefault();

                InventarioAlmacen inv = new InventarioAlmacenContext().InventarioAlmacenes.ToList().Where(s => s.IDAlmacen == AlmacenViene && s.IDCaracteristica == ordenp.IDCaracteristica).FirstOrDefault();


                string estadoProceso = "";

                estadoProceso = "Conflicto";



                string motivo = string.Empty;



                if (estadoOrden == "Conflicto" || estadoOrden == "En Revision")
                {
                    string cadenaP = "Delete from prioridades where idorden=" + IDOrden;
                    dbce.Database.ExecuteSqlCommand(cadenaP);

                }




                motivo = "Se reviso la orden";
                /// verificar automata proceso
                string cadena3 = " insert into dbo.CambioEstado(IDOrden, fecha, hora, EstadoAnterior, EstadoActual, motivo, usuario) values (" + ViewBag.IDOrden + ",'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "','" + estadoActual + "', '" + estadoOrden + "', '" + motivo + "','" + userid.Username + "')";
                dbce.Database.ExecuteSqlCommand(cadena3);

                string cadena4 = " Update dbo.OrdenProduccion set EstadoOrden='" + estadoOrden + "', prioridad=0 where IDOrden= " + IDOrden + "";
                dbpro.Database.ExecuteSqlCommand(cadena4);

                db.Database.ExecuteSqlCommand("update OrdenProduccionDetalle set EstadoProceso='" + estadoProceso + "' where IDOrden= " + IDOrden);



                return RedirectToAction("Index", new { page = page, PageSize = PageSize });
            }

            catch (Exception ERR)
            {
                return RedirectToAction("Index", new { page = page, PageSize = PageSize });
            }
        }

    }
}