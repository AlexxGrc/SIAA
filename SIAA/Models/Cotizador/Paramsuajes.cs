using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Data.Entity;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SIAAPI.Models.Cotizador
{
    [Table("paramsuajes")]
    public class Paramsuajes
    {
        [Key]
            public int IDParamsuajes { get; set; }
            public decimal costodiente7            {get;set;}
            public decimal costopulgada            {get;set;}
            public decimal PRECIODOLAR             {get;set;}
            public int MTSROLLO                     {get;set;}
            public decimal FactorG                  {get;set;}
            public decimal fletesua                {get;set;}
            public decimal costodiente10           {get;set;}
            public decimal factorn                 {get;set;}
            public decimal factorc                 {get;set;}
            public decimal Rangog1                 {get;set;}
            public decimal Rangog2                 {get;set;}
            public decimal Rangog3                 {get;set;}
            public decimal Rangog4                 {get;set;}
            public decimal Rangog5                 {get;set;}
            public decimal Rangog6                 {get;set;}
            public decimal Diente7Flex             {get;set;}
            public decimal Diente10Flex            {get;set;}
            public decimal PulgadaFlex             {get;set;}
            public decimal Cireldigital            {get;set;}
            public decimal Costohoraanalogo        {get;set;}
            public decimal Costohoradigital        {get;set;}
            public decimal fleteflexible           {get;set;}
            public decimal CostoDiente7Rod         {get;set;}
            public decimal CostoDiente10Rod        {get;set;}
            public decimal Costoextra7Rod          {get;set;}
            public decimal Costoextra10Rod         {get;set;}
            public decimal FormingPlate            {get;set;}
            public decimal RangoT1                 {get;set;}
            public decimal RangoT2                 {get;set;}
            public decimal RangoT3                 {get;set;}
            public decimal RangoT4                 {get;set;}
            public decimal RangoT5                 {get;set;}
            public decimal RangoT6                 {get;set;}
            public decimal NylonHS                 {get;set;}
            public decimal CosCirelMin              { get; set; }
    }

    public class ParamsuajesContext : DbContext
    {
        public ParamsuajesContext() : base("name=DefaultConnection")
        {

        }
        public DbSet<Paramsuajes> Paramsuajes { get; set; }
    }

}