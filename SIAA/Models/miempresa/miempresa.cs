using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.miempresa
{
    public class miempresa
    {

    }

    [Table("Departamentos")]
    public class Departamentos
    {
        [Key]
        public int IDDepartamento { get; set; }
        [DisplayName("Cave del Departamento")]
        public string ClaveDepartamento { get; set; }
        [DisplayName("Departamento")]
        public string Nombre { get; set; }
        [DisplayName("Activo")]
        public bool Obsoleto { get; set; }
        public string Responsable { get; set; }
        public int Usuario { get; set; }
    }
    public class DepartamentosContext : DbContext
    {
        public DepartamentosContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<Departamentos> Departamentos { get; set; }
    }

    [Table("RevisionD")]
    public class RevisionD
    {
        [Key]
        public int IDRevision { get; set; }
       
        public DateTime Fecha { get; set; }
       
        public string Documento { get; set; }
       
        public string Ruta { get; set; }
       
        public int Usuario { get; set; }
    }
    public class RevisionDContext : DbContext
    {
        public RevisionDContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<RevisionD> revisions { get; set; }
    }

    [Table("RolDepto")]
    public class RolDepto
    {
        [Key]
        public int ID { get; set; }
        [DisplayName("Departamento")]
        public int IDDepartamento { get; set; }
        [DisplayName("Rol")]
        public int IDRol { get; set; }
    }
    public class RolDeptoContext : DbContext
    {
        public RolDeptoContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<RolDepto> RolDepto { get; set; }
    }


    [Table("VRolDepto")]
    public class VRolDepto
    {
        [Key]
        public int ID { get; set; }
        public string Departamento { get; set; }
        public string Rol { get; set; }
    }
    public class VRolDeptoContext : DbContext
    {
        public VRolDeptoContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<VRolDepto> VRolDepto { get; set; }
    }

    [Table("DoctoSGC")]
    public class DoctoSGC
    {
        [Key]
        
        public int IDDocto { get; set; }
        [DisplayName("Clave")]
        public string Clave { get; set; }
        [DisplayName("Documento")]
        public string Documento{ get; set; }
    }
    public class DoctoSGCContext : DbContext
    {
        public DoctoSGCContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<DoctoSGC> DoctoSGC { get; set; }
    }

    [Table("SGC")]
    public class SGC
    {
        [Key]
        public int ID { get; set; }
        [DisplayName("Código")]
        public string Codigo { get; set; }
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
        [DisplayFormat(DataFormatString = "{0:1}")]
        [DisplayName("No. de Versión")]
        public double NoVersion { get; set; }
        public string RutaArchivo { get; set; }
        public string Documento { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true,
        DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        [DisplayName("Fecha de generación")]
        public DateTime FechaHora { get; set; }
        [DisplayName("Departamento")]
        public int IDDepartamento { get; set; }
        public virtual Departamentos Departamentos { get; set; }
        [DisplayName("Documento")]
        public int IDDocto { get; set; }
        public virtual DoctoSGC DoctoSGC { get; set; }
        [DisplayName("Activo")]
        public bool Obsoleto { get; set; }
    }
    public class SGCContext : DbContext
    {
        public SGCContext() : base("name=DefaultConnection")
        {
        }
        public DbSet<SGC> SGC { get; set; }
        public System.Data.Entity.DbSet<Departamentos> Departamentos { get; set; }
        public System.Data.Entity.DbSet<DoctoSGC> DoctoSGC { get; set; }
    }

    //[Table("DoctoSGCAdd")]

    public class DoctoSGCAdd
    {
        [Key]
        public int ID { get; set; }
        [DisplayName("Ruta Archivo")]
        public string RutaArchivo { get; set; }
        [DisplayName("Nombre Archivo")]
        public string Documento { get; set; }
    }
    public class DoctoSGCAddContext : DbContext
    {
        public DoctoSGCAddContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<DoctoSGCAdd> DoctoSGCAdds { get; set; }
        public DbSet<SGC> SGCs { get; set; }
        public DbSet<IndicadoresSGC> indicadoressgc { get; set; }
    }

    public class IndicadoresSGC
    {
        [Key]
        public int IDIndicadoresSGC { get; set; }
        [DisplayName("Departamento")]
        public int IDDepartamento { get; set; }
        [DisplayName("Mes")]
        public int IDMes { get; set; }
        [DisplayName("Año")]
        public int Ano { get; set; }
        public string Ruta { get; set; }
        [DisplayName("Archivo")]
        public string Documento { get; set; }
        [DisplayName("Fecha")]
        public string Fecha { get; set; }
    }
  

}