using System;
using System.Configuration;
using Core.Entity.Exorabilis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Index = Core.Entity.Exorabilis.Index;

namespace Core.Database
{
    public partial class ExorabilisContext : DbContext
    {
        public ExorabilisContext()
        {
        }
        public ExorabilisContext(DbContextOptions<ExorabilisContext> options)
            : base(options)
        {
        }


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{

        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        var environment = _configuration["Environment"];
        //        var connString = _configuration.GetConnectionString($"{environment}");

        //        optionsBuilder.UseMySql(connString, ServerVersion.AutoDetect(connString));
        //    }
        //}

        //Dummy table for store procedure
        public virtual DbSet<DashboardListDate> DashboardListDate { get; set; }
        public virtual DbSet<DashboardCount> DashboardCount { get; set; }
        public virtual DbSet<DocumentSearch> DocumentSearch { get; set; }
        public virtual DbSet<UserDocumentValidation> UserDocumentValidation { get; set; }

        //Dummy tables for pagination
        //public virtual DbSet<BatchQualityControl> BatchQualityControl { get; set; }
        //public virtual DbSet<BatchQualityControlCount> BatchQualityControlCount { get; set; }
        /* public virtual DbSet<DocumentQualityControl> DocumentQualityControl { get; set; }
         public virtual DbSet<DocumentReview> DocumentReview { get; set; }
         public virtual DbSet<DocumentQualityControlCount> DocumentQualityControlCount { get; set; }
         public virtual DbSet<UserReport> UserReport { get; set; }*/
        //public virtual DbSet<UserReportCount> UserReportCount { get; set; }
        //public virtual DbSet<FinalDocumentValidation> FinalDocumentValidation { get; set; }
        //public virtual DbSet<FinalDocumentValidationCount> FinalDocumentValidationCount { get; set; }
        public virtual DbSet<AcceptRejectFinal> AcceptRejectFinal { get; set; }
        public virtual DbSet<ClearTable> ClearTable { get; set; }
        public virtual DbSet<Batch> Batch { get; set; }
        public virtual DbSet<BatchHistory> BatchHistory { get; set; }
        public virtual DbSet<Chrono> Chrono { get; set; }
        public virtual DbSet<Parameter> Parameter { get; set; }
        public virtual DbSet<Cycle> Cycle { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentDetail> DocumentDetail { get; set; }
        public virtual DbSet<DocumentHistory> DocumentHistory { get; set; }
        public virtual DbSet<DocumentIndex> DocumentIndex { get; set; }
        public virtual DbSet<DocumentLog> DocumentLog { get; set; }
        public virtual DbSet<DocumentStatus> DocumentStatus { get; set; }
        public virtual DbSet<DocumentStep> DocumentStep { get; set; }
        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<DocumentValidation> DocumentValidation { get; set; }
        public virtual DbSet<DuplicateDocumentLink> DuplicateDocumentLink { get; set; }
        public virtual DbSet<FileType> FileType { get; set; }
        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<Image> Image { get; set; }
        public virtual DbSet<Index> Index { get; set; }
        public virtual DbSet<PoliceIndex> PoliceIndex { get; set; }
        public virtual DbSet<IndexDataType> IndexDataType { get; set; }
        public virtual DbSet<RejectionCode> RejectionCode { get; set; }
        public virtual DbSet<ResourceFile> ResourceFile { get; set; }
        public virtual DbSet<Session> Session { get; set; }
        public virtual DbSet<SessionState> SessionState { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }
        public virtual DbSet<WorkStation> WorkStation { get; set; }
        public virtual DbSet<WorkFlow> WorkFlow { get; set; }
        public virtual DbSet<Trademarks> Trademarks { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Document_type> Document_type { get; set; }
        public virtual DbSet<ProjectFolio> Project_folio { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.3-servicing-35854");

            modelBuilder.Entity<Batch>(entity =>
            {
                entity.ToTable("Batch");

                entity.HasIndex(e => e.DocumentStatusId)
                    .HasName("FK_Batch_DocumentStatus");

                entity.HasIndex(e => e.DocumentStepId)
                    .HasName("FK_Batch_DocumentStep");

                entity.HasIndex(e => e.DocumentTypeId)
                    .HasName("FK_Batch_DocumentType");

                entity.HasIndex(e => e.RejectionCodeId)
                    .HasName("FK_Batch_RejectionCode");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.DocumentStatusId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentStepId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentTypeId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");


                entity.Property(e => e.ReviewedBy)
               .HasMaxLength(50)
               .IsUnicode(false)
               .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReviewedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.RescanBy)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.Property(e => e.RescanOn).HasDefaultValueSql("NULL");
                entity.Property(e => e.ExportOn).HasDefaultValueSql("NULL");


                entity.Property(e => e.IndexedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IndexedOn).HasDefaultValueSql("NULL");


                entity.Property(e => e.QualityBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.QualityOn).HasDefaultValueSql("NULL");
                
                entity.Property(e => e.LockedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.ReferenceNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.SanityBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
                
                entity.Property(e => e.FileNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
                entity.Property(e => e.SanityOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.LastStep)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.Property(e => e.Pcname)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsLocked)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ExportStatus)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReasonOther)
                    .HasColumnType("text")

                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LockedBy)
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.NumberOfDocument)
                    .HasColumnType("int(10) unsigned zerofill")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReferenceNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.RejectionCodeId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ScannedOn).HasDefaultValueSql("NULL");
                entity.Property(e => e.ScannedBy).HasDefaultValueSql("NULL");

                entity.Property(e => e.LastUpdatedOn).HasDefaultValueSql("current_timestamp()");

                entity.HasOne(d => d.DocumentStatus)
                    .WithMany(p => p.Batch)
                    .HasForeignKey(d => d.DocumentStatusId)
                    .HasConstraintName("FK_Batch_DocumentStatus");

                entity.HasOne(d => d.DocumentStep)
                    .WithMany(p => p.Batch)
                    .HasForeignKey(d => d.DocumentStepId)
                    .HasConstraintName("FK_Batch_DocumentStep");

                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.Batch)
                    .HasForeignKey(d => d.DocumentTypeId)
                    .HasConstraintName("FK_Batch_DocumentType");

                entity.HasOne(d => d.RejectionCode)
                    .WithMany(p => p.Batch)
                    .HasForeignKey(d => d.RejectionCodeId)
                    .HasConstraintName("FK_Batch_RejectionCode");
            });

            modelBuilder.Entity<BatchHistory>(entity =>
            {
                entity.ToTable("BatchHistory");

                entity.Property(e => e.FileNumber)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.HasIndex(e => e.BatchId)
                    .HasName("FK_BatchHistory_Batch");

                entity.HasIndex(e => e.DocumentStatusId)
                    .HasName("FK_BatchHistory_DocumentStatus");

                entity.HasIndex(e => e.RejectionCode)
                    .HasName("FK_BatchHistory_RejectionCode");

                entity.HasIndex(e => e.WorkStationId)
                    .HasName("FK_BatchHistory_WorkStation");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BatchId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentStatusId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentStepId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.EndedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.LastUpdatedOn).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.NumberOfDocument)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.RejectionCode)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.StartedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.UserId);

                entity.Property(e => e.WorkStationId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.BatchHistory)
                    .HasForeignKey(d => d.BatchId)
                    .HasConstraintName("FK_BatchHistory_Batch");

                entity.HasOne(d => d.DocumentStatus)
                    .WithMany(p => p.BatchHistory)
                    .HasForeignKey(d => d.DocumentStatusId)
                    .HasConstraintName("FK_BatchHistory_DocumentStatus");


                entity.Property(e => e.Remark)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.WorkStation)
                    .WithMany(p => p.BatchHistory)
                    .HasForeignKey(d => d.WorkStationId)
                    .HasConstraintName("FK_BatchHistory_WorkStation");
            });

            modelBuilder.Entity<Chrono>(entity =>
            {
                entity.HasKey(e => e.Racine);

                entity.ToTable("Chrono");

                entity.HasIndex(e => e.LastUpdate)
                    .HasName("idx_update");

                entity.Property(e => e.Racine)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.DatabaseSave)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DatabaseSaveImage)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LastUpdate)
                    .HasColumnName("Last_Update")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.NbRemiseId)
                    .HasColumnName("Nb_RemiseId")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Parameter>(entity =>
            {
                entity.ToTable("Parameter");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasDefaultValueSql("NULL");
                entity.Property(e => e.isInExel)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Value)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.SanityName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ModifiedName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IndexedName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
                entity.Property(e => e.QualityName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
                entity.Property(e => e.ReviewedName)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
                entity.Property(e => e.SanityOn)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IndexedOn)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
                entity.Property(e => e.QualityOn)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
                entity.Property(e => e.ReviewedOn)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
                entity.Property(e => e.ModifiedOn)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Cycle>(entity =>
            {
                entity.ToTable("Cycle");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("int(1)");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document");

                entity.HasIndex(e => e.BatchId)
                    .HasName("FK_Document_Batch");

                entity.HasIndex(e => e.DocumentStatusId)
                    .HasName("FK_Document_DocumentStatus");

                entity.HasIndex(e => e.DocumentStepId)
                    .HasName("FK_Document_DocumentStep");

                entity.HasIndex(e => e.DocumentTypeId)
                    .HasName("FK_Document_DocumentType");

                entity.HasIndex(e => e.FileId)
                    .HasName("FK_Document_Files");

                entity.HasIndex(e => e.RejectionCodeId)
                    .HasName("FK_Document_RejectionCode");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BatchId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.CreatedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.CroppedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.CroppedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentStatusId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReasonOther)
                    .HasColumnType("text")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentStepId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentTypeId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.FileId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.FinalQualityBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.FinalQualityOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.ReviewedBy)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReviewedOn).HasDefaultValueSql("NULL");
                entity.Property(e => e.ExportOn).HasDefaultValueSql("NULL");


                entity.Property(e => e.RescanBy)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.Property(e => e.RescanOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.RejectedBy)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.Property(e => e.RejectedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.Pcname)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.Property(e => e.IndexedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IndexedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.LastUpdatedOn).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.PageOrder)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ExportStatus)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.QualityBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.QualityOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.ReferenceNumber)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.RejectionCodeId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.SanityBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LastStep)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.Property(e => e.SanityOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.ModifiedBy)
                   .HasMaxLength(50)
                   .IsUnicode(false)
                   .HasDefaultValueSql("NULL");

                entity.Property(e => e.ModifiedOn).HasDefaultValueSql("NULL");



                entity.HasOne(d => d.DocumentStatus)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.DocumentStatusId)
                    .HasConstraintName("FK_Document_DocumentStatus");

                entity.HasOne(d => d.DocumentStep)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.DocumentStepId)
                    .HasConstraintName("FK_Document_DocumentStep");

                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.DocumentTypeId)
                    .HasConstraintName("FK_Document_DocumentType");

                entity.HasOne(d => d.File)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.FileId)
                    .HasConstraintName("FK_Document_Files");

                entity.HasOne(d => d.RejectionCode)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.RejectionCodeId)
                    .HasConstraintName("FK_Document_Rejection");
            });

            modelBuilder.Entity<DocumentDetail>(entity =>
            {
                entity.ToTable("DocumentDetail");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Micr)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<DocumentHistory>(entity =>
            {
                entity.ToTable("DocumentHistory");

                entity.HasIndex(e => e.BatchId)
                    .HasName("FK_DocumentHistory_Batch");

                entity.HasIndex(e => e.DocumentId)
                    .HasName("FK_DocumentHistory_Document");

                entity.HasIndex(e => e.DocumentStatusId)
                    .HasName("FK_DocumentHistory_DocumentStatus");

                entity.HasIndex(e => e.RejectionCodeId)
                    .HasName("FK_DocumentHistory_RejectionCode");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.Amount)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.BatchId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Comment)
                    .HasColumnType("longtext")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentStatusId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentStepId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LastUpdatedOn).HasDefaultValueSql("current_timestamp()");

                entity.Property(e => e.Mirc)
                    .HasMaxLength(128)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.RejectionCodeId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.UserId);

                entity.Property(e => e.UserName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.WorkStationId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.DocumentHistory)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("FK_DocumentHistory_Document");

                entity.HasOne(d => d.DocumentStatus)
                    .WithMany(p => p.DocumentHistory)
                    .HasForeignKey(d => d.DocumentStatusId)
                    .HasConstraintName("FK_DocumentHistory_DocumentStatus");

                entity.HasOne(d => d.RejectionCode)
                    .WithMany(p => p.DocumentHistory)
                    .HasForeignKey(d => d.RejectionCodeId)
                    .HasConstraintName("FK_DocumentHistory_RejectionCode");
            });

            modelBuilder.Entity<DocumentIndex>(entity =>
            {
                entity.ToTable("DocumentIndex");

                entity.HasIndex(e => e.DocumentId)
                    .HasName("FK_DocumentIndex_Document");

                entity.HasIndex(e => e.Value)
                    .HasName("indexValue");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.DocumentId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IndexId)
                    .HasColumnType("bigint(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Status)
                    .HasColumnType("int(2)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Value)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.DocumentIndex)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("FK_DocumentIndex_Document");
            });

            modelBuilder.Entity<DocumentLog>(entity =>
            {
                entity.ToTable("DocumentLog");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.DeliveredBy)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Log)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ReceivedBy)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<DocumentStatus>(entity =>
            {
                entity.ToTable("DocumentStatus");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<DocumentStep>(entity =>
            {
                entity.ToTable("DocumentStep");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.ToTable("DocumentType");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<DocumentValidation>(entity =>
            {
                entity.ToTable("DocumentValidation");

                entity.HasIndex(e => e.MyId)
                    .HasName("MyId");

                entity.Property(e => e.Id)
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.CompletedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.MyId).HasColumnType("bigint(20)");

                entity.Property(e => e.PercentageToBeValidated)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.PercentageValidated)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.StartedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.TotalDocument).HasColumnType("int(11)");

                entity.Property(e => e.TotalToBeValidated)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.TotalValidated)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.TotalAccepted)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.TotalRejected)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.SuccessRate)
                    .HasColumnType("decimal(5,2)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ValidatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsLocked)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsCompleted)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsAccepted)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsRejected)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Status)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<DuplicateDocumentLink>(entity =>
            {
                entity.ToTable("DuplicateDocumentLink");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.DocId)
                    .HasColumnName("docId")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.LinkDocId)
                    .HasColumnName("linkDocId")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<FileType>(entity =>
            {
                entity.ToTable("FileType");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Files>(entity =>
            {
                entity.ToTable("Files");

                entity.HasIndex(e => e.BatchId)
                    .HasName("FK_Files_Batch");

                entity.HasIndex(e => e.FileTypeId)
                    .HasName("FK_Files_FileType");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BatchId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsFramework);

                entity.Property(e => e.FileTypeId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Lastupdate).HasDefaultValueSql("NULL");


                entity.HasOne(d => d.FileType)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.FileTypeId)
                    .HasConstraintName("FK_Files_FileTypeId");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.HasIndex(e => e.BatchId)
                    .HasName("FK_Image_Batch");

                entity.HasIndex(e => e.DocumentId)
                    .HasName("FK_Image_Document");

                entity.HasIndex(e => e.DocumentTypeId)
                    .HasName("FK_Image_DocumentType");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.BatchId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Data)
                    .HasColumnType("longblob")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentTypeId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ImageToBase64String)
                    .HasColumnType("longtext")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Path)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ScannedOrder)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Side)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Size)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("0");

                entity.HasOne(d => d.Batch)
                    .WithMany(p => p.Image)
                    .HasForeignKey(d => d.BatchId)
                    .HasConstraintName("image_ibfk_1");

                entity.HasOne(d => d.Document)
                    .WithMany(p => p.Image)
                    .HasForeignKey(d => d.DocumentId)
                    .HasConstraintName("image_ibfk_2");

                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.Image)
                    .HasForeignKey(d => d.DocumentTypeId)
                    .HasConstraintName("image_ibfk_3");
            });

            modelBuilder.Entity<Index>(entity =>
            {
                entity.ToTable("Index");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.DataTypeId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsUnique)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsRequired)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsExport)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.IsFileName)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<PoliceIndex>(entity =>
            {
                entity.ToTable("PoliceIndex");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();


                entity.Property(e => e.LicenseNumber)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Surname)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.OtherName)
                  .HasMaxLength(100)
                  .IsUnicode(false)
                  .HasDefaultValueSql("NULL");

                entity.Property(e => e.NIC)
                  .HasMaxLength(100)
                  .IsUnicode(false)
                  .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<IndexDataType>(entity =>
            {
                entity.ToTable("IndexDataType");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<RejectionCode>(entity =>
            {
                entity.ToTable("RejectionCode");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Status)
                    .HasColumnType("int(1)")
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<ResourceFile>(entity =>
            {
                entity.ToTable("ResourceFile");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.English)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Form)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.French)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("Session");

                entity.HasIndex(e => e.CycleId)
                    .HasName("FK_Session_Cycle");

                entity.HasIndex(e => e.SessionStateId)
                    .HasName("FK_Session_SessionState");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.ClosedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.ClosedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.CycleId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.OpenedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.OpenedOn).HasDefaultValueSql("NULL");

                entity.Property(e => e.SessionStateId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.HasOne(d => d.Cycle)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.CycleId)
                    .HasConstraintName("FK_Session_Cycle");

                entity.HasOne(d => d.SessionState)
                    .WithMany(p => p.Session)
                    .HasForeignKey(d => d.SessionStateId)
                    .HasConstraintName("FK_Session_SessionState");
            });

            modelBuilder.Entity<SessionState>(entity =>
            {
                entity.ToTable("SessionState");

                entity.Property(e => e.Id)
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("int(1)");
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("setting");

                entity.Property(e => e.Id)
                    .HasMaxLength(64)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.DocumentStepId)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("NULL");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasColumnType("int(1)");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<WorkStation>(entity =>
            {
                entity.ToTable("WorkStation");

                entity.Property(e => e.Id).HasColumnType("bigint(20)");

                entity.Property(e => e.MacAddress)
                    .IsRequired()
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Reference)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasDefaultValueSql("NULL");
            });

            modelBuilder.Entity<WorkFlow>(entity =>
            {
                entity.ToTable("WorkFlow");

                entity.Property(e => e.Id)
                   .HasMaxLength(64)
                   .IsUnicode(false)
                   .ValueGeneratedNever();

                entity.Property(e => e.FromStepId)
                  .HasColumnType("bigint(20)");

                entity.Property(e => e.ToStepId)
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Status).HasColumnType("int(1)");
            }); 
            
            modelBuilder.Entity<Trademarks>(entity =>
            {
                entity.ToTable("Trademarks");
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Project");
            });

            modelBuilder.Entity<Document_type>(entity =>
            {
                entity.ToTable("Document_type");
            });

            modelBuilder.Entity<ProjectFolio>(entity =>
            {
                entity.ToTable("Project_folio");
            });
        }
    }
}
