using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using vidster_api.Models;

namespace vidster_api.Context;

public partial class VidsterContext : DbContext
{
    public VidsterContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public VidsterContext(DbContextOptions<VidsterContext> options)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public virtual DbSet<Creator> Creators { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagInCreator> TagInCreators { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Work> Works { get; set; }
    
    public virtual DbSet<Service> Services { get; set; }
    
    public virtual DbSet<ServiceOfCreator> ServicesOfCreator { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Creator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("creator_pk");

            entity.ToTable("creator");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.Avatar)
                .HasDefaultValue(
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQioKINfcXK55EtAkOsFMG_CnHibqyNRI-tiPq_fGUVig&s")
                .HasColumnType("character varying")
                .HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Thumbnail)
                .HasDefaultValue("https://publish.nstu.ru/bitrix/templates/aspro-scorp/images/default-banner.jpg")
                .HasColumnType("character varying")
                .HasColumnName("thumbnail");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");

            entity.HasOne(d => d.User).WithMany(p => p.Creators)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("creator_user_id_fk");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_pk");

            entity.ToTable("customer");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Customers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_user_id_fk");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("review_pk");

            entity.ToTable("review");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Creator).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("review_creator_id_fk");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tag_pk");

            entity.ToTable("tag");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<TagInCreator>(entity =>
        {
            entity.ToTable("tag_in_creator");

            entity.HasKey(e => new { e.TagId, e.CreatorId });

            entity.HasOne(e => e.Creator)
                .WithMany(e => e.TagsInCreator)
                .HasForeignKey(e => e.CreatorId)
                .HasConstraintName("creator_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pk");

            entity.ToTable("user");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasDefaultValue(
                    "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQioKINfcXK55EtAkOsFMG_CnHibqyNRI-tiPq_fGUVig&s")
                .HasColumnType("character varying")
                .HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasColumnType("character varying")
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasColumnType("character varying")
                .HasColumnName("last_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PasswordKey).HasColumnName("password_key");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("updated_at");
            entity.Property(e => e.Role)
                .HasColumnType("character varying")
                .HasColumnName("role");
        });

        modelBuilder.Entity<Work>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("work_pk");

            entity.ToTable("work");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("updated_at");
            entity.Property(e => e.Url)
                .HasColumnType("character varying")
                .HasColumnName("url");

            entity.HasOne(d => d.Creator).WithMany(p => p.Works)
                .HasForeignKey(d => d.CreatorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("work_creator_id_fk");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_pk");

            entity.ToTable("service");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<ServiceOfCreator>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_of_creator_pk");

            entity.ToTable("service_of_creator");
            
            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Price)
                .HasColumnName("price");
            entity.Property(e => e.ServiceId)
                .HasColumnName("service_id");
            entity.Property(e => e.CreatorId)
                .HasColumnName("creator_id");
            
            entity.HasOne(e => e.Creator)
                .WithMany(e => e.ServiceOfCreator)
                .HasForeignKey(e => e.CreatorId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}