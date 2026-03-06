using IndyBooks.Services;
using IndyBooks.Models;
using IndyBooks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace IndyBooks.Controllers;

    public class AdminController : Controller
    {
        private IndyBooksDataContext _db;
        private Repository _repo;
        public AdminController(IndyBooksDataContext db, Repository repo) 
        { 
            _db = db; 
            _repo = repo;
        }
        
        /***
         * DELETE
         */
        [HttpGet]
        public IActionResult RemoveBook(long id)
        {
            //TODO: Remove the Book associated with the given id number; Save Changes
            Book query = new Book { Id = id };

            _db.Books.Remove(query);
            _db.SaveChanges();


            return RedirectToAction("Index");
        }
        /***
         * READ       
         */
        [HttpGet]
        public IActionResult Index(long id)
        { 
            IEnumerable<Book> books;
           
            //TODO: diplay a single book with the given id if its greater than zero
            if(id > 0){

                books = _db.Books.Where(b => b.Id == id)
                            .ToList();

            } else {
                // otherwise return the entire set of books
                books = _db.Books.OrderBy(b => b.SKU);
            
            }


                            

            
            var searchResults = new SearchResultsVM
            {
                Books = books,
                IsSale = false //Just display the regular prices
            };

            return View("SearchResults", searchResults);
        }

        /***
         * CREATE
         */
        [HttpGet]
        public IActionResult CreateBook()
        {
            //TODO: Build a new CreateBookViewModel with a complete set of Writers from the database
            //      sort the Writers by Name
            CreateBookVM createBookVM = new CreateBookVM
            {
                Authors = _db.Writers.OrderBy(w => w.Name)
            };

            return View(createBookVM); //Passes the ViewModel to populate the "AUTHOR NAME" drop down in the CreateBook View
        }
        [HttpPost]
        public IActionResult CreateBook(CreateBookVM createBookVM, long id)
        {
            //TODO: Build the Writer object for the given Book, using the view Model info.
            // HINT: you will need to do it differently based on what the user entered
            //    - the VM contains an AuthorId, then get the Author object from the DbContext
            //    - the VM contains an AuthorName, then create a new Author object and Add it to the DbContext
            Writer foundWriter = _db.Writers.Find(createBookVM.AuthorId);
            Writer bookAuthor = foundWriter ?? new Writer { Name = createBookVM.AuthorName };

            Book book = _db.Books.Find(id);

            if (book == null)
            {
                book = new Book();
                _db.Books.Add(book); 
            }

            //Builds the Book using the parameter data and your newly created author.
            //TODO: AFTER COMPLETING the UpdateBook method,adjust this code to make sure to only create a new book when the needed
            book.Title = createBookVM.Title;
            book.SKU = createBookVM.SKU;
            book.Price = createBookVM.Price;
            book.Author = bookAuthor;


            //TODO: Add the new book to the DbContext (or just skip to SaveChanges for an existing book update)
            _db.SaveChanges();

            //Shows the new book by passing the Book's id to the Index Action 
            return RedirectToAction("Index", new { id = book.Id });
        }


        /***
         *  UPDATE a Book (reusing the CreateBook View to let the user modify the data ) 
         */
         
         [HttpGet]
         public IActionResult UpdateBook(long id)
        {
            //TODO: Write a method to load book info into the ViewModel for the CreateBook View

            
            Book book = _db.Books.Find(id);

            var bookVM = new CreateBookVM
            {
                BookId = id,
                Title = book.Title,
                SKU = book.SKU,
                Price = book.Price,
                AuthorId = book.Author.Id,
                Authors = _db.Writers
            };

            return View("CreateBook", bookVM);
            
        }
        
        [HttpGet]
        public IActionResult Search() { return View(); }
        [HttpPost]
        public IActionResult Search(SearchVM searchVM)
        {
            var searchResults = searchVM.HalfPriceSale ?
            new SearchResultsVM { 
                Books = _repo.SaleResults,
                IsSale = true
            } : 
            new SearchResultsVM { 
                Books = _repo.searchResults(searchVM).ToList(),
                IsSale = false
            }; 

            return View("SearchResults", searchResults);

    }
}
