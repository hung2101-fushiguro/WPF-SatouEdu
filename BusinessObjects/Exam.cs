using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class Exam
{
    public int ExamId { get; set; }

    public int ClassId { get; set; }

    public string ExamName { get; set; } = null!;

    public int? ExamType { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int DurationMinutes { get; set; }

    public bool? IsScoreVisible { get; set; }

    public int? Status { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    public virtual ICollection<ExamSubmission> ExamSubmissions { get; set; } = new List<ExamSubmission>();
}
