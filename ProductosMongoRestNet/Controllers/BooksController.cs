using Microsoft.AspNetCore.Mvc;
using ProductosMongoRestNet.Models;
using ProductosMongoRestNet.Services;

namespace ProductosMongoRestNet.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Route("api/books")]
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

    [HttpPost]
    public async Task<ActionResult<Book>> Create(Book book)
    {
        var savedBook = await _booksService.CreateAsync(book);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, savedBook);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<ActionResult> Update(
        string id,
        [FromBody] Book book)
    {
        var updatedBook = await _booksService.UpdateAsync(id, book);

        if (updatedBook is null) return NotFound();

        return Ok(updatedBook);
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<ActionResult> Delete(string id)
    {
        var deletedBook = await _booksService.DeleteAsync(id);

        if (deletedBook is null) return NotFound();

        return NoContent();
    }
}