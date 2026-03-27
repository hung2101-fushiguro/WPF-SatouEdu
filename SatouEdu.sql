CREATE DATABASE SatouEduDB;
GO
USE SatouEduDB;
GO

-- 0. Bảng Môn học (Mới)
CREATE TABLE [Subject] (
    SubjectId INT IDENTITY(1,1) PRIMARY KEY,
    SubjectName NVARCHAR(100) NOT NULL UNIQUE
);

-- 1. Bảng Người dùng
CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    [Password] NVARCHAR(100) NOT NULL,
    DateOfBirth DATE,
    ClassName NVARCHAR(50), -- Chỉ dùng cho học sinh (VD: 10A1)
    [Role] INT NOT NULL,    -- 0: Admin, 1: Giáo viên, 2: Học sinh
    [Status] INT DEFAULT 1  -- 1: Active, 2: Inactive
);

-- 2. Bảng Lớp học (Tên lớp + Môn học)
CREATE TABLE Class (
    ClassId INT IDENTITY(1,1) PRIMARY KEY,
    ClassName NVARCHAR(100) NOT NULL, -- VD: "Toán 10A1"
    TargetClassName NVARCHAR(50), -- VD: "10A1" (Để kiểm tra điều kiện học sinh xin vào)
    TeacherId INT NOT NULL FOREIGN KEY REFERENCES [User](UserId),
    SubjectId INT NOT NULL FOREIGN KEY REFERENCES [Subject](SubjectId),
    Description NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- 3. Bảng Tham gia lớp học
CREATE TABLE ClassEnrollment (
    EnrollmentId INT IDENTITY(1,1) PRIMARY KEY,
    ClassId INT NOT NULL FOREIGN KEY REFERENCES Class(ClassId),
    StudentId INT NOT NULL FOREIGN KEY REFERENCES [User](UserId),
    EnrollmentDate DATETIME DEFAULT GETDATE(),
    [Status] INT DEFAULT 1 -- 1: Chờ duyệt, 2: Đã duyệt, 3: Từ chối
);

-- 4. Bảng Bài kiểm tra / Bài tập
CREATE TABLE Exam (
    ExamId INT IDENTITY(1,1) PRIMARY KEY,
    ClassId INT NOT NULL FOREIGN KEY REFERENCES Class(ClassId),
    ExamName NVARCHAR(200) NOT NULL,
    ExamType INT DEFAULT 1,        -- 1: Bài tập, 2: Bài kiểm tra trên lớp
    StartTime DATETIME NOT NULL,   
    EndTime DATETIME NOT NULL,     
    DurationMinutes INT NOT NULL,  -- VD: 45, 60
    IsScoreVisible BIT DEFAULT 0,  -- 0: Ẩn điểm, 1: Hiện điểm cho học sinh
    [Status] INT DEFAULT 1         -- 1: Nháp, 2: Đã xuất bản
);

-- 5. Ngân hàng Câu hỏi (Phân loại theo môn học)
CREATE TABLE QuestionBank (
    QuestionId INT IDENTITY(1,1) PRIMARY KEY,
    TeacherId INT NOT NULL FOREIGN KEY REFERENCES [User](UserId),
    SubjectId INT NOT NULL FOREIGN KEY REFERENCES [Subject](SubjectId),
    GradeLevel INT NOT NULL, -- CỘT MỚI: Phân biệt lớp 9, 10, 11, 12...
    Content NVARCHAR(MAX) NOT NULL,
    OptionA NVARCHAR(500) NOT NULL,
    OptionB NVARCHAR(500) NOT NULL,
    OptionC NVARCHAR(500) NOT NULL,
    OptionD NVARCHAR(500) NOT NULL,
    CorrectOption CHAR(1) NOT NULL 
);

-- 6. Chi tiết Đề thi
CREATE TABLE ExamQuestion (
    ExamId INT NOT NULL FOREIGN KEY REFERENCES Exam(ExamId),
    QuestionId INT NOT NULL FOREIGN KEY REFERENCES QuestionBank(QuestionId),
    OrderIndex INT NOT NULL, 
    PRIMARY KEY (ExamId, QuestionId)
);

-- 7. Bảng Kết quả
CREATE TABLE ExamSubmission (
    SubmissionId INT IDENTITY(1,1) PRIMARY KEY,
    ExamId INT NOT NULL FOREIGN KEY REFERENCES Exam(ExamId),
    StudentId INT NOT NULL FOREIGN KEY REFERENCES [User](UserId),
    SubmitTime DATETIME,
    TotalCorrect INT DEFAULT 0,  
    Score DECIMAL(4,2) DEFAULT 0.00, 
    [Status] INT DEFAULT 1 -- 1: Chưa làm, 2: Đang làm, 3: Đã nộp
);

-- 8. Chi tiết từng đáp án (Cần thiết để GV thống kê câu sai)
CREATE TABLE SubmissionDetail (
    SubmissionId INT NOT NULL FOREIGN KEY REFERENCES ExamSubmission(SubmissionId),
    QuestionId INT NOT NULL FOREIGN KEY REFERENCES QuestionBank(QuestionId),
    SelectedOption CHAR(1), 
    IsCorrect BIT,          
    PRIMARY KEY (SubmissionId, QuestionId)
);
