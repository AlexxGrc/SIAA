using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Cfdi
{
    public class FactVen
    {
        public int ID { get; set; }
        public string Serie { get; set; }
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public int IDCliente { get; set; }
        public string Nombre { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public decimal TC { get; set; }
        public int IDMoneda { get; set; }
        public string MonedaFactura { get; set; }
        public int IDVendedor { get; set; }
        public string Vendedor { get; set; }
    }
}