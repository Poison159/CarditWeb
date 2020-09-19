using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CardItWeb.Models;
using CardItWebApp.Database;
using CardItWebApp.Models;

namespace CardItWebApp.Controllers.MVC
{
    public class MerchantsController : Controller
    {
        private CardItDbContext db = new CardItDbContext();

        // GET: Merchants
        public ActionResult Index()
        {
            return View(db.Merchants.ToList());
        }

        // GET: Merchants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return HttpNotFound();
            }
            return View(merchant);
        }

        // GET: Merchants/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Merchants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Website,ImagePath")] Merchant merchant)
        {
            if (ModelState.IsValid)
            {
                string targetPath = Server.MapPath("~");
                var path = targetPath + @"Content\imgs\" + merchant.Name.Replace(" ", String.Empty) + ".png";
                try{
                    Helper.downloadImage(path, merchant.ImagePath);
                }
                catch (Exception){
                    return View(merchant);
                }
                merchant.ImagePath = "https://carditweb.conveyor.cloud/Content/imgs/" + merchant.Name + ".png";
                db.Merchants.Add(merchant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(merchant);
        }

        // GET: Merchants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return HttpNotFound();
            }
            return View(merchant);
        }

        // POST: Merchants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Website,ImagePath")] Merchant merchant)
        {
            if (ModelState.IsValid)
            {
                string targetPath = Server.MapPath("~");
                var path = targetPath + @"Content\imgs\" + merchant.Name.Replace(" ", String.Empty) + ".png";
                try
                {
                    Helper.downloadImage(path, merchant.ImagePath);
                }
                catch (Exception)
                {
                    return View(merchant);
                }
                merchant.ImagePath = "https://carditweb.conveyor.cloud/Content/imgs/" + merchant.Name.Replace(" ",String.Empty) + ".png";
                db.Entry(merchant).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(merchant);
        }

        // GET: Merchants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return HttpNotFound();
            }
            return View(merchant);
        }

        // POST: Merchants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Merchant merchant = db.Merchants.Find(id);
            db.Merchants.Remove(merchant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
