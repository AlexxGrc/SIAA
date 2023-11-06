
using SIAAPI.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SIAA.Controllers
{
    public class MyAccountController : Controller
    {
        public ActionResult Login(string mensaje)
        {
            if (mensaje==null)
            {
                mensaje = "Exito!";
            }
            Login nuevo = new Login();
            ViewBag.Mensaje=mensaje;
            //return RedirectToAction("Index", "Home", new { mensaje = mensaje });
            return View(nuevo);
        }

        [HttpPost]
        [AllowAnonymous]
       // [ValidateAntiForgeryToken]
        public ActionResult Login(Login l, string ReturnUrl = "")
        {
            string pass = MD5(l.Password);

            #region Existing Code
            UserRoleContext bd = new UserRoleContext();
            using (UserContext dc = new UserContext())
            {
                var user = dc.Users.Where(a => a.Username.Equals(l.Username) && a.Password.Equals(pass) && a.Estado!="Inactivo").FirstOrDefault();

               string roles = (from a in bd.Roless
                         join b in bd.UserRoles on a.RoleID equals b.RoleID
                         join c in bd.Users on b.UserID equals c.UserID
                         where c.Username.Equals(l.Username)
                         select a.ROleName).FirstOrDefault();
                

                //var session =(from a in bd.Users where  a.Username.Equals(l.Username) && a.Password.Equals(ses) select a.Session).FirstOrDefault();
                // System.Diagnostics.Debug.WriteLine();
                //System.Diagnostics.Debug.WriteLine(session);
                //System.Web.HttpContext.Current.Session["SessionU"] = session; 
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Username, l.RememberMe);
                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else if (roles=="Bitacora Produccion")
                    {
                        return RedirectToAction("Index", "Bitacora");
                    }
                    else
                    {
                            return RedirectToAction("MyProfile", "Home");
                    }
                }
                else
                {
                    string Mensaje = "Verifica tu datos de identidad";
                    //return View();
                    return RedirectToAction("FalloIdentidad");
                }
            }
            #endregion
            //ViewBag.showSuccessAlert = false;
            //ModelState.Remove("Password");
            //return View();
          
        }

        private string MD5(string password)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(password));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

       

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
             return RedirectToAction("Index","Home");
        }


        public ActionResult FalloIdentidad()
        {
            return View();
        }



    }
}
