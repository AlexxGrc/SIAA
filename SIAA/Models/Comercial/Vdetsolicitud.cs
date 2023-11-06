using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
    public class Vdetsolicitud
    {
        //Primary Key 
        [Key]
        public int IDDetSolicitud { get; set; }
        [DisplayName("ID Articulo")]
        public int IDArticulo { get; set; }
        [DisplayName("Código de Articulo")]
        public string Cref { get; set; }
        [DisplayName("Descripción")]
        public string descripcion { get; set; }
        [DisplayName("Presentación")]
        public string Presentacion { get; set; }
        [DisplayName("ID Caracteristica")]
        public int Caracteristica_ID { get; set; }

        [DisplayName("ID Almacen")]
        public int IDAlmacen { get; set; }

        [DisplayName("Almacen")]
        public string Almacen { get; set; }

        [DisplayName("Documento")]
        public string Documento { get; set; }
        [DisplayName("Número")]
        public int Numero { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Existencia")]
        public decimal Existencia { get; set; }
        [DisplayName("Por Llegar")]
        public decimal PorLlegar { get; set; }
        [DisplayName("Apartado")]
        public decimal Apartado { get; set; }
        [DisplayName("Disponibilidad")]
        public decimal Disponibilidad{ get; set; }
    }

        public class DetSolicitud
    {
            //Primary Key 
            [Key]
        public int IDDetSolicitud { get; set; }

        public int IDArticulo { get; set; }
        public int Caracteristica_ID { get; set; }
        public int IDAlmacen { get; set; }
        public string Documento { get; set; }
        public int Numero { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Costo { get; set; }
        public decimal CantidadPedida { get; set; }
        public decimal Descuento { get; set; }
        public decimal Importe { get; set; }
        public bool IVA { get; set; }
        public decimal ImporteIva { get; set; }
        public decimal ImporteTotal { get; set; }
        public string Nota { get; set; }
        public bool Requerido { get; set; }
        public decimal Suministro { get; set; }
        public string Status { get; set; }
        public string DocumentoR { get; set; }
        public int NumeroDR { get; set; }
        public string Presentacion { get; set; }
        public string jsonPresentacion { get; set; }

    }
}