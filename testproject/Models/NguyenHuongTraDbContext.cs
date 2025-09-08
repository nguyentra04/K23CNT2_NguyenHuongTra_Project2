using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace testproject.Models;

public partial class NguyenHuongTraDbContext : DbContext
{
    public NguyenHuongTraDbContext()
    {
    }

    public NguyenHuongTraDbContext(DbContextOptions<NguyenHuongTraDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Librarian> Librarians { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Reads connection string from configuration (appsettings.json or other sources)
            optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4889E4E3178");

            entity.HasIndex(e => e.UserId, "UQ__Admins__1788CC4DA35682C6").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);

            entity.HasOne(d => d.User).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Admins_Users");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC14632A17A3");

            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.Author1)
                .HasMaxLength(100)
                .HasColumnName("Author");
            entity.Property(e => e.Bio).HasMaxLength(300);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C207E2F76148");

            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Images).HasMaxLength(255);
            entity.Property(e => e.Publisher).HasMaxLength(100);
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.Property(e => e.Status).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.YearPublished).HasColumnName("Year_published");

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Books_Authors");

            entity.HasOne(d => d.Cate).WithMany(p => p.Books)
                .HasForeignKey(d => d.CateId)
                .HasConstraintName("FK_Books_Categories");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CateId).HasName("PK__Categori__27638D14CB452558");

            entity.Property(e => e.CateName).HasMaxLength(50);
        });

        modelBuilder.Entity<Librarian>(entity =>
        {
            entity.HasKey(e => e.LibraId).HasName("PK__Libraria__4A25F0D47C936686");

            entity.HasIndex(e => e.UserId, "UQ__Libraria__1788CC4D270F384F").IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithOne(p => p.Librarian)
                .HasForeignKey<Librarian>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Librarians_Users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A9D85631A");

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52B99096F9446");

            entity.HasIndex(e => e.UserId, "UQ__Students__1788CC4D23335033").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Students_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C38D70D66");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E445D08B21").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105348B2A4F65").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
