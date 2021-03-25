using Kimyon.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kimyon.Controllers
{
    public class UserController : Controller
    {

        eticaretEntities db = new eticaretEntities();
        // GET: User
        public ActionResult Index(int ?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.category_status == 1).OrderByDescending(x => x.category_id).ToList();
            IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(tbl_user uvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Görsel Yüklenemedi....";
            }
            else
            {
                tbl_user u = new tbl_user();
                u.u_name = uvm.u_name;
                u.u_email = uvm.u_email;
                u.u_password = uvm.u_password;
                u.u_image = path;
                u.u_contact = uvm.u_contact;
                db.tbl_user.Add(u);
                db.SaveChanges();
                return RedirectToAction("Login");
            
            
            }


                return View();
        }



        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(tbl_user avm)
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();
            if (ad != null)
            {

                Session["userid"] = ad.userid.ToString();
                return RedirectToAction("index");

            }
            else
            {
                ViewBag.error = "Geçersiz kullanıcı adı ve şifre";

            }

            return View();
        }


        public ActionResult CreateAd()
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "category_id", "category_name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateAd(tbl_product pvm, HttpPostedFileBase imgfile)
        {
            List<tbl_category> li = db.tbl_category.ToList();
            ViewBag.categorylist = new SelectList(li, "category_id", "category_name");


            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Görsel Yüklenemedi....";
            }
            else
            {
                tbl_product p = new tbl_product();
                p.product_name = pvm.product_name;
                p.product_price = pvm.product_price;
                p.product_image = path;
                p.product_fk_category = pvm.product_fk_category;
                p.product_description = pvm.product_description;
                p.product_fk_user = Convert.ToInt32(Session["userid"].ToString());
                db.tbl_product.Add(p);
                db.SaveChanges();
                Response.Redirect("index");


            }
                return View();
        }

        public ActionResult Ads(int ?id, int?page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.product_fk_category == id).OrderByDescending(x => x.product_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);
            
        }

        [HttpPost]
        public ActionResult Ads(int? id, int? page, string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.product_name.Contains(search)).OrderByDescending(x => x.product_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);

            //search butonu için

            return View(stu);

        }


        public ActionResult Adtocart(int? Id)
        {

            tbl_product p = db.tbl_product.Where(x => x.product_id == Id).SingleOrDefault();
            return View(p);
        }

        List<Cart> li = new List<Cart>();
        [HttpPost]
        public ActionResult Adtocart(tbl_product pi, string qty, int Id)
        {
            tbl_product p = db.tbl_product.Where(x => x.product_id == Id).SingleOrDefault();

            Cart c = new Cart();
            c.product_id = p.product_id;
            c.price = (float)p.product_price;
            c.qty = Convert.ToInt32(qty);
            c.bill = c.price * c.qty;
            c.product_name = p.product_name;
            if (TempData["Cart"] == null)
            {
                li.Add(c);
                TempData["Cart"] = li;

            }
            else
            {
                List<Cart> li2 = TempData["Cart"] as List<Cart>;
                li2.Add(c);
                TempData["Cart"] = li2;
            }

            TempData.Keep();




            return RedirectToAction("Index");
        }

        public ActionResult remove(int? id)
        {
            List<Cart> li2 = TempData["Cart"] as List<Cart>;
            Cart c = li2.Where(x => x.product_id == id).SingleOrDefault();
            li2.Remove(c);
            float h = 0;
            foreach (var item in li2)
            {
                h += item.bill;
            }
            TempData["total"] = h;
            return RedirectToAction("checkout");
        }
        public ActionResult checkout()
        {
            List<Cart> li = TempData["Cart"] as List<Cart>;
            TempData.Keep();


            return View();
        }
        
        [HttpPost]
        public ActionResult checkout(tbl_order O)
        {
            List<Cart> li = TempData["Cart"] as List<Cart>;

            /*tbl_invoice iv = new tbl_invoice();
            iv.in_fk_user = Convert.ToInt32(Session["userid"].ToString());
            iv.in_date = System.DateTime.Now;
            db.tbl_invoice.Add(iv);
            db.SaveChanges();*/

           /* foreach (var item in li)
            {
                tbl_order od = new tbl_order();
                od.o_fk_pro = item.product_id;
                od.o_fk_invoice = iv.in_id;
                od.o_date = System.DateTime.Now;
                od.o_qty = item.qty;
                od.o_unitprice = (int)item.price;
                od.o_bill = item.bill;
                db.tbl_order.Add(od);
                db.SaveChanges();
            }*/

            TempData.Remove("total");
            TempData.Remove("Cart");

            TempData["msg"] = "İşlem Gerçekleştirilmiştir";
            TempData.Keep();
            return RedirectToAction("Index");
        }


        public ActionResult ViewAd(int? id)
        {
            Adviewmodel ad = new Adviewmodel();
            tbl_product p = db.tbl_product.Where(x => x.product_id == id).SingleOrDefault();
            ad.product_id = p.product_id;
            ad.product_name = p.product_name;
            ad.product_image = p.product_image;
            ad.product_price = p.product_price;
            tbl_category cat = db.tbl_category.Where(x => x.category_id == p.product_fk_category).SingleOrDefault();
            ad.category_name = cat.category_name;
            tbl_user u = db.tbl_user.Where(x => x.userid == p.product_fk_user).SingleOrDefault();
            ad.u_name = u.u_name;
            ad.u_image = u.u_image;
            ad.u_contact = u.u_contact;
            ad.product_fk_user = u.userid;


            return View(ad);


        }

        public ActionResult Signout()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index");

        }

        public ActionResult DeleteAd(int? id)
        {
            tbl_product p = db.tbl_product.Where(x => x.product_id == id).SingleOrDefault();
            db.tbl_product.Remove(p);
            db.SaveChanges();

            return RedirectToAction("Index");
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
                Response.Write("<script>alert('Lütfen görsel seçiniz'); </script>");
                path = "-1";
            }



            return path;
        }







    }
}