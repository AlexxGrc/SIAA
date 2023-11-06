using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SIAAPI.Models.Produccion
{
    [Table("Proceso")]
    public class Proceso
    {
        [Key]
        public int IDProceso { get; set; }
        public IEnumerable<SelectListItem> procesos { get; set; }

        [Required(ErrorMessage = "El nombre del proceso es obligatorio y acepta como máximo 150 caracteres")]
        [DisplayName("Nombre del proceso")]
        [StringLength(150)]
        public string NombreProceso { get; set; }


        [Required(ErrorMessage = "Elija una opción que determine, sí el proceso usa o no máquinas")]
        [DisplayName("Usa máquina")]
        public bool UsaMaquina { get; set; }

        [Required(ErrorMessage = "Elija una opción que determine, sí el proceso usa o no herramientas")]
        [DisplayName("Usa herramienta")]
        public bool UsaHerramientas { get; set; }

        [Required(ErrorMessage = "Elija una opción que determine, sí el proceso usa o no Insumos")]
        [DisplayName("Usa insumos")]
        public bool UsaInsumos { get; set; }

        [Required(ErrorMessage = "Elija una opción que determine, sí el proceso usa o no documentos")]
        [DisplayName("Usa Documentos")]
        public bool UsaDocumentos { get; set; }

        [Required(ErrorMessage = "Elija una opción que determine, sí el proceso usa o no mano de obra")]
        [DisplayName("Usa mano de obra")]
        public bool UsaManoDeObra { get; set; }

        [Required(ErrorMessage = "Elija Elija una opción que determine si el proceso usa o no bitácora de trabajo")]
        [DisplayName("Usa bitacora de trabajo")]
        public bool UsaBitacoraDeTrabajo { get; set; }

        [Required(ErrorMessage = "Elija una opción que determine, sí el proceso va ligado a una orden de compra")]
        [DisplayName("Liga orden de compra")]
        public bool LigarOrdenDeCompra { get; set; }

        [Required(ErrorMessage = "Elija una opción que indique, sí el proceso solicita un lote")]
        [DisplayName("Solicita lote")]
        public bool SolicitaLote { get; set; }

        [Required(ErrorMessage = "Elija el proceso antecedente")]
        [DisplayName("Proceso antecedente")]
        public int ProcesoAntecedente { get; set; }

        [DisplayName("Proceso padre")]
        public int ProcesoPadre { get; set; }
    }
    public class ProcesoContext : DbContext
    {
        public ProcesoContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Proceso> Procesos { get; set; }
    }
    
    public class ProcesoRepository
    {
        private SelectListItem NombreProcesotip;

        public IEnumerable<SelectListItem> GetNombreProceso()
        {
            using (var context = new ProcesoContext())
            {
                List<SelectListItem> lista = context.Procesos.AsNoTracking()
                    .OrderBy(n => n.NombreProceso)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDProceso.ToString(),
                            Text = n.NombreProceso
                        }).ToList();
                var NombreProcesotip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona un nombre de proceso ---"
                };
                lista.Insert(0, NombreProcesotip);
                return new SelectList(lista, "Value", "Text");
            }
        }
    }
}

