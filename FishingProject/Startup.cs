using FishingProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FishingProject.Startup))]
namespace FishingProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRoles();
        }
        private void CreateRoles()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // creating Creating Customer role    
            if (!roleManager.RoleExists("Participant"))
            {
                var role = new IdentityRole();
                role.Name = "Participant";
                roleManager.Create(role);

            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Organization"))
            {
                var role = new IdentityRole();
                role.Name = "Organization";
                roleManager.Create(role);

            }
        }
    }
}
