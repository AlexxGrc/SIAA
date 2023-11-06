using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Articulo
{
    public class LPresentacionSchema
    {
        public List<LPresentacion> lPrecentacion = new List<LPresentacion>();
        public string Schema { get; set; }
    }
    public class LPresentacion
    {
        [Key]
        public int ID { get; set; }
        public int IDPresentacion { get; set; }
        public int Cotizacion { get; set; }
        public int version { get; set; }
        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }
        public int Articulo_IDArticulo { get; set; }
        public bool obsoleto { get; set; }

        public int IDCotizacion { get; set; }
        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }


    }
}