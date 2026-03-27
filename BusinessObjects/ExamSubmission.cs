using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class ExamSubmission
{
    public int SubmissionId { get; set; }

    public int ExamId { get; set; }

    public int StudentId { get; set; }

    public DateTime? SubmitTime { get; set; }

    public int? TotalCorrect { get; set; }

    public decimal? Score { get; set; }

    public int? Status { get; set; }

    public virtual Exam Exam { get; set; } = null!;

    public virtual User Student { get; set; } = null!;

    public virtual ICollection<SubmissionDetail> SubmissionDetails { get; set; } = new List<SubmissionDetail>();
}
