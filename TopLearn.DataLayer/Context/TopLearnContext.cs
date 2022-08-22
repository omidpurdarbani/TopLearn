using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.DataLayer.Context
{
    public class TopLearnContext:DbContext
    {
        public TopLearnContext(DbContextOptions<TopLearnContext> options):base(options)
        {

        }

        #region User
        
        public DbSet<User> users { get; set; }

        public DbSet<Role> roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        #endregion

      

    }
}
