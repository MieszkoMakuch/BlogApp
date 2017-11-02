namespace BlogApp.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Globalization;

    internal sealed class Configuration : DbMigrationsConfiguration<BlogApp.Models.BlogDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "BlogApp.Models.BlogDbContext";
        }

        protected override void Seed(BlogApp.Models.BlogDbContext context)
        {
            if (!context.Roles.Any())
            {
                this.CreateRole(context, "Admin");
                this.CreateRole(context, "User");
            }

            var adminEmail = "admin@admin.com";
            if (!context.Users.Any())
            {
                this.CreateUser(context, adminEmail, "Admin", "12345678");
                this.SetRoleToUser(context, "admin@admin.com", "Admin");

                var janKowalskiEmail = "jankowalski@gmaill.com";
                this.CreateUser(context, janKowalskiEmail, "Jan Kowalski", "12345678");
                this.SetRoleToUser(context, janKowalskiEmail, "Admin");

                var adamNowakEmail = "adamnowak@gmaill.com";
                this.CreateUser(context, adamNowakEmail, "Adam Nowak", "12345678");
                this.SetRoleToUser(context, adamNowakEmail, "User");
            }

            // Populate DB with posts
            if (!context.Publications.Any())
            {
                var adminUserId = context
                        .Users
                        .Where(u => u.Email == adminEmail)
                        .First()
                        .Id;

                AddPublication(
                    context, adminUserId,
                    "Ford's New Drift Stick for Focus RS Steps Up",
                    "Ford must really like the word ďdrift,Ē as well as the vehicular sliding it describes, a lot. The all-wheel-drive, turbocharged Ford Focus RS hot hatch has a Drift mode, in which more power is sent to the rear axle than typical and apportioned between the left and right rear wheels to enable silly, smoky oversteer. Now, RS owners can add more drifticality in the form of Ford Performanceís all-new ďDrift Stick,Ē introduced during the 2017 SEMA show. Essentially an electronically controlled, quick-releasing parking brake, the widget enables Focus RS drivers to pull a lever and lock up the rear wheels to initiate a drift. If youíve ever seen one of Ken Blockís Gymkhana videos, the concept should be familiar, only Blockís cars utilize a heavier-duty hydraulically actuated rear-axle brake. Since installing a similar setup in a normal car is arduous, Ford came up with a different, easier-to-implement solution.",
                    "https://blog.caranddriver.com/wp-content/uploads/2017/10/Ford-Performance-Drift-Stick-3-626x383.jpg",
                    "31.10.2017 20:01:34");

                AddPublication(
                   context, adminUserId,
                   "2019 Porsche Cayenne Once again,",
                   "It took Porsche 53 years to sell a million 911s. Porsche has sold 770,000 Cayennes in the 15 years since it was launched in 2002, and the millionth will probably be built in about three years. As much as we love and cherish the sports cars from Stuttgart, Porsche in the 21st century is the house that the Cayenne SUV built. Itís clear that Porsche didnít want to mess with success, so the new third-generation Cayenne looks a lot like previous Cayennes. Softly rounded front fenders, gaping air intakes, a bulging hood, and the shape of the greenhouse are all clear Cayenne traits that have carried through. Itís slightly lower and wider, but Porsche didnít radically alter the formula. It could have, as the 2019 model is entirely new.\n" +
                    "No sheetmetal carries over, and the Cayenne is now on a new platform.At this point, though, changing the model into something completely different would make about as much sense as replacing the 911 with a front - engine sports car.Built on the MLB platform that also forms the basis for most Audisóincluding the Q7 SUVóthe Cayenne retains its 113.9 - inch wheelbase, but the new architecture brings more aluminum to the structure, which reduces mass slightly.A longitudinally mounted engine is located just ahead of the front axle, which contributes to the V - 8Ėpowered Cayenne carrying 57 percent of its weight up front, a very Audi - like number.The lighter V - 6 models are slightly less nose heavy.The familiar and excellent ZF - sourced eight - speed automatic bolts directly to the engine and now incorporates the front differential.Order the Sport Chrono package($1130) and the transmission adds a launch - control function that reduces zero - to - 60 - mph times by about 0.3 second for each engine, according to Porsche.",
                   "https://hips.hearstapps.com/amv-prod-cad-assets.s3.amazonaws.com/images/17q4/692997/2019-porsche-cayenne-first-drive-review-car-and-driver-photo-694298-s-original.jpg?crop=1xw:1xh;center,center&resize=900:*",
                   "31.10.2017 23:49:12");
            }

        }

        private void AddPublication(BlogDbContext context, String userId, String title, String content, String link, String dateTimeString)
        {
            var dateTime = DateTime.ParseExact(dateTimeString, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            var publication = new Publication
            {
                AuthorId = userId,
                DateTime = DateTime.ParseExact(dateTimeString, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                Title = title,
                Content = content,
                Link = link
            };
            context.Publications.Add(publication);
            context.SaveChanges();
        }
        private void SetRoleToUser(BlogDbContext context, string email, string role)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var user = context.Users.Where(u => u.Email == email).First();
            var result = userManager.AddToRole(user.Id, role);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));
            }
        }

        private void CreateUser(BlogDbContext context, string email, string fullName, string password)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            userManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireDigit = false,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false,
            };

            var user = new ApplicationUser
            {
                UserName = email,
                FullName = fullName,
                Email = email,
            };

            var result = userManager.Create(user, password);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));
            }
        }

        private void CreateRole(BlogDbContext context, string roleName)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var result = roleManager.Create(new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(";", result.Errors));
            }
        }

    }
}
