CREATE SCHEMA Authors;
GO
CREATE SCHEMA Books;
GO

CREATE TABLE Authors.Author (
    AuthorId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Author PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    CONSTRAINT CK_Author_Name_Length CHECK (LEN(Name) >= 3)
);

CREATE TABLE Books.Book (
    BookId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Book PRIMARY KEY,
    AuthorId INT NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    SubTitle NVARCHAR(200) NULL,
    CONSTRAINT FK_Book_Author FOREIGN KEY (AuthorId) REFERENCES Authors.Author(AuthorId),
    CONSTRAINT CK_Book_Title_Length CHECK (LEN(Title) >= 3)
);

CREATE INDEX IX_Book_AuthorId ON Books.Book(AuthorId);
CREATE INDEX IX_Book_Title ON Books.Book(Title);
CREATE INDEX IX_Author_Name ON Authors.Author(Name);
