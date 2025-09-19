using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement2.Models;

public partial class LibraryDbContext : DbContext
{
    public LibraryDbContext()
    {
    }

    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Borrowed> Borroweds { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Librarian> Librarians { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=FENXIHALIN-2412\\SQLEXPRESS;Database=LibraryDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__AD0500A64451CAE2");

            entity.HasIndex(e => e.Email, "UQ__Admins__AB6E61644EF32954").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("adminId");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__8E2731B9C552F13F");

            entity.Property(e => e.AuthorId).HasColumnName("authorId");
            entity.Property(e => e.AuthorName)
                .HasMaxLength(100)
                .HasColumnName("author_name");
            entity.Property(e => e.Bio)
                .HasMaxLength(300)
                .HasColumnName("bio");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__8BE5A10DA370323F");

            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.AuthorId).HasColumnName("authorId");
            entity.Property(e => e.CateId).HasColumnName("cateId");
            entity.Property(e => e.Description)
                .HasMaxLength(300)
                .HasColumnName("description");
            entity.Property(e => e.Publisher)
                .HasMaxLength(100)
                .HasColumnName("publisher");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasColumnName("title");
            entity.Property(e => e.YearPublished).HasColumnName("year_published");

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Book_Author");

            entity.HasOne(d => d.Cate).WithMany(p => p.Books)
                .HasForeignKey(d => d.CateId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Book_Categories");
        });

        modelBuilder.Entity<Borrowed>(entity =>
        {
            entity.HasKey(e => e.BorrowId).HasName("PK__Borrowed__73982486F6DD3653");

            entity.ToTable("Borrowed");

            entity.Property(e => e.BorrowId).HasColumnName("borrowId");
            entity.Property(e => e.BookId).HasColumnName("bookId");
            entity.Property(e => e.BorrowDate).HasColumnName("borrowDate");
            entity.Property(e => e.DueDate).HasColumnName("dueDate");
            entity.Property(e => e.LibraId).HasColumnName("libraId");
            entity.Property(e => e.ReturnDate).HasColumnName("returnDate");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("studentId");

            entity.HasOne(d => d.Book).WithMany(p => p.Borroweds)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Borrowed_Books");

            entity.HasOne(d => d.Libra).WithMany(p => p.Borroweds)
                .HasForeignKey(d => d.LibraId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Borrowed_Librarians");

            entity.HasOne(d => d.Student).WithMany(p => p.Borroweds)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Borrowed_Students");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CateId).HasName("PK__Categori__A88B4DE49FE993DF");

            entity.Property(e => e.CateId).HasColumnName("cateId");
            entity.Property(e => e.CateName)
                .HasMaxLength(50)
                .HasColumnName("cateName");
        });

        modelBuilder.Entity<Librarian>(entity =>
        {
            entity.HasKey(e => e.LibraId).HasName("PK__Libraria__1C1CFE10F2CF49A7");

            entity.HasIndex(e => e.Email, "UQ__Libraria__AB6E6164ECE8312A").IsUnique();

            entity.Property(e => e.LibraId).HasColumnName("libraId");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.HireDate).HasColumnName("hireDate");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__CD98462A2AF37019");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__B194786138B0BF14").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__4D11D63C2A371C51");

            entity.HasIndex(e => e.Email, "UQ__Students__AB6E61649224911E").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("studentId");
            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__CD3149CC1AE0BBAC");

            entity.Property(e => e.UserRoleId).HasColumnName("userRoleId");
            entity.Property(e => e.AdminId).HasColumnName("adminId");
            entity.Property(e => e.LibraId).HasColumnName("libraId");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.StudentId).HasColumnName("studentId");

            entity.HasOne(d => d.Admin).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserRoles_Admins");

            entity.HasOne(d => d.Libra).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.LibraId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserRoles_Librarians");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_UserRoles_Roles");

            entity.HasOne(d => d.Student).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_UserRoles_Students");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
