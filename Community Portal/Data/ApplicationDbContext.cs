using Community_Portal.Models;
using CommunityPortal.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Community_Portal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

    }
}

