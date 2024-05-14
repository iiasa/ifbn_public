// <copyright file="IFBNContext.cs" company="IIASA">
// Copyright (c) IIASA. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace IFBN.Data
{
	using System.Linq;
	using System.Text.RegularExpressions;
	using IFBN.Data.Entities;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Metadata.Internal;

	public class IFBNContext : IdentityDbContext
	{
		public IFBNContext(DbContextOptions<IFBNContext> options)
			: base(options)
		{
		}

		public IFBNContext()
		{
		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }

		public DbSet<Census> Censuses { get; set; }

		public DbSet<Code> Codes { get; set; }

		public DbSet<Family> Families { get; set; }

		public DbSet<FeatureInstitution> FeatureInstitutions { get; set; }

		public DbSet<FeaturePrincipalInvestigator> FeaturePrincipalInvestigators { get; set; }

		public DbSet<Feature> Features { get; set; }

		public DbSet<Genus> Genera { get; set; }

		public DbSet<Institution> Institutions { get; set; }

		public DbSet<Measurement> Measurements { get; set; }

		public DbSet<Method> Methods { get; set; }

		public DbSet<Photo> Photos { get; set; }

		public DbSet<PlotType> PlotTypes { get; set; }

		public DbSet<PrincipalInvestigator> PrincipalInvestigators { get; set; }

		public DbSet<Species> Species { get; set; }

		public DbSet<TaxonomicIdentification> TaxonomicIdentifications { get; set; }

		public DbSet<TypeCode> TypeCodes { get; set; }

		public DbSet<TypeMethod> TypeMethods { get; set; }

		public DbSet<Type> Types { get; set; }

		public DbSet<TypeUnit> TypeUnits { get; set; }

		public DbSet<Unit> Units { get; set; }

		public DbSet<Download> Downloads { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{

                //localhost
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;User Id=xxx;Password=xxx;Database=xxx;Command Timeout=0;")
                    .UseLazyLoadingProxies();

            }
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<PlotType>().HasAlternateKey(x => x.Name);

			modelBuilder.Entity<Code>().HasAlternateKey(x => x.Name);
			modelBuilder.Entity<Type>().HasAlternateKey(x => x.Name);
			modelBuilder.Entity<Method>().HasAlternateKey(x => x.Name);

			modelBuilder.Entity<FeatureInstitution>().HasKey(x => new { x.FeatureId, x.InstitutionId });
			modelBuilder.Entity<FeaturePrincipalInvestigator>().HasKey(x => new { x.FeatureId, x.PrincipalInvestigatorId });

			modelBuilder.Entity<TypeUnit>().HasKey(x => new { x.TypeId, x.UnitId });
			modelBuilder.Entity<TypeMethod>().HasKey(x => new { x.TypeId, x.MethodId });
			modelBuilder.Entity<TypeCode>().HasKey(x => new { x.TypeId, x.CodeId });

			// DO NOT MODIFY THE ORDER HERE!
			modelBuilder.Entity<Measurement>().HasOne(t => t.TypeUnit).WithMany(t => t.Measurements).HasForeignKey("TypeId", "UnitId");
			modelBuilder.Entity<Measurement>().HasOne(t => t.TypeMethod).WithMany(t => t.Measurements).HasForeignKey("TypeId", "MethodId");
			modelBuilder.Entity<Measurement>().HasOne(t => t.TypeCode).WithMany(t => t.Measurements).HasForeignKey("TypeId", "CodeId");

			foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes())
			{
				Regex underscoreRegex = new Regex(@"(?<=.)([A-Z])");
				string tableName = entity.Relational().TableName.StartsWith("AspNet") ? entity.Relational().TableName
					: entity.DisplayName();

				entity.Relational().TableName = underscoreRegex.Replace(tableName, @"_$0").ToLower();

				entity.GetProperties()
					.ToList()
					.ForEach(x => x.Relational().ColumnName = underscoreRegex.Replace(x.Relational().ColumnName, @"_$0").ToLower());

				if (entity.FindPrimaryKey() != null)
				{
					entity.FindPrimaryKey().Relational().Name = entity.FindPrimaryKey().Relational().Name.ToLower();
				}

				entity.GetForeignKeys().ToList().ForEach(x => x.Relational().Name = x.Relational().Name.ToLower());
				entity.GetIndexes().ToList().ForEach(x => x.Relational().Name = x.Relational().Name.ToLower());
			}
		}
	}
}