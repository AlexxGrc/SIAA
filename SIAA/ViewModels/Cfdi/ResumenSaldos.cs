﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Cfdi
{
    public class ResumenSaldos
    {
       
        public string Moneda { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal ImportePagado { get; set; }
        public decimal ImporteSaldoInsoluto { get; set; }
        
    }
}