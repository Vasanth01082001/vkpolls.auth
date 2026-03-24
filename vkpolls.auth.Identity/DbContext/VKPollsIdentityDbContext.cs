using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace vkpolls.auth.Identity.DbContext
{
    public class VKPollsIdentityDbContext : IdentityDbContext<IdentityUser>
    {
        public VKPollsIdentityDbContext(DbContextOptions<VKPollsIdentityDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUser>(entity =>
            {
                // Unique index for Email when NOT NULL
                entity.HasIndex(e => e.Email)
                .HasDatabaseName("IX_AspNetUsers_Email")
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");

                entity.HasIndex(e => e.PhoneNumber)
                .HasDatabaseName("IX_AspNetUsers_PhoneNumber")
                .IsUnique()
                .HasFilter("[PhoneNumber] IS NOT NULL");
            });
        }
    }
}
