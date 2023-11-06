using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SIAAPI.Controllers.Cfdi
{
    public class SubirArchivosModelo
    {
        public string Confirmacion { get; set; }
        public Exception error { get; set; }
        public void SubirArchivo(string ruta, HttpPostedFile file)
        {
            try
            {
                file.SaveAs(ruta);
                this.Confirmacion = "Archivo Guardado";
            }
            catch(Exception ex)
            {
                this.error = ex;
            }
        }

        internal void SubirArchivo(string ruta, HttpPostedFileBase file)
        {
            try
            {
                file.SaveAs(ruta);
                this.Confirmacion = "Archivo Guardado";
            }
            catch (Exception ex)
            {
                this.error = ex;
            }
        }
        internal void EliminarArchivoIndicadorCarpeta(string ruta)
        {
            try
            {
                File.Delete(ruta);
                this.Confirmacion = "Archivo Guardado";
            }
            catch (Exception ex)
            {
                this.error = ex;
            }
        }
    }
} 