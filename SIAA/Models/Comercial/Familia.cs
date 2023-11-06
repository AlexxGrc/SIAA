namespace SIAAPI.Models.Comercial
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    [Table("Familia")]
    public partial class Familia
    {
        [Key]
        public int IDFamilia { get; set; }
        public IEnumerable<SelectListItem> familias { get; set; }

        [Display(Name = "Código de la Familia")]
        [Required(ErrorMessage = "Un codigo de Familia")]
        [StringLength(10)]
        public string CCodFam { get; set; }
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "Escribe una descripción")]
        [StringLength(100)]
        public string Descripcion { get; set; }

        [Display(Name = "Catálogo del SAT")]
        [Required(ErrorMessage = "Elije un producto del SAT")]
        public int IDProdServ { get; set; }
        [Display(Name = "Factor Mínimo de Ganancia")]
        public decimal FactorMinimoGanancia { get; set; }
        [DisplayName("Tipo de Articulo")]
        public int IDTipoArticulo { get; set; }
        public virtual TipoArticulo TipoArticulo { get; set; }
        [Display(Name = "Catálogo de la STCC")]
        [Required(ErrorMessage = "Elije un producto de la STCC")]
        public int IDClaveSTCC { get; set; }

    }

    public class FamiliaContext : DbContext
    {
        public FamiliaContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Familia> Familias { get; set; }
        public DbSet<TipoArticulo> TipoArticulo { get; set; }
    }


    [Table("NivelesGanancia")]
    public class NivelesGanancia
    {
        [Key]
        public int IDNivel { get; set; }
        [DisplayName("Familia")]
        public int IDFamilia { get; set; }
        public virtual Familia Familia { get; set; }
        [DisplayName("Porcentaje")]
        public decimal Porcentaje { get; set; }
        [DisplayName("Nivel")]
        public int Nivel { get; set; }
        public decimal RangInf { get; set; }
        public decimal RangSup { get; set; }
    }
    [Table("RangoPlaneacionArticulo")]
    public class RangoPlaneacionArticuloN
    {
        [Key]

        public int Nivel { get; set; }
        public decimal RangoInf { get; set; }

        public decimal RangoSup { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Costo { get; set; }

        public decimal Precio { get; set; }

        //public int IDMoneda { get; set; }

        //public decimal TC { get; set; }

    }
    public class NivelesGananciaContext : DbContext
    {
        public NivelesGananciaContext() : base("name=DefaultConnection")
        {

        }
        public virtual DbSet<NivelesGanancia> NivelesGanancias { get; set; }

    }
    public class FamiliaRepository
    {
        public IEnumerable<SelectListItem> GetFamiliasSinSuajes()
        {
            using (var context = new FamiliaContext())
            {
                string cadenasql = " select * from Familia where idfamilia<>11 and idfamilia<>69 and idfamilia<>71 and idfamilia<>72 and " +
                    "idfamilia<>75 and idfamilia<>76 and idfamilia<>77 and idfamilia<>80 and idfamilia<>81 and idfamilia<>91 and idfamilia<>93 and idfamilia<>70 and idfamilia<>95";


                List<SelectListItem> lista = context.Database.SqlQuery<Familia>(cadenasql).ToList()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDFamilia.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Familia de productos ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        public IEnumerable<SelectListItem> GetFamilias()
        {
            using (var context = new FamiliaContext())
            {
                List<SelectListItem> lista = context.Familias.AsNoTracking()
                    .OrderBy(n => n.Descripcion)
                        .Select(n =>
                        new SelectListItem
                        {
                            Value = n.IDFamilia.ToString(),
                            Text = n.Descripcion
                        }).ToList();
                var countrytip = new SelectListItem()
                {
                    Value = null,
                    Text = "--- Selecciona una Familia de productos ---"
                };
                lista.Insert(0, countrytip);
                return new SelectList(lista, "Value", "Text");
            }
        }

        //public string getAtributosJsonSchema(int _idFamila)
        //{
        //    AtributodeFamiliaContext db = new AtributodeFamiliaContext();
        //    var lista = from e in db.AtributodeFamilias.AsNoTracking()
        //                where e.IDFamilia == _idFamila
        //                orderby e.IDAtributo
        //                select e;


        //    string cadenajson = "{ \"title\": Caracteristicas del producto ,\n \"type\": \"object\", \n";

        //    int contadorRequeridos = 0;
        //    string cadenaRequeridos = "";


        //    foreach (var elemento in lista)
        //    {
        //        if (contadorRequeridos == 0)
        //        {
        //            cadenaRequeridos += "\n\"required\": [";
        //        }

        //        if (elemento.Requerido)
        //        {
        //            if (contadorRequeridos > 0)
        //            {
        //                cadenaRequeridos += ",\n";
        //            }
        //            cadenaRequeridos += "\"" + elemento.Descripcion + "\"";
        //            contadorRequeridos++;
        //        }
        //    }

        //    if (contadorRequeridos>0)
        //    {
        //        cadenaRequeridos += "\n],";
        //        cadenajson += cadenaRequeridos;
        //    }
        //        int contador = 0;

        //    cadenajson += " \"properties\": { ";

        //    foreach (var elemento in lista)
        //    {
        //        if (contador>0)
        //        {
        //            cadenajson += ",";
        //        }

        //        string tipop = "string";
        //        switch(elemento.Tipo.ToString())
        //        {
        //            case "CADENA":
        //                { tipop = "string"; break; }
        //            case "NUMERO":
        //                { tipop = "number"; break; }
        //            case "LISTA":
        //                { tipop = "array"; break; } ;
        //            case "RADIO LISTA":
        //                { tipop = "array"; break; };

        //        }

        //        cadenajson += "\"" + elemento.Descripcion + "\":{\n";
        //        cadenajson += ", \"Type\": \"" + tipop +"\",\n";
        //        cadenajson += " \"Title\": \"" + elemento.Titulo + "\"\n";

        //        if (elemento.Valores ==null)
        //        {
        //            cadenajson += "}";
        //        }
        //        else
        //        {
        //            if ((elemento.Valores.Trim()) == "")
        //            {
        //                cadenajson += "}";
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    string[] valores = elemento.Valores.Split(',');
        //                    if (valores.Length > 0)
        //                    {
        //                        cadenajson += ",\n\"items\": {\n\"type\": \"string\",\n\"enum\": [\n";

        //                        for (int i = 0; i < valores.Length; i++)
        //                        {
        //                            if (i > 0)
        //                            { cadenajson += ","; }
        //                            cadenajson += "\"" + valores[i] + "\"";
        //                        }
        //                        cadenajson += "]\n}\n\"uniqueItems\": true";

        //                    }
        //                }
        //                catch
        //                {
        //                    cadenajson += "}\n";
        //                }
        //            }
        //        }
        //        contador++;
        //    }
        //    cadenajson += "}";

        //    return cadenajson;
        //}

        //public string getAtributosJsonSchemaFJ(int _idFamila)
        //{
        //    string schemaJsonDes = "";
        //    AtributodeFamiliaContext db = new AtributodeFamiliaContext();
        //    var lista = from e in db.AtributodeFamilias.AsNoTracking()
        //                where e.IDFamilia == _idFamila
        //                orderby e.IDAtributo
        //                select e;

        //    parametrizacionSchemaFJ att = new parametrizacionSchemaFJ();


        //    foreach (var elemento in lista)
        //    {
        //        schemaJsonDes = schemaJsonDes + elemento.Descripcion + ": {";

        //        if (String.IsNullOrEmpty(elemento.Valores))
        //        {
        //            schemaJsonDes = schemaJsonDes + "type:\"string\", ";
        //            schemaJsonDes = schemaJsonDes + "minLength:" + elemento.LongitudMin + ", ";
        //            schemaJsonDes = schemaJsonDes + "maxLength:" + elemento.LongitudMax + ", ";
        //            schemaJsonDes = schemaJsonDes + "title:\"" + elemento.Titulo + "\", ";
        //            schemaJsonDes = schemaJsonDes + "required:" + elemento.Requerido.ToString().ToLower() + ", ";
        //            schemaJsonDes = schemaJsonDes + "description:\"" + elemento.Descripcion + "\"";
        //        }
        //        else
        //        {
        //            schemaJsonDes = schemaJsonDes + "type:\"string\", ";
        //            schemaJsonDes = schemaJsonDes + "title:\"" + elemento.Titulo + "\", ";

        //            string listValue = "enum: [";
        //            String[] substrings = elemento.Valores.Split(',');
        //            foreach (string dato in substrings)
        //            {
        //                listValue = listValue + "'" + dato + "', ";
        //            }
        //            listValue = listValue.Substring(0, listValue.Length - 2);
        //            listValue = listValue + "]";

        //            schemaJsonDes = schemaJsonDes + listValue;
        //        }

        //        schemaJsonDes = schemaJsonDes + "}, ";
        //    }

        //    schemaJsonDes = schemaJsonDes.Substring(0, schemaJsonDes.Length - 2);
        //    schemaJsonDes = " { " + schemaJsonDes + " } ";

        //    JObject json = JObject.Parse(schemaJsonDes);

        //    return json.ToString();
        //}

        //public class parametrizacionSchemaFJ
        //{
        //    public string type { get; set; }
        //    public int minLength { get; set; }
        //    public int maxLength { get; set; }
        //    public string title { get; set; }
        //    public bool required { get; set; }
        //    public string description { get; set; }
        //    public string _enum { get; set; }
        //}

        ////public List<string> getAtributosJsonSchemaFJ(int _idFamila)
        ////{
        ////    AtributodeFamiliaContext db = new AtributodeFamiliaContext();
        ////    var lista = from e in db.AtributodeFamilias.AsNoTracking()
        ////                where e.IDFamilia == _idFamila
        ////                orderby e.IDAtributo
        ////                select e;

        ////    List<string> Esquema = new List<string>();



        ////    foreach (var elemento in lista)
        ////    {
        ////        string cadenajson = "";
        ////        cadenajson += " " + elemento.Descripcion+ ": \n{";

        ////        string tipop = "string";
        ////        switch (elemento.Tipo.ToString())
        ////        {
        ////            case "CADENA":
        ////                { tipop = "string";
        ////                    cadenajson += " type: \"" + tipop + "\",\n";
        ////                    cadenajson += " minLength:" + elemento.LongitudMin + ",\n";
        ////                    cadenajson += " maxLength:" + elemento.LongitudMax + ",\n";
        ////                    cadenajson += " title: \"" + elemento.Titulo + "\",\n";
        ////                    if( elemento.Requerido)
        ////                    {
        ////                        cadenajson += " Required:true,\n";
        ////                    }
        ////                    else
        ////                    {
        ////                        cadenajson += " Required:false,\n";

        ////                    }
        ////                    cadenajson += " description: \"" + elemento.Ayuda + "\"}";



        ////                    break; }
        ////            case "NUMERO":
        ////                { tipop = "number";

        ////                    cadenajson += " type: \"" + tipop + "\",\n";


        ////                    cadenajson += " title: \"" + elemento.Titulo + "\",\n";
        ////                    if (elemento.Requerido)
        ////                    {
        ////                        cadenajson += " Required:true}\n";
        ////                    }
        ////                    else
        ////                    {
        ////                        cadenajson += " Required:false}\n";

        ////                    }


        ////                    break; }
        ////            case "LISTA":
        ////                {
        ////                    tipop = "string";
        ////                    cadenajson += " type: \"" + tipop + "\",\n";
        ////                    cadenajson += " title: \"" + elemento.Titulo + "\",\n";


        ////                    if (elemento.Valores == null)
        ////                    {
        ////                        cadenajson += "";
        ////                    }
        ////                    else
        ////                    {
        ////                        if ((elemento.Valores.Trim()) == "")
        ////                        {
        ////                            cadenajson += "";
        ////                        }
        ////                        else
        ////                        {
        ////                            try
        ////                            {
        ////                                string[] valores = elemento.Valores.Split(',');
        ////                                if (valores.Length > 0)
        ////                                {
        ////                                    cadenajson += "enum: [";

        ////                                    for (int i = 0; i < valores.Length; i++)
        ////                                    {
        ////                                        if (i > 0)
        ////                                        { cadenajson += ","; }
        ////                                        cadenajson += "'" + valores[i] + "'";
        ////                                    }
        ////                                    cadenajson += "]}";

        ////                                }
        ////                            }
        ////                            catch
        ////                            {
        ////                                cadenajson += "";
        ////                            }
        ////                        }
        ////                    }



        ////                    break; };
        ////                //case "RADIO LISTA":
        ////                //    { tipop = "array"; break; };

        ////        }


        ////        Esquema.Add(cadenajson);



        ////    }


        ////    return Esquema;
        ////}

        //public string getAtributosUIJson(int _idFamila)
        //{
        //    AtributodeFamiliaContext db = new AtributodeFamiliaContext();
        //    var lista = from e in db.AtributodeFamilias.AsNoTracking()
        //                where e.IDFamilia == _idFamila
        //                orderby e.IDAtributo
        //                select e;


        //    string cadenajson = "{";

        //    int contador = 0;

        //    foreach (var elemento in lista)
        //    {
        //        if (contador > 0)
        //        {
        //            cadenajson += ",";
        //        }

        //        cadenajson += "\"" + elemento.Descripcion + "\": {\n";

        //        string tipop = "string";
        //        switch (elemento.Tipo.ToString())
        //        {
        //            case "CADENA":
        //                { tipop = "\"ui: emptyValue\": \"\"\n"; break; }
        //            case "NUMERO":
        //                { tipop = "\"ui: widget\": \"updown\"\n"; break; }
        //            case "LISTA":
        //                { tipop = "\"ui: widget\": \"select\"\n"; break; };
        //            case "RADIO LISTA":
        //                { tipop = "\"ui:widget\": \"radio\",\n  \"ui: options\": {\"inline\": false }\n "; break; };

        //        }
        //        cadenajson += tipop;
        //        cadenajson += "\"ui: title\": \"" + elemento.Titulo + "\":}\n";


        //        contador++;
        //    }

        //    cadenajson += "}";

        //    return cadenajson;
        //}

    }
}
