using CashDesk.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CashDesk
{
    public class MemberContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public DbSet<Deposit> Deposits { get; set; }

        public DbSet<Membership> Memberships { get; set; }

        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MyDb");
        }
    }
}
