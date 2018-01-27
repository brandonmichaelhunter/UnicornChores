using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ParentsRules.Models;
using ParentsRules.Models.Chroes;
using ParentsRules.Models.Rooms;
using ParentsRules.Models.DashboardViewModels;
using ParentsRules.Models.ManageViewModels;
namespace ParentsRules.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> AccountUsers { get; set; }
        public DbSet<UserConformationRequests> UserConformationRequests { get; set; }
        public DbSet<AccountAssociations> AccountAssociations { get; set; }
        public DbSet<ChoreTypes> ChoreTypes { get; set; }
        public DbSet<UserChores> UserChores { get; set; }
        public DbSet<RoomTypes> RoomTypes { get; set; }
        public DbSet<UserRooms> UserRooms { get; set; }
        public DbSet<ChildrenWork> ChildWorkList { get; set; }
        public DbSet<CompletedChildrenWork> CompletedChildWorkList { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ApplicationUser>().ToTable("AccountUsers");
            builder.Entity<UserConformationRequests>().ToTable("UserConformationRequests");
            builder.Entity<AccountAssociations>().ToTable("AccountAssociations");
            builder.Entity<ChildrenWork>().ToTable("ChildrenWork");
            builder.Entity<CompletedChildrenWork>().ToTable("CompletedChildrenWork");

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<ParentsRules.Models.DashboardViewModels.DashboardViewModel> DashboardViewModel { get; set; }
        public DbSet<ParentsRules.Models.ManageViewModels.FriendViewModel> FriendViewModel { get; set; }
        public DbSet<ParentsRules.Models.ManageViewModels.FriendsRequestViewModel> FriendsRequestViewModel { get; set; }
        public DbSet<ParentsRules.Models.ManageViewModels.ChildrenViewModel> ChildrenViewModel { get; set; }
    }
}
