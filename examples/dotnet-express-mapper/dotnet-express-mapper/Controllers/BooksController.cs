using dotnet_express_mapper.Services;
using dotnet_express_mapper.Models;
using ExpressMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnet_express_mapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        // GET api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookViewModel>>> Get()
        {
            IEnumerable<Book> books = await _bookService.GetBooks();

            // express mapper
            IEnumerable<BookViewModel> bookViewModels = Mapper.Map<IEnumerable<Book>, IEnumerable<BookViewModel>>(books);

            return new ObjectResult(bookViewModels);
        }

        // GET api/Books/1
        [HttpGet("{id}")]
        public async Task<ActionResult<BookViewModel>> Get(int id)
        {
            Book book = await _bookService.GetBook(id);

            if (book == null)
                return new NotFoundResult();

            // express mapper
            BookViewModel bookViewModel = Mapper.Map<Book, BookViewModel>(book);

            return new OkObjectResult(bookViewModel);
        }

        // POST api/Books
        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody] BookViewModel bookViewModel)
        {
            // express mapper
            Book book = Mapper.Map<BookViewModel, Book>(bookViewModel);
            await _bookService.Create(book);
            return new OkObjectResult(book);
        }

        // PUT api/Books/1
        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(int id, [FromBody] BookViewModel bookViewModel)
        {
            var bookFromDb = await _bookService.GetBook(id);

            if (bookFromDb == null)
                return new NotFoundResult();

            // express mapper
            Book book = Mapper.Map<BookViewModel, Book>(bookViewModel);
            book.Id = bookFromDb.Id;

            await _bookService.Update(book);

            return new OkObjectResult(book);
        }
    }
}
