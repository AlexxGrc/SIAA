

using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SIAAPI.Controllers
{
    public class IndexController : Controller
    {

        public ActionResult Politicas( string mensaje)
        {
            return View("Politicas", "Miempresa");
        }

    }
}
