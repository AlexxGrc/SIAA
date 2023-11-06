using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SIAAPI.Models.Comercial;
using SIAAPI.Models.Login;
using SIAAPI.Models.miempresa;
using iTextSharp.text.pdf.parser;
using SIAAPI.Controllers.Cfdi;
using System.IO;
using System.Text;
using SIAAPI.Models;
using System.Globalization;
using OfficeOpenXml;
using PagedList;

namespace SIAAPI.Controllers.MiEmpresa
{
    public class MiEmpresaController : Controller
        {
        public DoctoSGCContext dbs = new DoctoSGCContext();
        public DepartamentosContext dbd = new DepartamentosContext();
        public SGCContext dbsgc = new SGCContext();

        public ActionResult Politicas()
            {
                return View();
            }
        public ActionResult Etica()
            {
                return View();
            }
        public ActionResult Alcance()
            {
                return View();
            }
        public ActionResult Objetivo()
            {
                return View();
            }
        public ActionResult Mision()
            {
                return View();
            }
        public ActionResult Vision()
            {
                return View();
            }
        public ActionResult Valores()
            {
                return View();
            }
        public ActionResult ObjetivoCalidad()
            {
                return View();
            }
        public ActionResult PrincipiosCalidad()
            {
                return View();
            }
        public ActionResult PlanificacionObjetivos()
            {
                return View();
            }
        public ActionResult PlanificacionEstrategica()
            {
                return View();
            }
        public ActionResult PartesInteresadas()
            {
                return View();
            }
        public ActionResult PuestosTrabajo()
            {
                return View();
            }

        public ActionResult Procedimientos(int? page, int? PageSize, string Filtro = "", string viene = "")
        {
            UserContext db = new UserContext();

            string cadenaSQl = string.Empty;

            //DepartamentosContext dbd = new DepartamentosContext();

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<UserRole> userrol = db.Database.SqlQuery<UserRole>("select * from [dbo].[UserRole] where userid='" + usuario + "'").ToList();
            int rol = userrol.Select(s => s.RoleID).FirstOrDefault();
            int iddepto = 0;
            foreach (UserRole user in userrol)
            {
                Departamentos depto = dbd.Database.SqlQuery<Departamentos>("select D.* from departamentos D inner join RolDepto R on D.IDDepartamento = R.IDDepartamento where R.IDRol= " + user.RoleID + "and D.Obsoleto=1").ToList().FirstOrDefault();
                try
                {
                    ViewBag.Departamento = depto.Nombre;
                    iddepto = depto.IDDepartamento;
                }
                catch (Exception err)
                {
                    ViewBag.Departamento = "";
                }

            }

            if (rol == 22)
            {


                if (viene == "Layout")
                {
                    List<SGC> listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES'").ToList();
                    ViewBag.listaind = listaind;
                    if (Filtro != "")
                    {
                        listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listaind = listaind;

                    }
                }
                else if (viene == "Matriz")
                {
                    List<SGC> listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION'").ToList();
                    ViewBag.listamco = listamco;
                    if (Filtro != "")
                    {
                        listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listamco = listamco;
                    }
                }
                else
                {
                    List<SGC> listapro = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'PROCEDIMIENTO'").ToList();
                    ViewBag.listapro = listapro;
                    List<SGC> listafto = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'FORMATO'").ToList();
                    ViewBag.listafto = listafto;
                    List<SGC> listadesP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DESCRIPCION DE PUESTO'").ToList();
                    ViewBag.listadesP = listadesP;
                    List<SGC> listacar = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'CARTA'").ToList();
                    ViewBag.listacar = listacar;
                    ///AGREGAR MATRIZ PROCESOS
                    List<SGC> listaMP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MAPA DE PROCESOS'").ToList();
                    ViewBag.listaMP = listaMP;
                    List<SGC> listadoc = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'DOCUMENTOS'").ToList();
                    ViewBag.listadoc = listadoc;
                    List<SGC> listamri = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE RIESGOS'").ToList();
                    ViewBag.listamri = listamri;
                    List<SGC> listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION'").ToList();
                    ViewBag.listamco = listamco;
                    List<SGC> listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'INDICADORES'").ToList();
                    ViewBag.listaind = listaind;
                    List<SGC> listains = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'INSTRUCCIONES'").ToList();
                    ViewBag.listains = listains;

                    if (Filtro != "")
                    {
                        listapro = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'PROCEDIMIENTO' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listapro = listapro;
                        listafto = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'FORMATO' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listafto = listafto;
                        listadesP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DESCRIPCION DE PUESTO' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listadesP = listadesP;
                        listacar = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'CARTA' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listacar = listacar;
                        listaMP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MAPA DE PROCESOS' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listaMP = listaMP;
                        listadoc = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'DOCUMENTOS' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listadoc = listadoc;
                        listamri = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE RIESGOS' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listamri = listamri;
                        listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listamco = listamco;
                        listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'INDICADORES' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listaind = listaind;
                        listains = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'INSTRUCCIONES' and S.Documento like '%" + Filtro + "%'").ToList();
                        ViewBag.listains = listains;
                    }
                }


            }
            else if (viene == "Layout")
            {



                List<SGC> listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES'").ToList();
                ViewBag.listaind = listaind;
                if (Filtro != "")
                {
                    listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES' and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listaind = listaind;
                }
            }
            else if (viene == "Matriz")
            {



                List<SGC> listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION'").ToList();
                ViewBag.listamco = listamco;
                if (Filtro != "")
                {
                    listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION'  and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listamco = listamco;
                }
            }
            else
            {


                List<SGC> listapro = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'PROCEDIMIENTO'").ToList();
                ViewBag.listapro = listapro;
                List<SGC> listafto = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'FORMATO'").ToList();
                ViewBag.listafto = listafto;
                List<SGC> listadesP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'DESCRIPCION DE PUESTO'").ToList();
                ViewBag.listadesP = listadesP;
                List<SGC> listacar = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'CARTA'").ToList();
                ViewBag.listacar = listacar;
                ///MATRIZ P
                List<SGC> listaMP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where D.Documento = 'MAPA DE PROCESOS'").ToList();
                ViewBag.listaMP = listaMP;
                List<SGC> listadoc = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DOCUMENTOS'").ToList();
                ViewBag.listadoc = listadoc;
                List<SGC> listamri = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'MATRIZ DE RIESGOS'").ToList();
                ViewBag.listamri = listamri;
                List<SGC> listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'MATRIZ DE COMUNICACION'").ToList();
                ViewBag.listamco = listamco;
                List<SGC> listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES'").ToList();
                ViewBag.listaind = listaind;
                List<SGC> listains = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and D.Documento = 'INSTRUCCIONES'").ToList();
                ViewBag.listains = listains;

                if (Filtro != "")
                {
                    listapro = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'PROCEDIMIENTO'   and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listapro = listapro;
                    listafto = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'FORMATO'   and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listafto = listafto;
                    listadesP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DESCRIPCION DE PUESTO'  and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listadesP = listadesP;
                    listacar = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'CARTA'   and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listacar = listacar;
                    //MATRIZ P
                    listaMP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where D.Documento = 'MAPA DE PROCESOS'   and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listaMP = listaMP;
                    listadoc = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DOCUMENTOS'   and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listadoc = listadoc;
                    listamri = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'MATRIZ DE RIESGOS'  and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listamri = listamri;
                    listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'MATRIZ DE COMUNICACION'  and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listamco = listamco;
                    listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES'  and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listaind = listaind;
                    listains = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and D.Documento = 'INSTRUCCIONES'  and S.Documento like '%" + Filtro + "%'").ToList();
                    ViewBag.listains = listains;
                }

            }


            ViewBag.Viene = viene;
            //var lista = from e in dbsgc.SGC
            //            orderby e.Codigo
            //            select e;
            //return View(lista);
            return View();
        }
        //public ActionResult Procedimientos(int? page, int? PageSize, string Filtro = "",string viene ="")
        //{
        //    UserContext db = new UserContext();

        //    string cadenaSQl = string.Empty;

        //    //DepartamentosContext dbd = new DepartamentosContext();

        //    List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
        //    int usuario = userid.Select(s => s.UserID).FirstOrDefault();
        //    List<UserRole> userrol = db.Database.SqlQuery<UserRole>("select * from [dbo].[UserRole] where userid='" + usuario + "'").ToList();
        //    int rol = userrol.Select(s => s.RoleID).FirstOrDefault();
        //    int iddepto = 0;
        //    foreach (UserRole user in userrol)
        //    {
        //        Departamentos depto = dbd.Database.SqlQuery<Departamentos>("select D.* from departamentos D inner join RolDepto R on D.IDDepartamento = R.IDDepartamento where R.IDRol= " + user.RoleID + "and D.Obsoleto=1").ToList().FirstOrDefault();
        //        try
        //        {
        //            ViewBag.Departamento = depto.Nombre;
        //            iddepto = depto.IDDepartamento;
        //        }
        //        catch (Exception err)
        //        {
        //            ViewBag.Departamento = "";
        //        }

        //    }

        //    if (rol == 22)
        //    {


        //        if (viene=="Layout")
        //        {
        //            List<SGC> listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES'").ToList();
        //            ViewBag.listaind = listaind;
        //            if (Filtro!="")
        //            {
        //             listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES' and S.Documento like '%"+Filtro+"%'").ToList();
        //                ViewBag.listaind = listaind;

        //            }
        //        }
        //        else if (viene=="Matriz")
        //        {
        //            List<SGC> listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION'").ToList();
        //            ViewBag.listamco = listamco;
        //            if (Filtro!="")
        //            {
        //                listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listamco = listamco;
        //            }
        //        }
        //        else
        //        {
        //            List<SGC> listapro = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'PROCEDIMIENTO'").ToList();
        //            ViewBag.listapro = listapro;
        //            List<SGC> listafto = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'FORMATO'").ToList();
        //            ViewBag.listafto = listafto;
        //            List<SGC> listadesP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DESCRIPCION DE PUESTO'").ToList();
        //            ViewBag.listadesP = listadesP;
        //            List<SGC> listacar = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'CARTA'").ToList();
        //            ViewBag.listacar = listacar;
        //            ///AGREGAR MATRIZ PROCESOS
        //            ///
        //            List<SGC> listadoc = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'DOCUMENTOS'").ToList();
        //            ViewBag.listadoc = listadoc;
        //            List<SGC> listamri = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE RIESGOS'").ToList();
        //            ViewBag.listamri = listamri;
        //            List<SGC> listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION'").ToList();
        //            ViewBag.listamco = listamco;
        //            List<SGC> listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'INDICADORES'").ToList();
        //            ViewBag.listaind = listaind;
        //            List<SGC> listains = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'INSTRUCCIONES'").ToList();
        //            ViewBag.listains = listains;

        //            if (Filtro!="")
        //            {
        //              listapro = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'PROCEDIMIENTO' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listapro = listapro;
        //              listafto = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'FORMATO' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listafto = listafto;
        //              listadesP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DESCRIPCION DE PUESTO' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listadesP = listadesP;
        //                listacar = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'CARTA' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listacar = listacar;

        //                listadoc = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'DOCUMENTOS' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listadoc = listadoc;
        //                 listamri = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE RIESGOS' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listamri = listamri;
        //                listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listamco = listamco;
        //                listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'INDICADORES' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listaind = listaind;
        //              listains = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'INSTRUCCIONES' and S.Documento like '%" + Filtro + "%'").ToList();
        //                ViewBag.listains = listains;
        //            }
        //        }


        //    }
        //    else if (viene=="Layout")
        //    {



        //        List<SGC> listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES'").ToList();
        //        ViewBag.listaind = listaind;
        //        if (Filtro!="")
        //        {
        //            listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES' and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listaind = listaind;
        //        }
        //    }
        //    else if (viene == "Matriz")
        //    {



        //        List<SGC> listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION'").ToList();
        //        ViewBag.listamco = listamco;
        //        if (Filtro != "")
        //        {
        //            listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where  D.Documento = 'MATRIZ DE COMUNICACION'  and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listamco = listamco;
        //        }
        //    }
        //    else
        //    {


        //        List<SGC> listapro = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'PROCEDIMIENTO'").ToList();
        //        ViewBag.listapro = listapro;                                                                                              
        //        List<SGC> listafto = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'FORMATO'").ToList();
        //        ViewBag.listafto = listafto;
        //        List<SGC> listadesP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'DESCRIPCION DE PUESTO'").ToList();
        //        ViewBag.listadesP = listadesP;
        //        List<SGC> listacar = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'CARTA'").ToList();
        //        ViewBag.listacar = listacar;
        //        ///MATRIZ P
        //        List<SGC> listadoc = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DOCUMENTOS'").ToList();
        //        ViewBag.listadoc = listadoc;                                                                                               
        //        List<SGC> listamri = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'MATRIZ DE RIESGOS'").ToList();
        //        ViewBag.listamri = listamri;                                                                                               
        //        List<SGC> listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'MATRIZ DE COMUNICACION'").ToList();
        //        ViewBag.listamco = listamco;                                                                                               
        //        List<SGC> listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES'").ToList();
        //        ViewBag.listaind = listaind;                                                                                                
        //        List<SGC> listains = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and D.Documento = 'INSTRUCCIONES'").ToList();
        //        ViewBag.listains = listains;

        //        if (Filtro!="")
        //        {
        //          listapro = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'PROCEDIMIENTO'   and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listapro = listapro;
        //           listafto = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'FORMATO'   and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listafto = listafto;
        //           listadesP = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DESCRIPCION DE PUESTO'  and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listadesP = listadesP;
        //            listacar = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'CARTA'   and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listacar = listacar;
        //            //MATRIZ P
        //            listadoc = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where   D.Documento = 'DOCUMENTOS'   and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listadoc = listadoc;
        //           listamri = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'MATRIZ DE RIESGOS'  and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listamri = listamri;
        //       listamco = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'MATRIZ DE COMUNICACION'  and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listamco = listamco;
        //            listaind = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and  D.Documento = 'INDICADORES'  and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listaind = listaind;
        //            listains = dbsgc.Database.SqlQuery<SGC>("select S.* from SGC S inner join DoctoSGC D on S.IDDocto = D.IDDocto where S.IDDepartamento = " + iddepto + " and D.Documento = 'INSTRUCCIONES'  and S.Documento like '%" + Filtro + "%'").ToList();
        //            ViewBag.listains = listains;
        //        }

        //    }


        //    ViewBag.Viene = viene;
        //    //var lista = from e in dbsgc.SGC
        //    //            orderby e.Codigo
        //    //            select e;
        //    //return View(lista);
        //    return View();  
        //}

        //// GET: SGC/Details/5
        //[Authorize(Roles = "Administrador")]

        public ActionResult Details(int id)
        {
            var elemento = dbsgc.SGC.Single(m => m.ID == id);
            if (elemento == null)
            {
                return NotFound();
            }
            return View(elemento);
        }

        private ActionResult NotFound()
        {
            throw new NotImplementedException();
        }

        // POST: SGC/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(int id, SGC collection)
        {
            var elemento = dbsgc.SGC.Single(m => m.ID == id);
            return View(elemento);
        }

        // GET: SGC/Create
        public ActionResult Create( string Mensaje="")
        {
            ViewBag.Mensaje = Mensaje;
            SGC elemento = new SGC();
            ViewBag.IDDepartamento = new SelectList(dbd.Departamentos, "IDDepartamento", "Nombre");
            ViewBag.IDDocto = new SelectList(dbs.DoctoSGC, "IDDocto", "Documento");
            elemento.FechaHora = DateTime.Parse(DateTime.Now.ToString());
            elemento.Documento = "";
            elemento.RutaArchivo = "";
            elemento.Obsoleto = true;
            return View(elemento);
        }

        // POST: SGC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SGC elemento)
        {
            ViewBag.IDDepartamento = new SelectList(dbd.Departamentos, "IDDepartamento", "Nombre");
            ViewBag.IDDocto = new SelectList(dbs.DoctoSGC, "IDDocto", "Documento");
            elemento.FechaHora = DateTime.Parse(DateTime.Now.ToString());
            elemento.Documento = "";
            elemento.RutaArchivo = "";
            elemento.Obsoleto = true;
            try
            {
                dbsgc.SGC.Add(elemento);
                dbsgc.SaveChanges();
                return RedirectToAction("Procedimientos");
            }
            catch(Exception e)
            {
                
               return RedirectToAction("Create", new { Mensaje="Código repetido\n"});
               
            }
        }

        // GET: SGC/Edit/5
        public ActionResult Edit(int id)
        {
            //var elemento = dbsgc.SGC.Single(m => m.ID == id);
            SGC elemento = dbd.Database.SqlQuery<SGC>("Select * from SGC where ID =" + id).ToList().FirstOrDefault();
            ViewBag.IDDepartamento = new SelectList(dbd.Departamentos, "IDDepartamento", "Nombre", elemento.IDDepartamento);
            ViewBag.IDDocto = new SelectList(dbs.DoctoSGC, "IDDocto", "Documento", elemento.IDDocto);
            ViewBag.Ruta = elemento.RutaArchivo;
            ViewBag.Documento = elemento.Documento;
            return View(elemento);
        }

        // POST: SGC/Edit/5
        [HttpPost]
        public ActionResult Edit(SGC elemento)
        {
            //var elemento = dbsgc.SGC.Single(m => m.ID == id);

            elemento.FechaHora = DateTime.Parse(DateTime.Now.ToString());
           
            try {

                string cadena = "update SGC set codigo='" + elemento.Codigo + "', descripcion='" + elemento.Descripcion + "',NoVersion='" + elemento.NoVersion + "', fechahora='" + elemento.FechaHora + "',iddepartamento='" + elemento.IDDepartamento + "', iddocto='" + elemento.IDDocto + "',obsoleto='" + elemento.Obsoleto + "' where id=" + elemento.ID;
                dbd.Database.ExecuteSqlCommand(cadena);
                //if (ModelState.IsValid)
                //{

                //    dbsgc.Entry(elemento).State = System.Data.Entity.EntityState.Modified;
                //    dbsgc.SaveChanges();
                    return RedirectToAction("Procedimientos");
                //}
            }
            catch
            {
                SGC elementos = dbd.Database.SqlQuery<SGC>("Select * from SGC where ID =" + elemento.ID).ToList().FirstOrDefault();

                ViewBag.IDDepartamento = new SelectList(dbd.Departamentos, "IDDepartamento", "Nombre", elemento.IDDepartamento);
                ViewBag.IDDocto = new SelectList(dbs.DoctoSGC, "IDDocto", "Documento", elemento.IDDocto);
                ViewBag.Ruta = elemento.RutaArchivo;
                ViewBag.Documento = elemento.Documento;
                return View(elemento);
            }
            return View(elemento);
        }

        // GET: /Delete/5

        public ActionResult Delete(int id)
        {
            var elemento = dbsgc.SGC.Single(m => m.ID == id);
            if (elemento == null)
            {
                return HttpNotFound();
            }

            return View(elemento);
        }


        // POST: a/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var elemento = dbsgc.SGC.Single(m => m.ID == id);
                dbsgc.SGC.Remove(elemento);
                dbsgc.SaveChanges();
                return RedirectToAction("Procedimientos");

            }
            catch
            {
                return View();
            }
        }


        //Subir Archivo
        public ActionResult SubirArchivo(int id)
        {
            ViewBag.ID = id;
            return View();
        }

        [HttpPost]
        public ActionResult SubirArchivo(HttpPostedFileBase file, int id)
        {
            int idTicket = int.Parse(id.ToString());
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            if (file != null)
            {
                string ruta = Server.MapPath("~/DoctoSGCAdd/");
                ruta += file.FileName;
                string cad = "update SGC set RutaArchivo = '" + ruta + "', Documento = '" + file.FileName + "' where ID =  " + id + "";
                new DoctoSGCAddContext().Database.ExecuteSqlCommand(cad);
                modelo.SubirArchivo(ruta, file);
                ViewBag.Error = modelo.error;
                ViewBag.Correcto = modelo.Confirmacion;
            }
            return RedirectToAction("Procedimientos", new { ID = id });
        }
        public FileResult DescargarPDFIndicador(int id)
        {
            // Obtener contenido del archivo
            DoctoSGCAddContext dbp = new DoctoSGCAddContext();
            IndicadoresSGC elemento = dbp.Database.SqlQuery<IndicadoresSGC>("select*from IndicadoresSGC where IDIndicadoresSGC ="+ id).FirstOrDefault();
            //string ruta = Server.MapPath("~/DoctoSGCAdd/");
            //ruta += elemento.Documento;
            //    string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            //    return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            //string extension = elemento.Documento.Substring(elemento.Documento.Length - 3, 3);
            string ruta = elemento.Ruta;
            string extension;
            string rutacarpeta = Server.MapPath("~/DoctoSGCAdd/" + elemento.Documento);

            extension = System.IO.Path.GetExtension(elemento.Documento);
            extension = extension.ToLower();
            string contentType = "";
            if (extension == ".pdf")
            {
                contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                return new FilePathResult(elemento.Ruta.ToString(), contentType);
            }
            if (extension == ".xml")
            {
                //var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaArchivo.ToString()));
                contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                return File(elemento.Ruta, contentType);


                //return File(rutacarpeta, contentType);
                //return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == ".doc" || extension == ".docx")
            {
                string path = ruta;
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                Response.AddHeader("content-disposition", "attachment; filename=" + elemento.Documento);
                Response.ContentType = "application/msword";//vnd.ms-word.document"; //x-zip-compressed";

                Response.WriteFile(rutacarpeta);
                Response.Flush();
                Response.End();
            }
            if (extension == ".xlsx" || extension == ".xls")
            {

                string direccion = elemento.Ruta;
                System.IO.FileStream fs = null;

                fs = System.IO.File.Open(direccion, System.IO.FileMode.Open);
                byte[] btFile = new byte[fs.Length];
                fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                Response.AddHeader("Content-disposition", "attachment; filename=" + elemento.Documento);
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(btFile);
                Response.Flush();
                Response.End();

            }
            if (extension == ".prn" || extension == ".txt")
            {
                contentType = System.Net.Mime.MediaTypeNames.Text.RichText;
                return File(elemento.Ruta.ToString(), contentType);
            }

            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".jfif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                return new FilePathResult(elemento.Ruta.ToString(), contentType);
            }
            if (extension == ".gif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Gif;
                return new FilePathResult(elemento.Ruta.ToString(), contentType);
            }

            return new FilePathResult(elemento.Ruta.ToString(), contentType);
        }

        public FileResult DescargarPDF(int id)
        {
            // Obtener contenido del archivo
            SGCContext dbp = new SGCContext();
            SGC elemento = dbp.SGC.Find(id);
             //ruta += elemento.Documento;
            //    string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            //    return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            //string extension = elemento.Documento.Substring(elemento.Documento.Length - 3, 3);
            string ruta = elemento.RutaArchivo;
            string rutacarpeta = Server.MapPath("~/DoctoSGCAdd/" + elemento.Documento);

            string extension;
            extension = System.IO.Path.GetExtension(elemento.Documento);
            extension = extension.ToLower();
            string contentType = "";
            if (extension == ".pdf")
            {
                contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == ".xml" )
            {
                //var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaArchivo.ToString()));
                contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                return File(elemento.RutaArchivo, contentType);
               

                //return File(rutacarpeta, contentType);
                //return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if ( extension == ".xlsx" || extension == ".xls")
            {

                string direccion = rutacarpeta;
                System.IO.FileStream fs = null;

                fs = System.IO.File.Open(direccion, System.IO.FileMode.Open);
                byte[] btFile = new byte[fs.Length];
                fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
                fs.Close();

               Response.AddHeader("Content-disposition", "attachment; filename=" + elemento.Documento);
               Response.ContentType = "application/octet-stream";
               Response.BinaryWrite(btFile);
               Response.Flush();
               Response.End();

            }
            if (extension == ".prn" || extension == ".txt")
            {
                contentType = System.Net.Mime.MediaTypeNames.Text.RichText;
                return File(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == ".doc" || extension == ".docx")
            {
                string path = ruta;
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                Response.AddHeader("content-disposition", "attachment; filename=" + elemento.Documento);
                Response.ContentType = "application/msword";//vnd.ms-word.document"; //x-zip-compressed";
             
                Response.WriteFile(rutacarpeta);
                Response.Flush();
                Response.End();
            }

            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".jfif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == ".gif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Gif;
                return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            
            return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
        }
        public FileResult DescargarPDFRevisicion(int id)
        {
            // Obtener contenido del archivo
         
            RevisionD elemento = new RevisionDContext().revisions.Find(id);
            //ruta += elemento.Documento;
            //    string contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
            //    return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            //string extension = elemento.Documento.Substring(elemento.Documento.Length - 3, 3);
            string ruta = elemento.Ruta;
            string rutacarpeta = Server.MapPath("~/DocumentosRevisonDirec/" + elemento.Documento);

            string extension;
            extension = System.IO.Path.GetExtension(elemento.Documento);
            extension = extension.ToLower();
            string contentType = "";
            if (extension == ".pdf")
            {
                contentType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                return new FilePathResult(elemento.Ruta.ToString(), contentType);
            }
            if (extension == ".xml")
            {
                //var stream = new MemoryStream(Encoding.ASCII.GetBytes(elemento.RutaArchivo.ToString()));
                contentType = System.Net.Mime.MediaTypeNames.Text.Xml;
                return File(elemento.Ruta, contentType);


                //return File(rutacarpeta, contentType);
                //return new FilePathResult(elemento.RutaArchivo.ToString(), contentType);
            }
            if (extension == ".doc" || extension == ".docx")
            {
                string path = ruta;
                System.IO.FileInfo file = new System.IO.FileInfo(path);
                Response.AddHeader("content-disposition", "attachment; filename=" + elemento.Documento);
                Response.ContentType = "application/msword";//vnd.ms-word.document"; //x-zip-compressed";

                Response.WriteFile(rutacarpeta);
                Response.Flush();
                Response.End();
            }
            if (extension == ".xlsx" || extension == ".xls")
            {

                string direccion = rutacarpeta;
                System.IO.FileStream fs = null;

                fs = System.IO.File.Open(direccion, System.IO.FileMode.Open);
                byte[] btFile = new byte[fs.Length];
                fs.Read(btFile, 0, Convert.ToInt32(fs.Length));
                fs.Close();

                Response.AddHeader("Content-disposition", "attachment; filename=" + elemento.Documento);
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(btFile);
                Response.Flush();
                Response.End();

            }
            if (extension == ".prn" || extension == ".txt")
            {
                contentType = System.Net.Mime.MediaTypeNames.Text.RichText;
                return File(elemento.Ruta.ToString(), contentType);
            }

            if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".jfif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                return new FilePathResult(elemento.Ruta.ToString(), contentType);
            }
            if (extension == ".gif")
            {
                contentType = System.Net.Mime.MediaTypeNames.Image.Gif;
                return new FilePathResult(elemento.Ruta.ToString(), contentType);
            }

            return new FilePathResult(elemento.Ruta.ToString(), contentType);
        }

        public ActionResult CrearIndicadores(int? IDDepartamento)
        {
            c_MesesContext mes = new c_MesesContext();
            IndicadoresSGC indicador = new IndicadoresSGC();
            ViewBag.IDDepartamento = new SelectList(dbd.Departamentos, "IDDepartamento", "Nombre", IDDepartamento);
            ViewBag.IDMes = new SelectList(mes.c_Meses, "IDMes", "Mes", DateTime.Now.Month);
           indicador.Ano= DateTime.Now.Year;
            indicador.Documento = "";

            return View(indicador);
        }

        // POST: SGC/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearIndicadores( HttpPostedFileBase file, IndicadoresSGC indicador,FormCollection coleccion )
        {
            ViewBag.IDDepartamento = new SelectList(dbd.Departamentos, "IDDepartamento", "Nombre");
            ViewBag.IDDocto = new SelectList(dbs.DoctoSGC, "IDDocto", "Documento");
            

            string NombredeArchivo = "";
            c_Meses meses = new c_MesesContext().Database.SqlQuery<c_Meses>("select*from c_Meses where idmes= "+ indicador.IDMes).ToList().FirstOrDefault();
            string nombredecarpeta = meses.Mes;/*obtenerNombreMesNumero(indicador.IDMes);*/
            string doc = coleccion.Get("file");
            string CarpetaAño = indicador.Ano.ToString();

            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Indicadores")))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Indicadores"));
            }
            string ruta = "";
            SubirArchivosModelo modelo = new SubirArchivosModelo();

           
                if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Indicadores")))
                {
                    Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Indicadores/" + nombredecarpeta));
                }
                else
                {
                    if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Indicadores/" + nombredecarpeta)))
                    {
                        Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Indicadores/" + nombredecarpeta));
                    }
                }
            if (!Directory.Exists(System.Web.HttpContext.Current.Server.MapPath("~/Indicadores/" + nombredecarpeta + "/"+ CarpetaAño)))
            {
                Directory.CreateDirectory(System.Web.HttpContext.Current.Server.MapPath("~/Indicadores/" + nombredecarpeta + "/" + CarpetaAño));
            }

        
            NombredeArchivo = file.FileName;

            ruta = Server.MapPath("~/Indicadores/");
            ruta +=  nombredecarpeta + "/"+CarpetaAño+"/"+ NombredeArchivo;

            indicador.Fecha = DateTime.Now.ToString();
            indicador.Documento = "";
            indicador.Documento = file.FileName;
            indicador.Ruta = ruta;
            
            modelo.SubirArchivo(ruta, file);
           

            try
            {
                new DoctoSGCAddContext().indicadoressgc.Add(indicador);
              
                string insert = "insert into indicadoressgc (IDDepartamento,IDMes,Ruta,Documento,Fecha,Ano) values ('" + indicador.IDDepartamento + "','" + indicador.IDMes + "','" + indicador.Ruta.Replace("/","\\") + "','" + indicador.Documento + "','" + indicador.Fecha + "','"+indicador.Ano+"')";
                new DoctoSGCAddContext().Database.ExecuteSqlCommand(insert);
                //new DoctoSGCAddContext().SaveChanges();
                return RedirectToAction("Indicadores");
            }
            catch (Exception e)
            {

                return RedirectToAction("CrearIndicadores");

            }
        }

        private string obtenerNombreMesNumero(int numeroMes)
        {
            try
            {
                DateTimeFormatInfo formatoFecha = CultureInfo.CurrentCulture.DateTimeFormat;
                string nombreMes = formatoFecha.GetMonthName(numeroMes);
                return nombreMes;
            }
            catch
            {
                return "Desconocido";
            }
        }
         public ActionResult Indicadores()
        {
            UserContext db = new UserContext();

            string cadenaSQl = string.Empty;

            //DepartamentosContext dbd = new DepartamentosContext();

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<UserRole> userrol = db.Database.SqlQuery<UserRole>("select * from [dbo].[UserRole] where userid='" + usuario + "'").ToList();
            int rol = userrol.Select(s => s.RoleID).FirstOrDefault();

            Departamentos depto = dbd.Database.SqlQuery<Departamentos>("select D.* from departamentos D inner join RolDepto R on D.IDDepartamento = R.IDDepartamento where R.IDRol= " + rol + "and D.Obsoleto=1").ToList().FirstOrDefault();
            ViewBag.Departamento = depto.Nombre;
            ViewBag.IDDepartamento = depto.IDDepartamento;
            int iddepto = depto.IDDepartamento;
          

            return View();
        }
        public ActionResult RevisionD(string sortOrder, string currentFilter, string searchString, int? page, int? PageSize, string viene="", string Trimestres="")
        {

            ViewBag.Viene = Trimestres ;

            UserContext db = new UserContext();

            var TrimestresL = new List<SelectListItem>();
            TrimestresL.Add(new SelectListItem { Text = "---Seleccionar Trimestre---", Value = "N/A" , Selected= true});
            TrimestresL.Add(new SelectListItem { Text = "Primer Trimestre", Value = "PrimerTrimestre" });
            TrimestresL.Add(new SelectListItem { Text = "Segunto Trimestre", Value = "SegundoTrimestre" });
            TrimestresL.Add(new SelectListItem { Text = "Tercer Trimestre", Value = "TercerTrimestre" });
            TrimestresL.Add(new SelectListItem { Text = "Cuarto Trimestre", Value = "CuartoTrimestre" });

            ViewBag.Trimestres = new SelectList(TrimestresL, "Value", "Text");

            string cadenaSQl = string.Empty;

            //DepartamentosContext dbd = new DepartamentosContext();

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<UserRole> userrol = db.Database.SqlQuery<UserRole>("select * from [dbo].[UserRole] where userid='" + usuario + "'").ToList();
            int rol = userrol.Select(s => s.RoleID).FirstOrDefault();

            Departamentos depto = dbd.Database.SqlQuery<Departamentos>("select D.* from departamentos D inner join RolDepto R on D.IDDepartamento = R.IDDepartamento where R.IDRol= " + rol + "and D.Obsoleto=1").ToList().FirstOrDefault();
            ViewBag.Departamento = depto.Nombre;
            ViewBag.IDDepartamento = depto.IDDepartamento;

            List<RevisionD> revi = db.Database.SqlQuery<RevisionD>("select * from [dbo].[RevisionD] where cuatrimestre='"+Trimestres+"'").ToList();



            //var elementos = db.Database.SqlQuery<VInventarioAlmacen>(CadenaSql).ToList();
            int count = revi.Count();
            //int count = db.InventarioAlmacenes.OrderByDescending(e => e.IDInventarioAlmacen).Count(); // Total number of elements

            // Populate DropDownList
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "10", Value = "10", Selected = true },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" },
                new SelectListItem { Text = "100", Value = "100" },
                new SelectListItem { Text = "Todo", Value = count.ToString() }
             };

            int pageNumber = (page ?? 1);
            int pageSize = (PageSize ?? 10);
            ViewBag.psize = pageSize;
        


            return View(revi.ToPagedList(pageNumber, pageSize));
        }
            public ActionResult SubirArchivoRevision(string viene="")
        {
            ViewBag.viene = viene;
            return View();
        }

        [HttpPost]
        public ActionResult SubirArchivoRevision(HttpPostedFileBase file, string viene="")
        {
            List<User> userid = new ArticuloContext().Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();

            SubirArchivosModelo modelo = new SubirArchivosModelo();
            if (file != null)
            {
                string ruta = Server.MapPath("~/DocumentosRevisonDirec/"+viene+ "/");
                ruta += file.FileName;
                string cad = "insert into RevisionD (Fecha,Documento,Ruta,Usuario,Cuatrimestre) values (SYSDATETIME(),'"+ file.FileName + "','"+ruta+"','"+ usuario+"','"+viene+"')";
                new DoctoSGCAddContext().Database.ExecuteSqlCommand(cad);
                modelo.SubirArchivo(ruta, file);
                ViewBag.Error = modelo.error;
                ViewBag.Correcto = modelo.Confirmacion;
            }
            return RedirectToAction("RevisionD", new { Trimestres=viene});
        }

        public ActionResult IndicadoresDoc(string viene)
        {
            UserContext db = new UserContext();

            string cadenaSQl = string.Empty;

            //DepartamentosContext dbd = new DepartamentosContext();

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<UserRole> userrol = db.Database.SqlQuery<UserRole>("select * from [dbo].[UserRole] where userid='" + usuario + "'").ToList();
            int rol = userrol.Select(s => s.RoleID).FirstOrDefault();
            //if (usuario== 1625 || usuario==23)
            //{
            //    Departamentos depto = dbd.Database.SqlQuery<Departamentos>("select D.* from departamentos D inner join RolDepto R on D.IDDepartamento = R.IDDepartamento where R.IDRol= " + rol + "and D.Obsoleto=1").ToList().FirstOrDefault();
            //    ViewBag.Departamento = depto.Nombre;
            //    int iddepto = depto.IDDepartamento;
            //    c_Meses meses = new c_MesesContext().Database.SqlQuery<c_Meses>("select*from c_Meses where mes= '" + viene + "'").ToList().FirstOrDefault();

            //    int numeromes = meses.IDMes;/*DateTime.ParseExact(viene, "MMMM", CultureInfo.CurrentCulture).Month;*/
            //    ViewBag.IDMes = numeromes;
            //    List<ListAno> indicador = db.Database.SqlQuery<ListAno>("select distinct Ano, Ano from [dbo].[IndicadoresSGC]").ToList();

            //    ViewBag.IndicadorMes = indicador;
            //}
            //else
            //{
                Departamentos depto = dbd.Database.SqlQuery<Departamentos>("select D.* from departamentos D inner join RolDepto R on D.IDDepartamento = R.IDDepartamento where R.IDRol= " + rol + "and D.Obsoleto=1").ToList().FirstOrDefault();
                ViewBag.Departamento = depto.Nombre;
                int iddepto = depto.IDDepartamento;
                c_Meses meses = new c_MesesContext().Database.SqlQuery<c_Meses>("select*from c_Meses where mes= '" + viene + "'").ToList().FirstOrDefault();

                int numeromes = meses.IDMes;/*DateTime.ParseExact(viene, "MMMM", CultureInfo.CurrentCulture).Month;*/
                ViewBag.IDMes = numeromes;
            //List<ListAno> indicador = db.Database.SqlQuery<ListAno>("select distinct Ano, Ano from [dbo].[IndicadoresSGC] where iddepartamento=" + iddepto).ToList();
            List<ListAno> indicador = db.Database.SqlQuery<ListAno>("select distinct Ano, Ano from [dbo].[IndicadoresSGC]").ToList();
            ViewBag.IndicadorMes = indicador;
            //}
            

            return View();
        }
        public ActionResult IndicadoresDocAn(string an, int IDMes, string Proceso)
        {
            UserContext db = new UserContext();

            string cadenaSQl = string.Empty;
            List<SelectListItem> Departamento;
            string cadena1c = "select*from departamentos";

            Departamento = new ClientesContext().Database.SqlQuery<Departamentos>(cadena1c).ToList().OrderBy(s => s.Nombre).Select(c => new SelectListItem
            {
                Value = c.IDDepartamento.ToString(),
                Text = c.Nombre
            }).ToList();

            SelectListItem sinclien = new SelectListItem();
            sinclien.Text = "---- Selecciona un departamento ----";
            sinclien.Value = "0";
            sinclien.Selected = true;
            Departamento.Add(sinclien);

            ViewBag.Proceso = Departamento;
            ViewBag.an = an;
            ViewBag.idmes = IDMes;
            int IDDepartamento = 0;
            try
            {
                IDDepartamento= int.Parse(Proceso);
            }
            catch (Exception err)
            {

            }
            //DepartamentosContext dbd = new DepartamentosContext();

            List<User> userid = db.Database.SqlQuery<User>("select * from [dbo].[User] where Username='" + User.Identity.Name + "'").ToList();
            int usuario = userid.Select(s => s.UserID).FirstOrDefault();
            List<UserRole> userrol = db.Database.SqlQuery<UserRole>("select * from [dbo].[UserRole] where userid='" + usuario + "'").ToList();
            int rol = userrol.Select(s => s.RoleID).FirstOrDefault();

            Departamentos depto = dbd.Database.SqlQuery<Departamentos>("select D.* from departamentos D inner join RolDepto R on D.IDDepartamento = R.IDDepartamento where R.IDRol= " + rol + "and D.Obsoleto=1").ToList().FirstOrDefault();
            ViewBag.Departamento = depto.Nombre;
            int iddepto = depto.IDDepartamento;
            ViewBag.Usuario = usuario;
            int numeromes = IDMes/*DateTime.ParseExact(an, "MMMM", CultureInfo.CurrentCulture).Month*/;
            //if (usuario == 1625 || usuario == 23)
            //{
                List<IndicadoresSGC> indicador = db.Database.SqlQuery<IndicadoresSGC>("select * from [dbo].[IndicadoresSGC] where ano=" + an + " and idmes='" + numeromes + "' and iddepartamento="+ IDDepartamento).ToList();

                ViewBag.IndicadorMes = indicador;
            //}
            //else
            //{
            //    List<IndicadoresSGC> indicador = db.Database.SqlQuery<IndicadoresSGC>("select * from [dbo].[IndicadoresSGC] where ano=" + an + " and idmes='" + numeromes + "' and iddepartamento=" + iddepto).ToList();

            //    ViewBag.IndicadorMes = indicador;
            //}


            return View();
        }
        public ActionResult EliminarArchivo(int id)
        {
            SGCContext db = new SGCContext();
            string cad = "update SGC set RutaArchivo = NULL, Documento = NULL where ID = " + id + "";
            new DoctoSGCAddContext().Database.ExecuteSqlCommand(cad);

            return RedirectToAction("Procedimientos");
        }
        public ActionResult EliminarArchivoRevision(int id)
        {
            SGCContext db = new SGCContext();

            try
            {
                RevisionD revisionD = new RevisionDContext().revisions.Find(id);
                System.IO.File.Delete(revisionD.Ruta);
            }
            catch (Exception ERR)
            {

            }
            string cad = "delete from  RevisionD where idrevision= " + id + "";
            new DoctoSGCAddContext().Database.ExecuteSqlCommand(cad);
         
            return RedirectToAction("RevisionD");
        }
        public ActionResult EliminarArchivoIndicador(int id)
        {
            SubirArchivosModelo modelo = new SubirArchivosModelo();
            IndicadoresSGC docAn = new DoctoSGCAddContext().Database.SqlQuery<IndicadoresSGC>("select*from IndicadoresSGC where IDIndicadoresSGC=" + id).FirstOrDefault();

            string ruta = "@"+docAn.Ruta; 
            try
            {
                modelo.EliminarArchivoIndicadorCarpeta(ruta);
            }
            catch (Exception err)
            {

            }
                SGCContext db = new SGCContext();
            string cad = "delete from IndicadoresSGC where IDIndicadoresSGC = " + id + "";
            new DoctoSGCAddContext().Database.ExecuteSqlCommand(cad);

            return RedirectToAction("Indicadores");
        }
    }
}

public class ListAno
{
    public int Ano { get; set; }
}