using ProductosMongoRestNet.Models;

namespace ProductosMongoRestNet.Mappers;

public static class BookMapper
{
    public static Book ToBook(this BookDocument bookDocument)
    {
        return new Book
        {
            Id = bookDocument.Id,
            BookName = bookDocument.BookName,
            Price = bookDocument.Price,
            Category = bookDocument.Category,
            Author = bookDocument.Author
        };
    }

    public static BookDocument ToBookDocument(this Book book)
    {
        return new BookDocument
        {
            Id = book.Id,
            BookName = book.BookName,
            Price = book.Price,
            Category = book.Category,
            Author = book.Author
        };
    }
}