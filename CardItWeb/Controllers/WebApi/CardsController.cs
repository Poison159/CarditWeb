using CardItWeb.Models;
using CardItWebApp.Database;
using CardItWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CardItWebApp.Controllers.WebApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CardsController : ApiController
    {
        private CardItDbContext dbContext = new CardItDbContext();

        // GET api/cards
        public IEnumerable<Card> Get()
        {
            var cards = dbContext.Cards.ToList();

            return cards;
        }

        // GET api/cards/5
        public Card Get(int id)
        {
            var card = dbContext.Cards.Where(x => x.Id == id).FirstOrDefault();
            return card;
        }

        // POST api/cards
        public void Post([FromBody]string value)
        {
            //How can one add a new Card??
            //Already have user info
            //Already selected the merchant
        }

        // PUT api/cards/x
        [Route("api/Edit")]
        [HttpGet]
        public object Put(int id, string cardNUmber)
        {
            int number;
            foreach (var item in cardNUmber)
            {
                if (!int.TryParse(item.ToString(), out number))
                {
                    return new { Errors = "Card number is invalid" };
                }
            }
            var existingCard = dbContext.Cards
                                            .Where(s => s.Id == id)
                                            .FirstOrDefault<Card>();
            Helper.deleteOldBarcode(existingCard.CardNumber);
            var res = Helper.createBarcode(cardNUmber);
            if (res == true)
            {
                
                if (!ModelState.IsValid)
                    return new { Errors = "Could not create card please check number and try again" };

                if (existingCard != null)
                {
                    existingCard.CardNumber = cardNUmber;

                    dbContext.SaveChanges();
                }
                else
                    return new { Errors = "Card not found" };

                return new { };
            }
            else {
                return new { Errors = "Card number is invalid" };
            }
        }
        [Route("api/AddCard")]
        [HttpGet]
        public object AddCard(string merchantName, string cardNumber, int userId )
        {
            var userCards = dbContext.Cards.Where(x => x.userId == userId).ToList();
            if (userCards.Where(x => x.CardNumber.Equals(cardNumber)).Count() != 0) {
                return new { Errors = "Card number already exists" };
            }
            try
            {
                var res = Helper.createBarcode(cardNumber);
                if (res == true) {
                    var card = new Card();
                    var merchant = dbContext.Merchants.ToList().First(x => x.Name.ToLower().Trim() == merchantName.ToLower().Trim().ToString());
                    card.userId = userId;
                    card.CardNumber = cardNumber;
                    card.merchantId = merchant.Id; 
                    dbContext.Cards.Add(card);

                    dbContext.SaveChanges();
                    return new { card = card, merchant = merchant };
                }else{
                    return new { Errors = "Could not add card" };
                }
                
            }
            catch (Exception)
            {
                return new { Errors = "Could not add card" };
            }
            
        }

        // DELETE api/cards/5
        public object Delete(int id, string email)
        {
            if (id <= 0)
                return null;
            try
            {
                var user = dbContext.AppUsers.First(x => x.Email == email);
                var card = dbContext.Cards.Where(s => s.Id == id).FirstOrDefault();
                Merchant merchant = dbContext.Merchants.ToList().First(x => x.Id == card.merchantId);
                dbContext.Entry(card).State = System.Data.Entity.EntityState.Deleted;
                Helper.deleteOldBarcode(card.CardNumber); // Delete car barcode image
                dbContext.SaveChanges();
                return merchant;
            }
            catch (Exception)
            {
                return new {Errors="could not remove card" };
            } 
            
        }

    }
}
