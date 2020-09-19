using CardItWeb.Models;
using CardItWebApp.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CardItWebApp.Database
{
    public class CardItDbContext : IdentityDbContext<ApplicationUser>
    {
        public CardItDbContext() : base("CardIt", throwIfV1Schema: false)
        {
        }

        public static CardItDbContext Create()
        {
            return new CardItDbContext();
        }

        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }
}