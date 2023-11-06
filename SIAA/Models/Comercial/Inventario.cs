using SIAAPI.Models.Administracion;
using SIAAPI.Models.Comercial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Inventarios
{
    [Table("Inventario")]
    public class Inventario
    {
        [Key]
        public int IDInventario { get; set; }

        [DisplayName("Almacen")]
        [Required]
        public int IDAlmacen { get; set; }
        public virtual Almacen Almacen { get; set; }
        [DisplayName("Artículo")]
        [Required]
        public int IDArticulo { get; set; }
        public virtual Articulo Articulo { get; set; }
        [DisplayName("Presentacion")]
        public int IDCaracteristica { get; set; }
        public virtual Caracteristica Caracteristicas { get; set; }
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("ClaveUnidad")]
        public int IDClaveUnidad { get; set; }
        public virtual c_ClaveUnidad c_ClaveUnidades { get; set; }


    }
    public class InventarioContext : DbContext
    {
        public InventarioContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<InventarioContext>(null);
        }
        public DbSet<Inventario> Inventarios { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Comercial.Almacen> Almacens { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Comercial.Articulo> Articuloes { get; set; }
        public DbSet<TipoArticulo> TipoArticulo { get; set; }
        public DbSet<Caracteristica> Caracteristicas { get; set; }
        public DbSet<c_ClaveUnidad> c_ClaveUnidades { get; set; }
    }

    [Table("VInventarioInicial")]
    public class VInventarioInicial
    {
        [Key]
        public int IDInventario { get; set; }

        [DisplayName("IDAlmacen")]
        public int IDAlmacen { get; set; }
        [DisplayName("Almacen")]
        public string Almacen { get; set; }
        [DisplayName("IDArtículo")]
        public int IDArticulo { get; set; }
        [DisplayName("Artículo")]
        public string Producto { get; set; }
        [DisplayName("IDCaracterística")]
        public int IDCaracteristica { get; set; }
        [DisplayName("Característica")]
        public string Presentacion { get; set; }
        [DisplayName("Version")]
        public int Version { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        [DisplayName("Cantidad")]
        public decimal Cantidad { get; set; }
        [DisplayName("Clave Unidad")]
        public int IDClaveUnidad { get; set; }
        [DisplayName("Unidad")]
        public string Unidad { get; set; }
    }

    public class VInventarioInicialContext : DbContext
    {
        public VInventarioInicialContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VInventarioInicialContext>(null);
        }
        public DbSet<VInventarioInicial> VInvIni { get; set; }
    }

    [Table("VArtCaracteristica")]
    public class VArtCaracteristica
    {
        [Key]
        public int ID { get; set; }

        [DisplayName("IDArticulo")]
        public int IDArticulo { get; set; }
        [DisplayName("IDCaracteristica")]
        public int IDCaracteristica { get; set; }
        [DisplayName("Cref")]
        public string Cref { get; set; }
        [DisplayName("Descripcion")]
        public string Descripcion { get; set; }
        [DisplayName("Presentacion")]
        public string Presentacion { get; set; }
        [DisplayName("IDClaveUnidad")]
        public int IDClaveUnidad { get; set; }
        [DisplayName("Unidad")]
        public string Unidad { get; set; }
		[DisplayName("no Presentacion")]
        public int IDPresentacion { get; set; }
        public decimal MinimoVenta { get; set; }
        public decimal MinimoCompra { get; set; }
        [DisplayName("Stock Minimo")]
        public decimal StockMin { get; set; }
        [DisplayName("Stock Maximo")]
        public decimal StockMax { get; set; }
    }
    public class VArtCaracteristicaContext : DbContext
    {
        public VArtCaracteristicaContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<VArtCaracteristicaContext>(null);
        }
        public DbSet<VArtCaracteristica> VArtC { get; set; }
    }



}