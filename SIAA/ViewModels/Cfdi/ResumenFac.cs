using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Cfdi
{
    public class ResumenFac
    {
        public string MonedaOrigen { get; set; }
        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal Total { get; set; }

        public decimal TotalenPesos { get; set; }
    }

    public class ResumenReq
    {
        public string MonedaOrigen { get; set; }
        public decimal Subtotal { get; set; }

        public decimal IVA { get; set; }

        public decimal Total { get; set; }

    }

    public class ResumenSaldoInsoluto
    {
        public string MonedaOrigen { get; set; }
        public decimal SaldoActual { get; set; }

        public decimal TotalenPesos { get; set; }
    }
}