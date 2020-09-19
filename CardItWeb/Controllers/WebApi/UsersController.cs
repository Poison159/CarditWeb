using CardItWeb.Models;
using CardItWebApp.Database;
using CardItWebApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CardItWebApp.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsersController : ApiController
    {
        private CardItDbContext dbContext = new CardItDbContext();

        // GET api/users/x
        public IHttpActionResult Get(int userId)
        {
            var user = dbContext.AppUsers.First(x => x.Id == userId);
            //Get User cards
            var cards = dbContext.Cards.Where(x => x.userId == user.Id).ToList();

            List<Merchant> merchants = new List<Merchant>();

            foreach (var card in cards)
            {
                //Get Merchant for each card
                var merchant = dbContext.Merchants.Where(x => x.Id == card.merchantId).FirstOrDefault();

                merchants.Add(merchant);
            }
            cards.Reverse();
            merchants.Reverse();
            CardRepresentative userCard = new CardRepresentative()
            {
                User = user,
                Cards =  cards,
                Merchants = merchants
            };

            return Ok(userCard);
            
            
        }



        [Route("api/RegisterUser")]
        [HttpGet]
        public object RegisterUser(string name, string email, string mobileNumber, string password)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));
            UserManager.UserValidator = new UserValidator<ApplicationUser>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            var user = new ApplicationUser();

            user.Email = email.ToLower().Trim();
            user.UserName = email.ToLower().Trim();
            var result = UserManager.Create(user, password);
            var appUser = new AppUser() {Name = name, Email = email.ToLower().Trim()};
            if (result.Succeeded)
            {
                var token = Helper.saveAppUserAndToken(appUser, dbContext);
                return new { _token = token, _user = appUser };
            }
            else
            {
                return result;
            }
        }

        [Route("api/CheckToken")]
        [HttpGet]
        public object getToken(string email, string password)
        {
            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);
            ApplicationUser user = null;
            AppUser appUser = null;


            try
            {
                appUser = dbContext.AppUsers.First(x => x.Email.ToLower().Trim() == email.ToLower().Trim());
                user = dbContext.Users.First(x => x.Email.ToLower().Trim() == email.ToLower().Trim());
            }catch (Exception){
                return new { Errors = "User not found" };
            }
            if (user != null)
            {
                var isMatch = userManager.CheckPassword(user, password);
                if (isMatch)
                {
                    var token = dbContext.Tokens.ToList().First(x => x._userId.ToLower().Trim() == email.ToLower().Trim());
                    if (token != null)
                    {
                        token._grantDate    = DateTime.Now;
                        token._expiryDate   = DateTime.Now.AddDays(60);
                        dbContext.SaveChanges();
                        return new { _token = token, _user = appUser };
                    }
                    else
                    {
                        return new { Errors= "User not found"};
                    }
                }
                else
                {
                    return new { Errors = "Incorrect cridentials" };
                }
            }
            else {
                return new { Errors = "User not found" };
            }
        }

        [Route("api/UserLogin")]
        public IHttpActionResult UserLogin(string name, string email, string mobileNumber, string password)
        {
            //Check if user exists
            var encryptedPassword = EncryptPassword(password);

            var user = dbContext.AppUsers.Where(x => x.Password == encryptedPassword).FirstOrDefault();

            if (user is null)
                //User doesn't exist
                return NotFound();

            //Check for changes
            if (user.Name != name || user.Email != email || user.MobileNumber != mobileNumber)
            {
                user.Name = name;
                user.Email = email;
                user.MobileNumber = mobileNumber;
            }

            dbContext.SaveChanges();

            user.Password = null;

            return Ok(user);
        }

        // POST api/users
        public void Post([FromBody]string value)
        {

        }

        // PUT api/users/x
        public IHttpActionResult Put(AppUser user)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            var existingUser = dbContext.AppUsers
                                        .Where(s => s.Id == user.Id)
                                        .FirstOrDefault<AppUser>();

            if (existingUser != null)
            {
                existingUser.MobileNumber = user.MobileNumber;
                existingUser.Email = user.Email;

                dbContext.SaveChanges();
            }
            else
                return NotFound();

            return Ok();
        }

        // DELETE api/users/5
        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid User id");

            var user = dbContext.AppUsers
                                 .Where(s => s.Id == id)
                                 .FirstOrDefault();

            dbContext.Entry(user).State = System.Data.Entity.EntityState.Deleted;
            dbContext.SaveChanges();

            return Ok();
        }

        public string EncryptPassword(string password)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(password);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            String encryptedPasswordHash = System.Text.Encoding.ASCII.GetString(data);

            return encryptedPasswordHash;
        }
    }

    public class CardRepresentative
    {
        public AppUser User {get; set;}
        public List<Card> Cards {get; set;}

        public List<Merchant> Merchants {get; set;}
    }
}
