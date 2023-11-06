using SIAAPI.Models;
using SIAAPI.Models.Administracion;
using SIAAPI.Models.Cfdi;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Comisiones
{
    public class VPagoComi
    {

        [Key]
        public int ID { get; set; }

        [DisplayName("Fecha de pago")]
        public DateTime fechapago { get; set; }

        [DisplayName("Importe pagado")]
        public decimal importepagado { get; set; }

        [DisplayName("Moneda de pago")]
        public string Monedapago { get; set; }

        [DisplayName("Serie")]
        public string serie { get; set; }

        [DisplayName("Numero de factura")]
        public int numero { get; set; }

        [DisplayName("Fecha factura")]
        public DateTime fecha { get; set; }

        [DisplayName("Cliente")]
        public int IDCliente { get; set; }
        public virtual ICollection<Clientes> Clientes { get; set; }

        [DisplayName("Subtotal")]
        public decimal subtotal { get; set; }

        [DisplayName("Moneda subtotal")]
        public int IDMoneda { get; set; }
        public virtual ICollection<c_Moneda> c_Moneda { get; set; }

        [DisplayName("Vendedor")]
        public int IDVendedor { get; set; }
        public virtual ICollection<Vendedor> Vendedor { get; set; }

        //---------------Periodo
        [DisplayName("Mes")]
        public int IDMes { get; set; }
        public virtual ICollection<c_Meses> c_Meses { get; set; }
        [DisplayName("Año")]
        public int Ano { get; set; }

    }

    public class VArticuloDet
    {
        public string SerieDigital { get; set; }
        public string NumeroDigital { get; set; }
        public string Cref { get; set; }
        public string Articulo { get; set; }
        public string Presentacion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CostoArticulo { get; set; }
        public string ClaveMoneda { get; set; }
        public decimal PrecioCliente { get; set; }

    }
}