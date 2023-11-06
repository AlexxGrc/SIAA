using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Comercial
{
    public class EstadisticasController : Controller
    {
        // GET: Estadisticas
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompraMateriaPrima()
        {
            return View();
        }

        public ActionResult ProduccionMateriaPrima()
        {
            return View();
        }

        [HttpGet]
        public JsonResult getProduccionmateriaPrima(string buscar, string fechaInicio, string fechaFinal)
        {
            decimal resultado = 0;

            try
            {
                //string nuevacadena = "select SUM(P.entregado) as Dato from materialasignado P inner join Articulo A on P.idmapri=A.IDArticulo inner join OrdenProduccion OP on P.Orden = OP.IDOrden where(OP.EstadoOrden = 'Finalizada' or OP.EstadoOrden = 'Iniciada')  and(OP.FechaCreacion between '"+ fechaInicio +" 00:00:00.1' and '"+ fechaFinal +" 23:59:59.999')and A.Cref like '"+ buscar +"%'";
                string cadena = "select SUM(P.Cantidad) as Dato from (ArticuloProduccion P inner join Articulo A on P.IDArticulo=A.IDArticulo)inner join OrdenProduccion OP on P.IDOrden=OP.IDOrden and (OP.EstadoOrden='Finalizada' or OP.EstadoOrden='Iniciada') and (OP.FechaCreacion between '" + fechaInicio + " 00:00:00.1' and '" + fechaFinal + " 23:59:59.999')and A.Cref like '%" + buscar + "%'";
                ClsDatoDecimal Total = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>(cadena).FirstOrDefault();
                resultado = Total.Dato;

                if (resultado == null)
                {
                    resultado = 0;
                }
            }
            catch(Exception err)
            {

            }

           

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult getCompramateriaPrima(string buscar, string fechaInicio, string fechaFinal)
        {

            decimal resultado = 0;
            try
            {

                string cadena = "select SUM(D.Cantidad)as Dato from (DetOrdenCompra D inner join Articulo A on D.IDArticulo=A.IDArticulo)inner join EncOrdenCompra E on D.IDOrdenCompra = E.IDOrdenCompra where A.Cref like '%" + buscar + "%' and(E.Fecha between '" + fechaInicio + " 00:00:00.1' and '" + fechaFinal + " 23:59:59.999')and E.Status = 'Activo'";
                ClsDatoDecimal Total = new ArticuloContext().Database.SqlQuery<ClsDatoDecimal>(cadena).FirstOrDefault();
                 resultado = Total.Dato;

                if (resultado == null)
                {
                    resultado = 0;
                }

            }catch(Exception err)
            {

            }
            

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        }
}