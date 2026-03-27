using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class ClassEnrollment
{
    public int EnrollmentId { get; set; }

    public int ClassId { get; set; }

    public int StudentId { get; set; }

    public DateTime? EnrollmentDate { get; set; }

    public int? Status { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual User Student { get; set; } = null!;
}
