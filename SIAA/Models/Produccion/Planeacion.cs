using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.Produccion
{
    public class Planeacion
    {
    }

    public class modelAddEditPlaneacion
    {
        //public Articulo Articulo { get; set; }
        public List<ModelosDeProduccion> Modelos { get; set; }
        public List<VistaModeloProceso> VistaModeloProceso { get; set; }

     
    }

    public class PlenacionContext : DbContext
    {
        public PlenacionContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<ModelosDeProduccion> ModelosDeProduccions { get; set; }

        public DbSet<VistaModeloProceso> VistaModeloProcesos { get; set; }
    }
}