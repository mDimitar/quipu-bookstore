INSERT INTO Authors.Author (Name) VALUES
    ('J.R.R. Tolkien'),
    ('Isaac Asimov'),
    ('Agatha Christie');

INSERT INTO Books.Book (AuthorId, Title, SubTitle) VALUES
    (1, 'The Fellowship of the Ring', 'The Lord of the Rings, Part 1'),
    (1, 'The Two Towers', 'The Lord of the Rings, Part 2'),
    (2, 'Foundation', NULL),
    (2, 'I, Robot', NULL),
    (3, 'Murder on the Orient Express', NULL);
