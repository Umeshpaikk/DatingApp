using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DatatContext : IdentityDbContext<AppUser,AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DatatContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<UserLike>      Likes{get;set;}
        public DbSet<Message>       Messages{get;set;}

        public DbSet<Group>         Groups{get;set;}
        public DbSet<Connection>    Connections{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(u => u.UserId)
            .IsRequired();

            
            modelBuilder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(u => u.RoleId)
            .IsRequired();


            modelBuilder.Entity<UserLike>()
            .HasKey(x=> new {x.SourceUserId, x.LikedUserId});

            modelBuilder.Entity<UserLike>()
            .HasOne(s => s.SourceUserRef)
            .WithMany(l => l.LikedUsersBase)
            .HasForeignKey(s => s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade);
        
            modelBuilder.Entity<UserLike>()
            .HasOne(s => s.LikedUserRef)
            .WithMany(l => l.LikedByUsersBase)
            .HasForeignKey(s => s.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Message>()
            .HasOne(u => u.Recipient)
            .WithMany( m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany( m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        }


    }
}