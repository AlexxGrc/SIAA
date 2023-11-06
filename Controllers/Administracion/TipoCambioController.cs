using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using SIAAPI.Models.Administracion;
using System;
using System.Runtime.Serialization;
using System.Data.SqlClient;

using System.IO;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using SIAAPI.Reportes;


namespace SIAAPI.Controllers.Administracion
{
    [Authorize(Roles = "Administrador, Gerencia,Sistemas, GerenteVentas,Facturacion, Compras,")]
    public class TipoCambioController : Controller
    {
        private TipoCambioContext db = new TipoCambioContext();
        private c_MonedaContext db1 = new c_MonedaContext();
        private MonedaRepository re = new MonedaRepository();

        // GET: TipoCambio/Create
      
        public ActionResult Create()
        {
            var monedas = new MonedaRepository().GetMoneda();
            ViewBag.Moneda = monedas;
            ViewBag.Mensaje = "";
            return View();
        }

        // POST: TipoCambio/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTipoCambio,FechaTC,IDMonedaOrigen,IDMonedaDestino,TC")] TipoCambio tipoCambio)
        {
           
                try
                {
                    db.Database.ExecuteSqlCommand("insert into TipoCambio (FechaTC,IDMonedaOrigen,IDMonedaDestino,TC) VALUES ('" + tipoCambio.FechaTC.Value.Year + "-" + tipoCambio.FechaTC.Value.Month + "-" + tipoCambio.FechaTC.Value.Day + "','" + tipoCambio.IDMonedaOrigen + "','" + tipoCambio.IDMonedaDestino + "'," + tipoCambio.TC + ")");

                    db.Database.ExecuteSqlCommand("insert into TipoCambio (FechaTC,IDMonedaDestino,IDMonedaOrigen,TC) VALUES ('" + tipoCambio.FechaTC.Value.Year + "-" + tipoCambio.FechaTC.Value.Month + "-" + tipoCambio.FechaTC.Value.Day + "','" + tipoCambio.IDMonedaOrigen + "','" + tipoCambio.IDMonedaDestino + "', (1/" + tipoCambio.TC + "))");
                //db.SaveChanges();

                return RedirectToAction("Index", "VObtenerMoneda");
                }
                catch (SqlException err)
                {
                    var monedas = new MonedaRepository().GetMoneda();
                    ViewBag.Mensaje = err.Message;
                    ViewBag.Moneda = monedas;
                    return View();
                }
                catch (Exception err)
                {
                    var monedas = new MonedaRepository().GetMoneda();
                    ViewBag.Mensaje = err.Message;
                    ViewBag.Moneda = monedas;
                    return View();
                }
         

            return View(tipoCambio);
        }
       

    }
}

