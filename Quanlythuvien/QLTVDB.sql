--drop if exít
DROP DATABASE IF  EXISTS QuanlythuvienDB;

-- Create the LibraryDB database
CREATE DATABASE QuanlythuvienDB;
GO

-- Use the LibraryDB database
USE QuanlythuvienDB;
GO

-- Create Publishers table for normalization
CREATE TABLE Publishers (
    publisherId INT PRIMARY KEY IDENTITY(1,1),
    publisherName NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- Create Admins table
CREATE TABLE Admins (
    adminId INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(50) NOT NULL UNIQUE,
    passwordHash VARCHAR(255) NOT NULL,
    fullname NVARCHAR(100),
    email VARCHAR(50) UNIQUE CHECK (email LIKE '%@%.%'),
    createdAt DATETIME DEFAULT GETDATE()
);
GO

-- Create Librarians table
CREATE TABLE Librarians (
    libraId INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(50) NOT NULL UNIQUE,
    passwordHash VARCHAR(255) NOT NULL,
    fullname NVARCHAR(100),
    email VARCHAR(50) UNIQUE CHECK (email LIKE '%@%.%'),
    hireDate DATE,
    status BIT DEFAULT 1
);
GO

-- Create Students table (Removed enrollmentDate, dateOfBirth, createdAt)
CREATE TABLE Students (
    studentId INT PRIMARY KEY IDENTITY(1,1),
    username VARCHAR(50) NOT NULL UNIQUE,
    passwordHash VARCHAR(255) NOT NULL,
    fullname NVARCHAR(100),
    email VARCHAR(50) UNIQUE CHECK (email LIKE '%@%.%'),
    phone VARCHAR(10) CHECK (LEN(phone) = 10 AND phone NOT LIKE '%[^0-9]%'),
    address NVARCHAR(100),
    status BIT DEFAULT 1
);
GO

-- Create Categories table
CREATE TABLE Categories (
    cateId INT PRIMARY KEY IDENTITY(1,1),
    cateName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Create Authors table
CREATE TABLE Authors (
    authorId INT PRIMARY KEY IDENTITY(1,1),
    authorName NVARCHAR(100) NOT NULL UNIQUE,
    bio NVARCHAR(300)
);
GO

-- Create Books table (Removed isbn, isDigital)
CREATE TABLE Books (
    bookId INT PRIMARY KEY IDENTITY(1,1),
    title NVARCHAR(100) NOT NULL,
    publisherId INT NULL,
    yearPublished INT CHECK (yearPublished >= 1800 AND yearPublished <= YEAR(GETDATE())),
    quantity INT CHECK (quantity >= 0),
    imagePath NVARCHAR(255),
    description NVARCHAR(500),
    location NVARCHAR(50),
    downloadLink NVARCHAR(255) NULL,
    status BIT DEFAULT 1,
    CONSTRAINT FK_Book_Publisher FOREIGN KEY (publisherId) REFERENCES Publishers(publisherId) ON DELETE SET NULL
);
GO

-- Create BookCategories table (For many-to-many relationship between Books and Categories)
CREATE TABLE BookCategories (
    bookId INT NOT NULL,
    cateId INT NOT NULL,
    PRIMARY KEY (bookId, cateId),
    CONSTRAINT FK_BookCategories_Books FOREIGN KEY (bookId) REFERENCES Books(bookId) ON DELETE CASCADE,
    CONSTRAINT FK_BookCategories_Categories FOREIGN KEY (cateId) REFERENCES Categories(cateId) ON DELETE CASCADE
);
GO

-- Create BookAuthors table (For many-to-many relationship between Books and Authors)
CREATE TABLE BookAuthors (
    bookId INT NOT NULL,
    authorId INT NOT NULL,
    PRIMARY KEY (bookId, authorId),
    CONSTRAINT FK_BookAuthors_Books FOREIGN KEY (bookId) REFERENCES Books(bookId) ON DELETE CASCADE,
    CONSTRAINT FK_BookAuthors_Authors FOREIGN KEY (authorId) REFERENCES Authors(authorId) ON DELETE CASCADE
);
GO

-- Create Borrowed table (Added bookStatus)
CREATE TABLE Borrowed (
    borrowId INT PRIMARY KEY IDENTITY(1,1),
    studentId INT NULL,
    bookId INT NULL,
    borrowDate DATE DEFAULT GETDATE(),
    dueDate DATE,
    returnDate DATE,
    libraId INT NULL,
    fineAmount DECIMAL(10,2) DEFAULT 0,
    bookStatus NVARCHAR(50),
    status BIT DEFAULT 1,
    CONSTRAINT FK_Borrowed_Students FOREIGN KEY (studentId) REFERENCES Students(studentId) ON DELETE SET NULL,
    CONSTRAINT FK_Borrowed_Books FOREIGN KEY (bookId) REFERENCES Books(bookId) ON DELETE SET NULL,
    CONSTRAINT FK_Borrowed_Librarians FOREIGN KEY (libraId) REFERENCES Librarians(libraId) ON DELETE SET NULL,
    CONSTRAINT CHK_DueDate CHECK (dueDate >= borrowDate)
);
GO

-- Create Fines table
CREATE TABLE Fines (
    fineId INT PRIMARY KEY IDENTITY(1,1),
    borrowId INT NULL,
    amount DECIMAL(10,2) NOT NULL CHECK (amount > 0),
    paid BIT DEFAULT 0,
    paidDate DATE NULL,
    createdAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Fines_Borrowed FOREIGN KEY (borrowId) REFERENCES Borrowed(borrowId) ON DELETE SET NULL
);
GO

-- Create Roles table
CREATE TABLE Roles (
    roleId INT PRIMARY KEY IDENTITY(1,1),
    roleName NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Create UserRoles table
CREATE TABLE UserRoles (
    userRoleId INT PRIMARY KEY IDENTITY(1,1),
    roleId INT NOT NULL,
    adminId INT NULL,
    libraId INT NULL,
    studentId INT NULL,
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (roleId) REFERENCES Roles(roleId) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Admins FOREIGN KEY (adminId) REFERENCES Admins(adminId) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Librarians FOREIGN KEY (libraId) REFERENCES Librarians(libraId) ON DELETE CASCADE,
    CONSTRAINT FK_UserRoles_Students FOREIGN KEY (studentId) REFERENCES Students(studentId) ON DELETE CASCADE,
    CONSTRAINT CHK_OneUserType CHECK (
        (adminId IS NOT NULL AND libraId IS NULL AND studentId IS NULL) OR
        (adminId IS NULL AND libraId IS NOT NULL AND studentId IS NULL) OR
        (adminId IS NULL AND libraId IS NULL AND studentId IS NOT NULL)
    )
);
GO

-- Create indexes for performance
CREATE INDEX IX_Books_Title ON Books(title);
CREATE INDEX IX_Borrowed_StudentId ON Borrowed(studentId);
CREATE INDEX IX_Borrowed_BookId ON Borrowed(bookId);
CREATE INDEX IX_Students_Email ON Students(email);
CREATE INDEX IX_BookCategories_BookId ON BookCategories(bookId);
CREATE INDEX IX_BookCategories_CateId ON BookCategories(cateId);
CREATE INDEX IX_BookAuthors_BookId ON BookAuthors(bookId);
CREATE INDEX IX_BookAuthors_AuthorId ON BookAuthors(authorId);
GO

-- Trigger to update book quantity on borrow
CREATE TRIGGER TRG_Borrowed_Insert
ON Borrowed
AFTER INSERT
AS
BEGIN
    UPDATE Books
    SET quantity = quantity - 1
    FROM Books b
    INNER JOIN inserted i ON b.bookId = i.bookId
    WHERE b.quantity > 0 AND i.returnDate IS NULL;
END;
GO

-- Trigger to update book quantity on return
CREATE TRIGGER TRG_Borrowed_Update
ON Borrowed
AFTER UPDATE
AS
BEGIN
    UPDATE Books
    SET quantity = quantity + 1
    FROM Books b
    INNER JOIN inserted i ON b.bookId = i.bookId
    INNER JOIN deleted d ON i.borrowId = d.borrowId
    WHERE i.returnDate IS NOT NULL AND d.returnDate IS NULL;
END;
GO

-- Trigger for calculating fine on overdue return
CREATE TRIGGER TRG_Borrowed_CalculateFine
ON Borrowed
AFTER UPDATE
AS
BEGIN
    DECLARE @borrowId INT, @returnDate DATE, @dueDate DATE, @daysOverdue INT, @fineAmount DECIMAL(10,2);

    SELECT @borrowId = i.borrowId, @returnDate = i.returnDate, @dueDate = i.dueDate
    FROM inserted i
    WHERE i.returnDate IS NOT NULL;

    IF @returnDate > @dueDate
    BEGIN
        SET @daysOverdue = DATEDIFF(DAY, @dueDate, @returnDate);
        SET @fineAmount = @daysOverdue * 1.00; -- Example: $1 per day

        UPDATE Borrowed
        SET fineAmount = @fineAmount
        WHERE borrowId = @borrowId;

        INSERT INTO Fines (borrowId, amount)
        VALUES (@borrowId, @fineAmount);
    END;
END;
GO

-- Insert sample data
INSERT INTO Publishers (publisherName)
VALUES 
(N'NXB Trẻ'),
(N'NXB Tổng hợp TP.HCM'),
(N'NXB Giáo dục'),
(N'NXB Thế Giới'),
(N'NXB Văn học');
GO

INSERT INTO Admins (username, passwordHash, fullname, email)
VALUES ('admin1', '$2a$10$hashedpasswordexample123', N'Nguyễn Văn A', 'admin1@qltv.com');
GO

INSERT INTO Librarians (username, passwordHash, fullname, email, hireDate, status)
VALUES 
('lib1', '$2a$10$hashedpasswordexample123', N'Nguyễn Thị B', 'lib1@qltv.com', '2022-01-01', 1),
('lib2', '$2a$10$hashedpasswordexample123', N'Trần Thanh T', 'lib2@qltv.com', '2023-05-10', 1);
GO

INSERT INTO Students (username, passwordHash, fullname, email, phone, address, status)
VALUES 
('sv01', '$2a$10$hashedpasswordexample123', N'Nguyễn Văn Nam', 'nguyenvannam@qltv.com', '0912345681', N'Hà Nội', 1),
('sv02', '$2a$10$hashedpasswordexample123', N'Trần Thị Lan', 'sv05@qltv.com', '0901234568', N'Huế', 1),
('sv03', '$2a$10$hashedpasswordexample123', N'Lê Thị Minh', 'lethiminh@qltv.com', '0931234568', N'Đà Nẵng', 1),
('sv04', '$2a$10$hashedpasswordexample123', N'Phạm Văn Hùng', 'phamvanhung@qltv.com', '0941234568', N'Hải Phòng', 1),
('sv06', '$2a$10$hashedpasswordexample123', N'Trần Văn Minh', 'tranvanminh@qltv.com', '0921234568', N'Quảng Nam', 1);
GO

INSERT INTO Categories (cateName)
VALUES 
(N'Thiếu nhi'),
(N'Kỹ năng'),
(N'Văn học'),
(N'Giáo trình'),
(N'Khoa học'),
(N'Tình cảm');
GO

INSERT INTO Authors (authorName, bio)
VALUES 
(N'Nguyễn Nhật Ánh', N'Tác giả nổi tiếng với nhiều tác phẩm thiếu nhi'),
(N'J.K. Rowling', N'Tác giả Harry Potter'),
(N'Dale Carnegie', N'Chuyên gia nổi tiếng về nghệ thuật giao tiếp và phát triển bản thân'),
(N'Nguyễn Đình Trí', N'Tác giả nhiều giáo trình Toán cao cấp'),
(N'Stephen Hawking', N'Nhà vật lý lý thuyết, tác giả các sách khoa học nổi tiếng'),
(N'Nguyễn Du', N'Đại thi hào dân tộc Việt Nam, tác giả Truyện Kiều'),
(N'Trần Văn Hùng', N'Giáo sư toán học, đồng tác giả giáo trình');
GO

INSERT INTO Roles (roleName)
VALUES (N'Admin'), (N'Librarian'), (N'Student');
GO

INSERT INTO Books (title, publisherId, yearPublished, quantity, imagePath, description, location, downloadLink, status)
VALUES 
(N'Mắt biếc', 1, 1990, 8, '/images/books/matbiec.jpg', N'Tiểu thuyết nổi tiếng của Nguyễn Nhật Ánh', 'Shelf A1', NULL, 1),
(N'Harry Potter và Hòn đá phù thủy', 1, 1997, 10, '/images/books/harrypotter.jfif', N'Truyện thiếu nhi kỳ ảo', 'Shelf B2', NULL, 1),
(N'Đắc nhân tâm', 2, 1936, 7, '/images/books/dacnhantam.jpg', N'Sách kỹ năng sống kinh điển', 'Shelf C3', NULL, 1),
(N'Toán cao cấp tập 1', 3, 2005, 15, '/images/books/toancaocap.jfif', N'Giáo trình cho sinh viên đại học', 'Shelf D4', NULL, 1),
(N'Vũ trụ trong vỏ hạt dẻ', 4, 2001, 8, '/images/books/vutru.jpg', N'Sách khoa học của Stephen Hawking', 'Shelf E5', NULL, 1),
(N'Truyện Kiều', 5, 1820, 12, '/images/books/truyenkieu.jpg', N'Tác phẩm kinh điển của Nguyễn Du', 'Shelf F6', NULL, 1),
(N'Sách kỹ thuật số mẫu', 1, 2025, 100, '/images/digital.jpg', N'Sách mẫu', NULL, 'https://example.com/download.pdf', 1);
GO

-- Insert into BookCategories (many-to-many)
INSERT INTO BookCategories (bookId, cateId)
VALUES 
(1, 3), -- Mắt biếc: Văn học
(1, 6), -- Mắt biếc: Tình cảm
(2, 1), -- Harry Potter: Thiếu nhi
(3, 2), -- Đắc nhân tâm: Kỹ năng
(4, 4), -- Toán cao cấp: Giáo trình
(5, 5), -- Vũ trụ: Khoa học
(6, 3), -- Truyện Kiều: Văn học
(6, 6); -- Truyện Kiều: Tình cảm
GO

-- Insert into BookAuthors (many-to-many)
INSERT INTO BookAuthors (bookId, authorId)
VALUES 
(1, 1), -- Mắt biếc: Nguyễn Nhật Ánh
(2, 2), -- Harry Potter: J.K. Rowling
(3, 3), -- Đắc nhân tâm: Dale Carnegie
(4, 4), -- Toán cao cấp: Nguyễn Đình Trí
(4, 7), -- Toán cao cấp: Trần Văn Hùng (đồng tác giả)
(5, 5), -- Vũ trụ: Stephen Hawking
(6, 6); -- Truyện Kiều: Nguyễn Du
GO

INSERT INTO Borrowed (studentId, bookId, borrowDate, dueDate, returnDate, libraId, bookStatus, status)
VALUES 
(1, 1, '2024-08-01', '2024-08-15', NULL, 1, N'Good', 1),
(2, 2, '2024-08-05', '2024-08-20', NULL, 2, N'Good', 1),
(3, 3, '2024-09-01', '2024-09-15', NULL, 1, N'Slightly Damaged', 1);
GO

INSERT INTO Fines (borrowId, amount)
VALUES (1, 5.00);
GO

INSERT INTO UserRoles (roleId, adminId) VALUES (1, 1);
INSERT INTO UserRoles (roleId, libraId) VALUES (2, 1), (2, 2);
INSERT INTO UserRoles (roleId, studentId) VALUES (3, 1), (3, 2), (3, 3), (3, 4), (3, 5);
GO