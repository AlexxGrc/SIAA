using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SIAAPI.Models.CartaPorte
{
	[Table("c_DerechosDePaso")]
	public class c_DerechosDePaso
    {
		[Key]
		public int IDDerecho { get; set; }
		[DisplayName("Clave")]
		public string ClavePaso { get; set; }
		[DisplayName("Derecho de paso")]
		public string DerechoPaso { get; set; }
		[DisplayName("Entre")]
		public string Entre { get; set; }
		[DisplayName("Hasta")]
		public string Hasta { get; set; }
		[DisplayName("OtorgaR")]
		public string OtorgaRecibe { get; set; }
		[DisplayName("Cuestionario")]
		public string Concesionario { get; set; }
		[DisplayName("Fecha de Inicio")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime FIvigencia { get; set; }
		[DisplayName("Fecha de Terminación")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		public DateTime? FFvigencia { get; set; }
	}
	public class c_DerechosDePasoContext : DbContext
	{
		public c_DerechosDePasoContext() : base("name=DefaultConnection")
		{

		}
		public DbSet<c_DerechosDePaso> derecho{ get; set; }
	}
}