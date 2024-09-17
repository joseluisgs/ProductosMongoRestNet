using Microsoft.AspNetCore.Mvc;
using ProductosMongoRestNet.Models;
using ProductosMongoRestNet.Services;

namespace ProductosMongoRestNet.Controllers;

[ApiController]
// [Route("api/[controller]")]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBooksService _booksService;

    public BooksController(IBooksService booksService)
    {
        _booksService = booksService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Book>>> GetAll()
    {
        var books = await _booksService.GetAllAsync();
        return Ok(books);
    }

    [HttpGet("{id:length(24)}")] // Para que el id tenga 24 caracteres (ObjectId)
    public async Task<ActionResult<Book>> GetById(string id)
    {
        var book = await _booksService.GetByIdAsync(id);

        if (book is null) return NotFound();

        return book;
    }
}