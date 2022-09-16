namespace TestOne.DAL
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using TestOne.Models;

    public class BlogsContext : DbContext
    {
        // Do not change names of these properties
        public DbSet<BlogEntity> BlogsEntities { get; set; }
        public DbSet<PostEntity> PostsEntities { get; set; }

        /// <summary>
        /// Sobreescritura del Método OnConfiguring de la clase DbContext – Que realiza la configuración del proveedor de datos que utilizará el DbContext.
        /// </summary>
        /// <param name="optionsBuilder">DbContextOptionsBuilder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                // CONFIGURACION DE SQLITE IN-MEMORY
                optionsBuilder.UseSqlite("Filename=./messagedb.sqlite;");                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogEntity>().ToTable("blogs");
            modelBuilder.Entity<BlogEntity>().Property(x=>x.BlogId).HasColumnName("blog_id");
            modelBuilder.Entity<BlogEntity>().HasKey(x=>x.BlogId);
            modelBuilder.Entity<BlogEntity>().Property(x=>x.IsActive).IsRequired(true);
            modelBuilder.Entity<BlogEntity>().Property(x=>x.Name).IsRequired(true);
            modelBuilder.Entity<BlogEntity>().Property(x=>x.Name).HasMaxLength(50);

            modelBuilder.Entity<PostEntity>().ToTable("articles");
            modelBuilder.Entity<PostEntity>().HasKey(x=>x.PostId);
            modelBuilder.Entity<PostEntity>().Property(x=>x.Name).IsRequired(true);
            modelBuilder.Entity<PostEntity>().Property(x=>x.Content).IsRequired(true);
            modelBuilder.Entity<PostEntity>().Property(x=>x.Created).IsRequired(true);
            modelBuilder.Entity<PostEntity>().Property(x=>x.ParentId).IsRequired(true);
            modelBuilder.Entity<PostEntity>().Property(x=>x.PostId).HasColumnName("post_id");

            modelBuilder.Entity<PostEntity>().Property(x=>x.ParentId).HasColumnName("blog_id");

            modelBuilder.Entity<PostEntity>().HasOne(x => x.Blog).WithMany(x => x.Articles).HasForeignKey(x=>x.ParentId);


            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            
            this.ChangeTracker.DetectChanges();
            var added = this.ChangeTracker.Entries()
                        .Where(t => t.State == EntityState.Added)
                        .Select(t => t.Entity)
                        .ToArray();

            foreach (var entity in added)
            {
                if (entity is BlogEntity blog)
                {
                    //validate properties
                    foreach (var prop in typeof(BlogEntity).GetProperties()
                        .Where(x=>x.GetCustomAttributes(true).Any(y=>y.GetType() ==typeof(System.ComponentModel.DataAnnotations.RequiredAttribute))).Select(x=>x))
                    {
                        if (prop.Name == nameof(BlogEntity.BlogId))
                        {
                            if(Convert.ToUInt32((object)prop.GetValue(blog))!=0)
                            {
                                throw new ValidationException();
                            }

                            if (prop != typeof(Guid))
                            {
                                blog.BlogId = this.BlogsEntities.Count() + 1;
                            }
                            else
                            {
                                prop.SetValue(blog, Guid.NewGuid());
                            }
                        }
                        else 
                        {
                            if (string.IsNullOrEmpty(prop.GetValue(blog).ToString()))
                                throw new ValidationException();
                        }
                    }
                }
                if(entity is PostEntity post)
                {
                    foreach(var prop in typeof(PostEntity).GetProperties()
                        .Where(x=>x.GetCustomAttributes(true).Any(y=>y.GetType()==typeof(System.ComponentModel.DataAnnotations.RequiredAttribute))).Select(x=>x))
                    {
                        if (prop.Name == nameof(PostEntity.PostId))
                        {
                            if (Convert.ToUInt32((object)prop.GetValue(post)) != 0)
                            {
                                throw new ValidationException("Id must be 0 when inserting a new post");
                            }

                            if (prop != typeof(Guid))
                            {
                                post.PostId = this.PostsEntities.Count() + 1;
                            }
                            else
                            {
                                prop.SetValue(post, Guid.NewGuid());
                            }
                        }
                        else if(prop.Name==nameof(PostEntity.ParentId))
                        {
                            if(Convert.ToInt32(prop.GetValue(post).ToString())==0)
                            {
                                throw new ValidationException("post must contain a foreign key to a blog");
                            }
                        }
                        else
                        {
                            var val = prop.GetValue(post).ToString();
                            if(string.IsNullOrEmpty(val))
                            {
                                throw new ValidationException();
                            }

                            if (prop == typeof(DateTime))
                            {
                                if(DateTime.Parse(val) ==DateTime.MinValue)
                                {
                                    throw new ValidationException();
                                }
                            }
                        }

                    }
                }
            }

            var modified = this.ChangeTracker.Entries()
                        .Where(t => t.State == EntityState.Modified)
                        .Select(t => t.Entity)
                        .ToArray();

            foreach (var entity in modified)
            {
                if (entity is BlogEntity blog)
                {
                    foreach (var prop in typeof(BlogEntity).GetProperties()
                        .Where(x => x.GetCustomAttributes(true).Any(y => y.GetType() == typeof(System.ComponentModel.DataAnnotations.RequiredAttribute))).Select(x => x))
                    {
                        if (prop.Name == nameof(BlogEntity.BlogId))
                        {
                            if (Convert.ToUInt32((object)prop.GetValue(blog)) == 0)
                            {
                                throw new ValidationException();
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(prop.GetValue(blog).ToString()))
                                throw new ValidationException();
                        }
                    }
                }
                if (entity is PostEntity post)
                {
                    foreach (var prop in typeof(PostEntity).GetProperties()
                        .Where(x => x.GetCustomAttributes(true).Any(y => y.GetType() == typeof(System.ComponentModel.DataAnnotations.RequiredAttribute))).Select(x => x))
                    {
                        if (prop.Name == nameof(PostEntity.PostId))
                        {
                            if (Convert.ToUInt32((object)prop.GetValue(post)) == 0 || !this.PostsEntities.Any(x=>x.PostId ==post.PostId))
                            {
                                throw new ValidationException($"{prop.Name} must have a valid value cannot insert 0 index or there is not entry with this key");
                            }
                            
                        }
                        else if (prop.Name == nameof(PostEntity.ParentId))
                        {
                            if (Convert.ToInt32(prop.GetValue(post).ToString()) == 0 || !this.BlogsEntities.Any(x => x.BlogId == post.ParentId))
                            {
                                throw new ValidationException("post must contain a foreign key to a blog");
                            }
                        }
                        else
                        {
                            var val = prop.GetValue(post).ToString();
                            if (string.IsNullOrEmpty(val))
                            {
                                throw new ValidationException($"{prop.Name} must not be null");
                            }

                            if (prop == typeof(DateTime))
                            {
                                if (DateTime.Parse(val) == DateTime.MinValue)
                                {
                                    throw new ValidationException($"{prop.Name} must have a valid value");
                                }
                            }
                        }

                    }
                }
            }
            return base.SaveChanges();
        }
            
    }
}
