using Kimyon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using PagedList;

namespace Kimyon.Controllers
{
    public class AdminController : Controller
    {
        eticaretEntities db = new eticaretEntities();
        // GET: Admin
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(tbl_admin avm)
        {
            tbl_admin ad = db.tbl_admin.Where(x => x.admin_username == avm.admin_username && x.admin_password == avm.admin_password).SingleOrDefault();
            if (ad != null)
            {

                Session["admin_id"] = ad.admin_id.ToString();
                return RedirectToAction("Create");

            }
            else
            {
                ViewBag.error = "Geçersiz kullanıcı adı ve şifre";

            }

            return View();
        }

        public ActionResult Create()
        {
            if (Session["admin_id"] == null)
            {
                return RedirectToAction("login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(tbl_category cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Görsel Yüklenemedi...";
            }
            else
            {
                tbl_category cat = new tbl_category();
                cat.category_name = cvm.category_name;
                cat.category_image = path;
                cat.category_status = 1;
                cat.category_fk_admin = Convert.ToInt32(Session["admin_id"].ToString());
                db.tbl_category.Add(cat);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }
            
            return View();
        }//son..................
        public ActionResult Delete(int? id)
        {
            
            tbl_category c = db.tbl_category.Where(x => x.category_id == id).SingleOrDefault();
            db.tbl_category.Remove(c);
            
            db.SaveChanges();

            return RedirectToAction("ViewCategory");
        }
        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.category_status == 1).OrderByDescending(x => x.category_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);

        }

        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Sadece jpg ,jpeg veya png formatları kabul edilebilir....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Lütfen bir görsel seçiniz'); </script>");
                path = "-1";
            }



            return path;
        }


    }
}