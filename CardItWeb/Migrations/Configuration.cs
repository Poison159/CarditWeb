namespace CardItWebApp.Migrations
{
    using CardItWeb.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CardItWebApp.Database.CardItDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(CardItWebApp.Database.CardItDbContext context)
        {
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);


            if (!context.Users.Any(x => x.UserName == "admin@Cardit.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@Cardit.com",
                    Email = "admin@Cardit.com"
                };
                userManager.Create(user, "Pa$$w0rd1");
                context.Roles.AddOrUpdate(x => x.Name, new IdentityRole { Name = "Admin" });
                context.SaveChanges();
                userManager.AddToRole(user.Id, "Admin");
            }
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
