using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProductosMongoRestNet.Database;
using ProductosMongoRestNet.Mappers;
using ProductosMongoRestNet.Models;

namespace ProductosMongoRestNet.Services;

public class BooksService : IBooksService
{
    private const string CacheKeyPrefix = "Book_"; //Para evitar colisiones en la caché de memoria con otros elementos
    private readonly IMongoCollection<BookDocument> _booksCollection;
    private readonly ILogger _logger;
    private readonly IMemoryCache _memoryCache;

    public BooksService(IOptions<BookStoreMongoConfig> bookStoreDatabaseSettings, ILogger<BooksService> logger,
        IMemoryCache memoryCache)
    {
        _logger = logger;
        _memoryCache = memoryCache;
        var mongoClient = new MongoClient(bookStoreDatabaseSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(bookStoreDatabaseSettings.Value.DatabaseName);
        _booksCollection =
            mongoDatabase.GetCollection<BookDocument>(bookStoreDatabaseSettings.Value.BooksCollectionName);
    }

    public async Task<List<Book>> GetAllAsync()
    {
        _logger.LogInformation("Getting all books from cache");
        // return (await _booksCollection.Find(_ => true).ToListAsync()).ConvertAll(bookDocument => bookDocument.ToBook());
        // Todo esto es porque no se puede hacer mapeo directamente con el método ToListAsync
        var bookDocuments = await _booksCollection.Find(_ => true).ToListAsync();
        return bookDocuments.ConvertAll(bookDocument =>
            bookDocument.ToBook()); // Mejor que usar Select porque es una lista
    }

    public async Task<Book?> GetByIdAsync(string id)
    {
        _logger.LogInformation($"Getting book with id: {id}");
        var cacheKey = CacheKeyPrefix + id;

        // Primero intentamos obtener el libro de la caché
        if (_memoryCache.TryGetValue(cacheKey, out Book? cachedBook))
        {
            _logger.LogInformation("Getting book from cache");
            return cachedBook;
        }

        // Si no está en la caché, lo obtenemos de la base de datos
        var bookDocument = await _booksCollection.Find(bookDocument => bookDocument.Id == id).FirstOrDefaultAsync();
        var book = bookDocument?.ToBook();

        // Si el libro está en la base de datos, lo guardamos en la caché
        if (book != null)
        {
            _memoryCache.Set(cacheKey, book,
                TimeSpan.FromMinutes(30)); // Ajusta el tiempo de caché según tus necesidades
            _logger.LogInformation("Caching the book");
        }

        // Devolvemos el libro
        return book;
    }

    public async Task<Book> CreateAsync(Book book)
    {
        _logger.LogInformation("Creating a new book");
        var bookDocument = book.ToBookDocument();

        // Inserta el documento en la base de datos
        await _booksCollection.InsertOneAsync(bookDocument);

        // Convierte el documento a la entidad Book
        return bookDocument.ToBook();

    }

    public async Task<Book?> UpdateAsync(string id, Book book)
    {
        _logger.LogInformation($"Updating book with id: {id}");
        var bookDocument = book.ToBookDocument();

        // Realiza el reemplazo y devuelve el documento actualizado
        var updatedDocument = await _booksCollection.FindOneAndReplaceAsync(
            document => document.Id == id,
            bookDocument
        );


        var updatedBook = updatedDocument?.ToBook();

        // Gestionamos la caché
        var cacheKey = CacheKeyPrefix + id;

        // Eliminamos el libro de la caché si se ha actualizado
        if (updatedBook != null)
        {
            _memoryCache.Remove(cacheKey); // Invalida la clave de la caché
            _logger.LogInformation($"Removed cached book with id: {id}");
        }

        return updatedBook;
    }

    public async Task<Book?> DeleteAsync(string id)
    {
        _logger.LogInformation($"Deleting book with id: {id}");

        // Elimina el documento de la base de datos y devuelve el documento eliminado
        var deletedDocument = await _booksCollection.FindOneAndDeleteAsync(bookDocument => bookDocument.Id == id);
    
        var deletedBook = deletedDocument?.ToBook();

        // Genera la clave de caché
        var cacheKey = CacheKeyPrefix + id;

        // Elimina el libro de la caché si existe
        if (deletedBook != null)
        {
            _memoryCache.Remove(cacheKey);
            _logger.LogInformation($"Removed cached book with id: {id}");
        }

        return deletedBook;
    }
}