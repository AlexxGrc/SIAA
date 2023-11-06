using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Inventarios
{
    [Table("TempInvProm")]
    public class TempInvProm
    {
        [Key]
        public int ID { get; set; }
        public int IDArticulo { get; set; }
        public DateTime FechaUltimaCompra { get; set; }
        public decimal UltimoCosto { get; set; }
        public string UltimaMoneda { get; set; }
        public decimal PromedioenPesos { get; set; }
        public decimal Promedioendls { get; set; }
        public int IDCaracteristica { get; set; }
        public string cref { get; set; }
        public string Descripcion { get; set; }
        public string Presentacion { get; set; }
        public int np { get; set; }
        public decimal existencia { get; set; }
        public string Unidad { get; set; }
        public decimal CostoInvPesos { get; set; }
        public decimal CostoInvDls { get; set; }
        public int IDFamilia { get; set; }
        public int IDAlmacen { get; set; }

        public string Familia { get; set; }

        public string Almacen { get; set; }
        public decimal MinimoVenta { get; set; }
        public decimal MinimoCompra { get; set; }
        public decimal StockMin { get; set; }
        public decimal StockMax { get; set; }

    }
    public class TempInvPromContext : DbContext
    {
        public TempInvPromContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<TempInvProm> TempInvProms { get; set; }
    }



    [Table("Promedio")]
    public class VPedidoAvg
    {
        [Key]
        public int ID { get; set; }
        public int IDArticulo { get; set; }
        public DateTime FechaUltimaCompra { get; set; }
        public decimal UltimoCosto { get; set; }
        public int IDUltimaMoneda { get; set; }
        public decimal PromedioenPesos { get; set; }
        public decimal Promedioendls { get; set; }
    }
    public class VPedidoAvgContext : DbContext
    {
        public VPedidoAvgContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<VPedidoAvg> VPedidoAvgs { get; set; }
    }



    [Table("CostoArticulo")]
    public class CostoArticulo
    {
        [Key]
        public int ID { get; set; }
        public int IDArticulo { get; set; }
        public decimal Costo { get; set; }
        public int IDMoneda { get; set; }
        public string ClaveMoneda { get; set; }
    }
    public class CostoArticuloContext : DbContext
    {
        public CostoArticuloContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<CostoArticulo> CostoArticulos { get; set; }
    }
}