using Xunit;
using BookListMVC.Controllers;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using BookListMVC.Models;
using static BookListMVC.Controllers.BooksController;
using System.Text.Json;

namespace BookListTests
{
    public class UnitTest1
    {
        
        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "BookListMVC" + System.Guid.NewGuid())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void AddBook_ShouldCreateNewBook()
        {
            var context = CreateInMemoryDb();
            var controller = new BooksController(context)
            {
                Book = new Book
                {
                    Name = "Clean Code",
                    Author = "Robert C. Martin",
                    ISBN = "1234567890"
                }
            };

            var result = controller.Upsert();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var book = context.Books.FirstOrDefault();
            Assert.NotNull(book);
            Assert.Equal("Clean Code", book.Name);
            Assert.Equal("Robert C. Martin", book.Author);
            Assert.Equal("1234567890", book.ISBN);
        }

        [Fact]
        public void UpdateBook_ShouldChangeAuthor_WhenIdExists()
        {
            var context = CreateInMemoryDb();

            // Création initiale
            var book = new Book
            {
                Name = "Design Patterns",
                Author = "Initial Author",
                ISBN = "0987654321"
            };
            context.Books.Add(book);
            context.SaveChanges();

            context.Entry(book).State = EntityState.Detached;

            // Création d'une nouvelle instance avec le même Id
            var controller = new BooksController(context)
            {
                Book = new Book
                {
                    Id = book.Id,
                    Name = "Design Patterns",
                    Author = "Updated Author",
                    ISBN = "0987654321"
                }
            };

            var result = controller.Upsert();

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            var updatedBook = context.Books.FirstOrDefault(b => b.Id == book.Id);
            Assert.NotNull(updatedBook);
            Assert.Equal("Updated Author", updatedBook.Author);
        }


        [Fact]
        public async Task DeleteBook_ShouldRemoveBook_WhenIdExists()
        {
            var context = CreateInMemoryDb();
            var book = new Book
            {
                Name = "To Delete",
                Author = "Author D",
                ISBN = "1111222233"
            };
            context.Books.Add(book);
            context.SaveChanges();

            var controller = new BooksController(context);

            // Act
            var result = await controller.Delete(book.Id) as JsonResult;

            // Convertir en JSON puis en objet typé
            var json = JsonSerializer.Serialize(result.Value);
            var response = JsonSerializer.Deserialize<DeleteResponse>(json);

            // Assert
            Assert.True(response.success);
            Assert.Equal("Delete successful", response.message);
            Assert.Empty(context.Books);
        }




        [Fact]
        public void AddBook_WithInvalidModel_ShouldReturnView()
        {
            var context = CreateInMemoryDb();
            var controller = new BooksController(context)
            {
                Book = new Book { Name = "" } // invalid
            };

            controller.ModelState.AddModelError("Name", "Required");

            var result = controller.Upsert();

            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<Book>(view.Model);
        }

        [Fact]
        public void UpsertGet_WithNullId_ShouldReturnEmptyBook()
        {
            var context = CreateInMemoryDb();
            var controller = new BooksController(context);

            var result = controller.Upsert(null) as ViewResult;

            var model = Assert.IsType<Book>(result.Model);
            Assert.Equal(0, model.Id);
            Assert.Null(model.Name);
        }

        [Fact]
        public void UpsertGet_WithValidId_ShouldReturnCorrectBook()
        {
            var context = CreateInMemoryDb();
            var book = new Book { Name = "Design Patterns" };
            context.Books.Add(book);
            context.SaveChanges();

            var controller = new BooksController(context);
            var result = controller.Upsert(book.Id) as ViewResult;

            var model = Assert.IsType<Book>(result.Model);
            Assert.Equal("Design Patterns", model.Name);
        }


    }
}