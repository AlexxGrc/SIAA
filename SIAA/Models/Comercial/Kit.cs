using SIAAPI.Models.Administracion;
using SIAAPI.Models.Calidad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Comercial
{
        [Table("Kit")]
        public class Kit
        {
            [Key]
            public int IDKit{ get; set; }
        public int IDArticulo { get; set; }
        public int IDCaracteristica { get; set; }
        public virtual Articulo Articulo { get; set; }
        public int IDArticuloComp { get; set; }
        public virtual Articulo ArticuloComp { get; set; }
        //public int IDCaracteristica { get; set; }
        //public virtual Caracteristica Caracteristica { get; set; }


        public decimal Cantidad { get; set; }

        public string Clave { get; set; }

        public bool porcantidad { get; set; }
        public bool porporcentaje { get; set; }
        public int IDAlmacen { get; set; }
        public decimal Precio { get; set; }


    }

    public class KitContext : DbContext
        {
            public KitContext() : base("name=DefaultConnection")
            {

            }
            public DbSet<Kit> Kits { get; set; }

        }
    [Table("Articulo")]
    public class Articulokit
    {
        [Key]
        public int IDArticulo { get; set; }
        public string Cref { get; set; }
        public string Descripcion { get; set; }
        public int IDFamilia { get; set; }
        public virtual Familia Familia { get; set; }
        public int IDTipoArticulo { get; set; }
        public bool Preciounico { get; set; }
        public int IDMoneda { get; set; }
        public virtual c_Moneda c_Moneda { get; set; }
        public bool CtrlStock { get; set; }
        public bool ManejoCar { get; set; }
        public int IDClaveUnidad { get; set; }
        public virtual c_ClaveUnidad c_ClaveUnidad { get; set; }
        public bool bCodigodebarra { get; set; }
        public string Codigodebarras { get; set; }
        public string Obscalidad { get; set; }
        public bool ExistenDev { get; set; }
        public int IDAQL { get; set; }
        public virtual AQLCalidad AQLCalidad { get; set; }
        public int IDInspeccion { get; set; }
        public virtual Inspeccion Inspeccion { get; set; }
        public int IDMuestreo { get; set; }
        public virtual Muestreo Muestreo { get; set; }
        public bool esKit { get; set; }
        public bool GeneraOrden { get; set; }
        public bool obsoleto { get; set; }
        public string nameFoto { get; set; }
        public decimal MinimoVenta { get; set; }
        public decimal MinimoCompra { get; set; }
  
    }
    public class ArticulokitContext : DbContext
    {
        public ArticulokitContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer<ArticulokitContext>(null);
        }
        public DbSet<Articulo> Articulo { get; set; }
        public DbSet<Composicion> Composicion { get; set; }

        public DbSet<Caracteristica> Caracteristica { get; set; }
        public DbSet<MatrizPrecio> MatrizPrecio { get; set; }
        public DbSet<ArtProveedor> ArtProveedor { get; set; }
        
        public DbSet<Muestreo> Muestreo { get; set; }

        public DbSet<Inspeccion> Inspeccion { get; set; }
        public DbSet<TipoArticulo> TipoArticulo { get; set; }

        public DbSet<AQLCalidad> AQLCalidad { get; set; }

        public DbSet<SIAAPI.Models.Administracion.c_ClaveUnidad> c_ClaveUnidad { get; set; }

        public DbSet<SIAAPI.Models.Administracion.c_Moneda> c_Moneda { get; set; }

        public DbSet<SIAAPI.Models.Comercial.Familia> Familias { get; set; }
    
        //public System.Data.Entity.DbSet<SIAAPI.Models.Produccion.ActividadBitacora> ActividadBitacoras { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Produccion.Proceso> Procesoes { get; set; }

        public System.Data.Entity.DbSet<SIAAPI.Models.Produccion.Trabajador> Trabajadors { get; set; }
    }

    [Table("Kit")]
    public class VKit
    {
        [Key]
        public int IDKit { get; set; }
        public int IDArticulo { get; set; }
        public string Articulo{ get; set; }
        public string Descripcion { get; set; }
        public decimal Cantidad { get; set; }

        public string Clave { get; set; }

        public bool porcantidad { get; set; }
        public bool porporcentaje { get; set; }
        public string Imagen { get; set;}

    }


}