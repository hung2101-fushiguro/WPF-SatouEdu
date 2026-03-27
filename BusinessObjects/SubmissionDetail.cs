using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class SubmissionDetail
{
    public int SubmissionId { get; set; }

    public int QuestionId { get; set; }

    public string? SelectedOption { get; set; }

    public bool? IsCorrect { get; set; }

    public virtual QuestionBank Question { get; set; } = null!;

    public virtual ExamSubmission Submission { get; set; } = null!;
}
