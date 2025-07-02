using Core.Entity.Exorabilis;
using Core.Entity.MyCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Configuration;
using Cycle = Core.Entity.MyCore.Cycle;
using Session = Core.Entity.MyCore.Session;

namespace Core.Database
{
    public partial class MyCoreContext : DbContext
    {
        //private string ConnectionString { get; set; }

       public MyCoreContext() {
       }

        public MyCoreContext(DbContextOptions<MyCoreContext> options)
            : base(options)
        {
        }

        public  DbSet<Address> Address { get; set; }
        public  DbSet<AddressType> AddressType { get; set; }
        public  DbSet<ApplicationSettings> ApplicationSettings { get; set; }
        public  DbSet<Component> Component { get; set; }
        public  DbSet<ComponentCategory> ComponentCategory { get; set; }
        public  DbSet<Country> Country { get; set; }
        public  DbSet<Cycle> Cycle { get; set; }
        public  DbSet<Element> Element { get; set; }
        public  DbSet<ElementAction> ElementAction { get; set; }
        public  DbSet<ElementBoutton> ElementBoutton { get; set; }
        public  DbSet<ElementHierarchy> ElementHierarchy { get; set; }
        public  DbSet<ElementType> ElementType { get; set; }
        public  DbSet<Permission> Permission { get; set; }
        public  DbSet<Person> Person { get; set; }
        public  DbSet<Role> Role { get; set; }
        public  DbSet<RoleAccessControl> RoleAccessControl { get; set; }
        public  DbSet<RoleHierarchy> RoleHierarchy { get; set; }
        public  DbSet<Session> Session { get; set; }
        public  DbSet<User> Users { get; set; }
        public  DbSet<UserRole> UserRole { get; set; }
        public  DbSet<UserStatus> UserStatus { get; set; }
        public  DbSet<VersionHistory> VersionHistory { get; set; }
        public  DbSet<VersionHistoryQueries> VersionHistoryQueries { get; set; }
        public  DbSet<Notification> Notification { get; set; }
        public  DbSet<Resources> Resource { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        var environment = _configuration["Environment"];
        //        var connString = _configuration.GetConnectionString($"{environment}_CORE");

        //        optionsBuilder.UseMySQL(connString);
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.HasIndex(e => e.AddressTypeId)
                    .HasName("FK_Address_AddressType");

                entity.HasIndex(e => e.CountryId)
                    .HasName("FK_Address_CountryId");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.AddressTypeId)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.CountryId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Line1)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Line2)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Line3)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Status).HasColumnType("int(2)");

                entity.Property(e => e.TownCity)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Address)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("address_ibfk_2");
            });
            modelBuilder.Entity<Resources>(entity =>
            {
                entity.ToTable("Resources");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.En);

                entity.Property(e => e.Fr);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notification");

                entity.HasIndex(e => e.Receiver)
                    .HasName("Notification_User_Id_fk");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.IsRead)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Link)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("'''#'''");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.Receiver)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Sender)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Type)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.ReceiverNavigation)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.Receiver)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Notification_User_Id_fk");
            });

            modelBuilder.Entity<AddressType>(entity =>
            {
                entity.ToTable("AddressType");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<ApplicationSettings>(entity =>
            {
                entity.ToTable("ApplicationSettings");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Component>(entity =>
            {
                entity.ToTable("Component");

                entity.HasIndex(e => e.ComponentCategoryId)
                    .HasName("FK_Component_ComponentCategory");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.ComponentCategoryId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.ComponentNumber)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status).HasColumnType("int(2)");

                entity.HasOne(d => d.ComponentCategory)
                    .WithMany(p => p.Component)
                    .HasForeignKey(d => d.ComponentCategoryId)
                    .HasConstraintName("component_ibfk_1");
            });

            modelBuilder.Entity<ComponentCategory>(entity =>
            {
                entity.ToTable("ComponentCategory");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("Country");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Cycle>(entity =>
            {
                entity.ToTable("Cycle");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status).HasColumnType("int(2)");
            });

            modelBuilder.Entity<Element>(entity =>
            {
                entity.ToTable("Element");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.DefaultDisplayText)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.ElementNumber)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Url)
                    .HasMaxLength(256)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Step)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Order)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status).HasColumnType("int(2)");

            });

            modelBuilder.Entity<ElementBoutton>(entity =>
            {
                entity.ToTable("ElementBoutton");
                entity.Property(e => e.Id)
                    .HasColumnType("int(11)");
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");
                entity.Property(e => e.ElementId)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");
                entity.Property(e => e.RoleId)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<ElementAction>(entity =>
            {
                entity.ToTable("ElementAction");

                entity.HasIndex(e => e.ElementId)
                    .HasName("FK_ElementAction_Element");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.ActionName)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Controller)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.ElementId)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.RouteName)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status).HasColumnType("int(2)");

                entity.Property(e => e.Url)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<ElementHierarchy>(entity =>
            {
                entity.ToTable("ElementHierarchy");

                entity.HasIndex(e => e.ElementId)
                    .HasName("FK_ElementHierarchy_Element");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.HasIndex(e => e.ParentElementId)
                    .HasName("FK_ElementHierarchy_ParentElement");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.ElementId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.ParentElementId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status).HasColumnType("int(2)");

                entity.HasOne(d => d.Element)
                    .WithMany(p => p.ElementHierarchyElement)
                    .HasForeignKey(d => d.ElementId)
                    .HasConstraintName("elementhierarchy_ibfk_1");

                entity.HasOne(d => d.ParentElement)
                    .WithMany(p => p.ElementHierarchyParentElement)
                    .HasForeignKey(d => d.ParentElementId)
                    .HasConstraintName("elementhierarchy_ibfk_2");
            });

            modelBuilder.Entity<ElementType>(entity =>
            {
                entity.ToTable("ElementType");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permission");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<Person>(entity =>
            {
                entity.ToTable("Person");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MiddleName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Status).HasColumnType("int(2)");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<RoleAccessControl>(entity =>
            {
                entity.ToTable("RoleAccessControl");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.HasIndex(e => e.RoleElementNumber)
                    .HasName("FK_RoleAccessControl");

                entity.HasIndex(e => e.RoleId)
                    .HasName("FK_RoleAccessControl_Role");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.RoleElementNumber)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.RoleId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status).HasColumnType("int(2)");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleAccessControl)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("roleaccesscontrol_ibfk_1");

                entity.HasOne(d => d.Element)
                    .WithMany(p => p.RoleAccessControl)
                    .HasForeignKey(d => d.ElementId)
                    .HasConstraintName("roleaccesscontrol_element_Id_fk");
            });

            modelBuilder.Entity<RoleHierarchy>(entity =>
            {
                entity.ToTable("RoleHierarchy");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.HasIndex(e => e.ParentRoleId)
                    .HasName("FK_RoleHierarchy_ParentRole");

                entity.HasIndex(e => e.RoleId)
                    .HasName("FK_RoleHierarchy_Role");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.ParentRoleId)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.Status).HasColumnType("int(2)");

                entity.HasOne(d => d.ParentRole)
                    .WithMany(p => p.RoleHierarchyParentRole)
                    .HasForeignKey(d => d.ParentRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("rolehierarchy_ibfk_2");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.RoleHierarchyRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("rolehierarchy_ibfk_1");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("Session");

                entity.HasIndex(e => e.UserId)
                    .HasName("FK_Session_user");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("session_ibfk_1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.HasIndex(e => e.Mail)
                    .HasName("user_Mail_uindex")
                    .IsUnique();

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.HasIndex(e => e.PersonId)
                    .HasName("FK_User_Person");

                entity.HasIndex(e => e.UserStatusId)
                    .HasName("FK_User_UserStatus");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.HashCode)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Mail)
                    .HasColumnName("Mail")
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.PersonId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status).HasColumnType("int(2)");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.UserStatusId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.PasswordResetRequest)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.Person)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.PersonId)
                    .HasConstraintName("user_ibfk_1");

                entity.HasOne(d => d.UserStatus)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.UserStatusId)
                    .HasConstraintName("user_ibfk_2");
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.HasIndex(e => e.RoleId)
                    .HasName("FK_UserRole_Role");

                entity.HasIndex(e => e.UserId)
                    .HasName("FK_UserRole_User");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.RoleId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status).HasColumnType("int(2)");

                entity.Property(e => e.UserId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("userrole_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("userrole_ibfk_1");
            });

            modelBuilder.Entity<UserStatus>(entity =>
            {
                entity.ToTable("UserStatus");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.LastUpdatedBy)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<VersionHistory>(entity =>
            {
                entity.ToTable("VersionHistory");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.Description)
                    .HasMaxLength(765)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.ExecutedBy)
                    .HasMaxLength(150)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.MyId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Name)
                    .HasMaxLength(150)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.VersionNumber)
                    .HasColumnType("decimal(17,0)")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<VersionHistoryQueries>(entity =>
            {
                entity.ToTable("VersionHistoryQueries");

                entity.HasIndex(e => e.VersionHistoryId)
                    .HasName("VersionHistoryId");

                entity.Property(e => e.Id).HasMaxLength(64);

                entity.Property(e => e.MyId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Query).HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.VersionHistoryId)
                    .HasMaxLength(64)
                    .HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.VersionHistory)
                    .WithMany(p => p.VersionHistoryQueries)
                    .HasForeignKey(d => d.VersionHistoryId)
                    .HasConstraintName("versionhistoryqueries_ibfk_1");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
