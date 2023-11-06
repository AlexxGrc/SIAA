using SIAAPI.ViewModels.Articulo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SIAAPI.Controllers.Administracion
{
    public class FilePDFController : Controller
    {
        //public FilePDF F = new FilePDF(); 
        // GET: PDF
        public FileResult DescargarPDFH(string ruta, string name)
        {
            // Obtener contenido del archivo   
            //    string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            //    return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            ruta +=name;
            string rutaF = Server.MapPath(ruta);
            string extension = name.Substring(name.Length - 3, 3);
            extension = extension.ToLower();
            string contentType = "";
            if (extension == "pdf")
            {
                contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                return new FilePathResult(rutaF.ToString(), contentType);
            }
            if (extension == "xml")
            {
                var stream = new MemoryStream(Encoding.ASCII.GetBytes(ruta.ToString()));
                return File(stream, "text/plain", name);
            }
            if (extension == "prn" || extension == "txt")
            {
                contentType = System.Net.Mime.MediaTypeNames.Text.RichText;
                return File(rutaF.ToString(), contentType);
            }

            if (extension == "jpg" || extension == "jpeg")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                return new FilePathResult(rutaF.ToString(), contentType);
            }
            if (extension == "gif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Gif;
                return new FilePathResult(rutaF.ToString(), contentType);
            }
            return new FilePathResult(rutaF.ToString(), contentType);
        }


    }
}