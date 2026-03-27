using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class QuestionBank
{
    public int QuestionId { get; set; }

    public int TeacherId { get; set; }

    public int SubjectId { get; set; }
    public int GradeLevel { get; set; }

    public string Content { get; set; } = null!;

    public string OptionA { get; set; } = null!;

    public string OptionB { get; set; } = null!;

    public string OptionC { get; set; } = null!;

    public string OptionD { get; set; } = null!;

    public string CorrectOption { get; set; } = null!;

    public virtual ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();

    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<SubmissionDetail> SubmissionDetails { get; set; } = new List<SubmissionDetail>();

    public virtual User Teacher { get; set; } = null!;
}
