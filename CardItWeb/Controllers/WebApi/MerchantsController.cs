using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using CardItWebApp.Database;
using CardItWebApp.Models;

namespace CardItWebApp.Controllers.WebApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MerchantsController : ApiController
    {
        private CardItDbContext db = new CardItDbContext();

        // GET: api/Merchants
        public IQueryable<Merchant> GetMerchants()
        {
            return db.Merchants;
        }

        // GET: api/Merchants/5
        [ResponseType(typeof(Merchant))]
        public IHttpActionResult GetMerchant(int id)
        {
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return NotFound();
            }

            return Ok(merchant);
        }

        [Route("api/AddMerchant")]
        public IHttpActionResult AddMerchant(string name, string website, string imagePath = null)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var merchant = db.Merchants.Where(x => x.Name.ToLower() == name.ToLower()).FirstOrDefault();

            if (merchant != null)
                return BadRequest();

            Merchant newMerchant = new Merchant
            {
                Name = name,
                Website = website,
                ImagePath = imagePath
            };

            db.Merchants.Add(newMerchant);
            db.SaveChanges();

            return Ok(); ;
        }

        // PUT: api/Merchants/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMerchant(int id, Merchant merchant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != merchant.Id)
            {
                return BadRequest();
            }

            db.Entry(merchant).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MerchantExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Merchants
        [ResponseType(typeof(Merchant))]
        public IHttpActionResult PostMerchant(Merchant merchant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Merchants.Add(merchant);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = merchant.Id }, merchant);
        }

        // DELETE: api/Merchants/5
        [ResponseType(typeof(Merchant))]
        public IHttpActionResult DeleteMerchant(int id)
        {
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return NotFound();
            }

            db.Merchants.Remove(merchant);
            db.SaveChanges();

            return Ok(merchant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MerchantExists(int id)
        {
            return db.Merchants.Count(e => e.Id == id) > 0;
        }
    }
}