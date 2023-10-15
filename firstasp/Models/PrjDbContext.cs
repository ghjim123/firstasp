using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace firstasp.Models;

public partial class PrjDbContext : DbContext
{
    public PrjDbContext()
    {
    }

    public PrjDbContext(DbContextOptions<PrjDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArticleTable> ArticleTables { get; set; }

    public virtual DbSet<CategoryTable> CategoryTables { get; set; }

    public virtual DbSet<PhotoTable> PhotoTables { get; set; }

    public virtual DbSet<ReplyTable> ReplyTables { get; set; }

    public virtual DbSet<UserTable> UserTables { get; set; }
    string mdf_path = System.IO.Directory.GetCurrentDirectory()+ "\\App_Data";
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={mdf_path}\\asppro.mdf;Integrated Security=True;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Chinese_Taiwan_Stroke_CI_AS");

        modelBuilder.Entity<ArticleTable>(entity =>
        {
            entity.HasKey(e => e.ArticleTitle).HasName("PK__tmp_ms_x__47E2AAF3DCE911E8");

            entity.ToTable("article_table");

            entity.Property(e => e.ArticleTitle)
                .HasMaxLength(50)
                .HasColumnName("article_title");
            entity.Property(e => e.ArticleCategory)
                .HasMaxLength(50)
                .HasColumnName("article_category");
            entity.Property(e => e.ArticleContent).HasColumnName("article_content");
            entity.Property(e => e.ArticleDate)
                .HasMaxLength(20)
                .HasColumnName("article_date");
            entity.Property(e => e.PhotoName)
                .HasMaxLength(20)
                .HasColumnName("photo_name");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .HasColumnName("user_id");

            entity.HasOne(d => d.ArticleCategoryNavigation).WithMany(p => p.ArticleTables)
                .HasForeignKey(d => d.ArticleCategory)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_category_table_ToTable");

            entity.HasOne(d => d.PhotoNameNavigation).WithMany(p => p.ArticleTables)
                .HasForeignKey(d => d.PhotoName)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_photo_ToTable");

            entity.HasOne(d => d.User).WithMany(p => p.ArticleTables)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_article_table_ToTable");
        });

        modelBuilder.Entity<CategoryTable>(entity =>
        {
            entity.HasKey(e => e.ArticleCategory).HasName("PK__tmp_ms_x__FD43734D6245BF1E");

            entity.ToTable("category_table");

            entity.Property(e => e.ArticleCategory)
                .HasMaxLength(50)
                .HasColumnName("article_category");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<PhotoTable>(entity =>
        {
            entity.HasKey(e => e.PhotoName).HasName("PK__tmp_ms_x__D4C4946C1B9FBC23");

            entity.ToTable("photo_table");

            entity.Property(e => e.PhotoName)
                .HasMaxLength(20)
                .HasColumnName("photo_name");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.PhotoTables)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_photo_table_ToTable");
        });

        modelBuilder.Entity<ReplyTable>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("reply_table");

            entity.Property(e => e.ArticleCategary)
                .HasMaxLength(50)
                .HasColumnName("article_categary");
            entity.Property(e => e.ArticleTitle)
                .HasMaxLength(50)
                .HasColumnName("article_title");
            entity.Property(e => e.ReplyContent).HasColumnName("reply_content");
            entity.Property(e => e.ReplyDate)
                .HasMaxLength(20)
                .HasColumnName("reply_date");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .HasColumnName("user_id");

            entity.HasOne(d => d.ArticleTitleNavigation).WithMany()
                .HasForeignKey(d => d.ArticleTitle)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_reply_table_ToTable_1");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_reply_table_ToTable");
        });

        modelBuilder.Entity<UserTable>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__tmp_ms_x__B9BE370F428577CF");

            entity.ToTable("user_table");

            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .HasColumnName("user_id");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(50)
                .HasColumnName("user_email");
            entity.Property(e => e.UserPwd)
                .HasMaxLength(10)
                .HasColumnName("user_pwd");
            entity.Property(e => e.UserRole)
                .HasMaxLength(10)
                .HasColumnName("user_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
