using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.ViewModels.Cfdi
{
    public class VPagoFacMesS_NC_A
    {
        [Key]
        public int ID { get; set; }

        public System.DateTime fechapago { get; set; }
        public decimal importepagado { get; set; }
        public string Monedapago{ get; set; }

        public string serie { get; set; }

        public int numero { get; set; }
        public System.DateTime fechaFactura{ get; set; }

        public string nombre_cliente { get; set; }

        public decimal subtotal { get; set; }

        public decimal iva { get; set; }

        public decimal total { get; set; }
        public string moneda { get; set; }
        public decimal Importesaldoinsoluto { get; set; }
        public string Vendedor { get; set; }
    }
    public class VPagoFacMesS_NC_AContext : DbContext
    {
        public VPagoFacMesS_NC_AContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPagoFacMesS_NC_A> VPagoFacMesS_NC_As { get; set; }
    }

    public class PeriodoFechasPagoF
    {

        [DisplayFormat(ApplyFormatInEditMode = true,
                   DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime fechaInicial { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
                   DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime fechaFinal { get; set; }

        //public int Anticipo { get; set; }
        //public int NotaCredito { get; set; }
    }
    //public class PeriodoFechasPagoFContext : DbContext
    //{
    //    public PeriodoFechasPagoFContext() : base("name=DefaultConnection")
    //    {

    //    }
    //    public DbSet<PeriodoFechasPagoF> PeriodoFechasPagoF { get; set; }
    //}

}