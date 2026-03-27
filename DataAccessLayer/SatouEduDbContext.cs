using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace DataAccessLayer;

    public partial class SatouEduDbContext : DbContext
{
    public SatouEduDbContext()
    {
    }

    public SatouEduDbContext(DbContextOptions<SatouEduDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassEnrollment> ClassEnrollments { get; set; }

    public virtual DbSet<Exam> Exams { get; set; }

    public virtual DbSet<ExamQuestion> ExamQuestions { get; set; }

    public virtual DbSet<ExamSubmission> ExamSubmissions { get; set; }

    public virtual DbSet<QuestionBank> QuestionBanks { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubmissionDetail> SubmissionDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }
    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true).Build();
        return config["ConnectionStrings:SatouEduDB"];
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.ClassId).HasName("PK__Class__CB1927C02E5CB6F2");

            entity.ToTable("Class");

            entity.Property(e => e.ClassName).HasMaxLength(100);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.TargetClassName).HasMaxLength(50);

            entity.HasOne(d => d.Subject).WithMany(p => p.Classes)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Class__SubjectId__3F466844");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classes)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Class__TeacherId__3E52440B");
        });

        modelBuilder.Entity<ClassEnrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__ClassEnr__7F68771BEE1DAEA1");

            entity.ToTable("ClassEnrollment");

            entity.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Class).WithMany(p => p.ClassEnrollments)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassEnro__Class__4316F928");

            entity.HasOne(d => d.Student).WithMany(p => p.ClassEnrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClassEnro__Stude__440B1D61");
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.ExamId).HasName("PK__Exam__297521C7B0D29662");

            entity.ToTable("Exam");

            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.ExamName).HasMaxLength(200);
            entity.Property(e => e.ExamType).HasDefaultValue(1);
            entity.Property(e => e.IsScoreVisible).HasDefaultValue(false);
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasDefaultValue(1);

            entity.HasOne(d => d.Class).WithMany(p => p.Exams)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Exam__ClassId__48CFD27E");
        });

        modelBuilder.Entity<ExamQuestion>(entity =>
        {
            entity.HasKey(e => new { e.ExamId, e.QuestionId }).HasName("PK__ExamQues__F9A9273D67B7EE9E");

            entity.ToTable("ExamQuestion");

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamQuest__ExamI__52593CB8");

            entity.HasOne(d => d.Question).WithMany(p => p.ExamQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamQuest__Quest__534D60F1");
        });

        modelBuilder.Entity<ExamSubmission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId).HasName("PK__ExamSubm__449EE12576EAAAC8");

            entity.ToTable("ExamSubmission");

            entity.Property(e => e.Score)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(4, 2)");
            entity.Property(e => e.Status).HasDefaultValue(1);
            entity.Property(e => e.SubmitTime).HasColumnType("datetime");
            entity.Property(e => e.TotalCorrect).HasDefaultValue(0);

            entity.HasOne(d => d.Exam).WithMany(p => p.ExamSubmissions)
                .HasForeignKey(d => d.ExamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamSubmi__ExamI__5629CD9C");

            entity.HasOne(d => d.Student).WithMany(p => p.ExamSubmissions)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ExamSubmi__Stude__571DF1D5");
        });

        modelBuilder.Entity<QuestionBank>(entity =>
        {
            entity.HasKey(e => e.QuestionId).HasName("PK__Question__0DC06FAC17D686E2");

            entity.ToTable("QuestionBank");

            entity.Property(e => e.CorrectOption)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.OptionA).HasMaxLength(500);
            entity.Property(e => e.OptionB).HasMaxLength(500);
            entity.Property(e => e.OptionC).HasMaxLength(500);
            entity.Property(e => e.OptionD).HasMaxLength(500);

            entity.HasOne(d => d.Subject).WithMany(p => p.QuestionBanks)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuestionB__Subje__4F7CD00D");

            entity.HasOne(d => d.Teacher).WithMany(p => p.QuestionBanks)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__QuestionB__Teach__4E88ABD4");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK__Subject__AC1BA3A8DAA68528");

            entity.ToTable("Subject");

            entity.HasIndex(e => e.SubjectName, "UQ__Subject__4C5A7D554BBDE0CA").IsUnique();

            entity.Property(e => e.SubjectName).HasMaxLength(100);
        });

        modelBuilder.Entity<SubmissionDetail>(entity =>
        {
            entity.HasKey(e => new { e.SubmissionId, e.QuestionId }).HasName("PK__Submissi__9442E7DF745C292A");

            entity.ToTable("SubmissionDetail");

            entity.Property(e => e.SelectedOption)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.Question).WithMany(p => p.SubmissionDetails)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__Quest__5DCAEF64");

            entity.HasOne(d => d.Submission).WithMany(p => p.SubmissionDetails)
                .HasForeignKey(d => d.SubmissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Submissio__Submi__5CD6CB2B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CC4C29C6563B");

            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ__User__A9D105341A1659D4").IsUnique();

            entity.Property(e => e.ClassName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Status).HasDefaultValue(1);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
