
using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using MongoDB.Bson;

namespace BookLibary.Api.Services.AuthServices.BookServices
{
    public interface IBookService
    {
        Task<GetManyResult<Book>> GetAllBooksAsync();
        Task<Book> GetByIdAsync(string id);
        Task<Book> GetByNameAsync(string name); 
        Task<Book> CreateBookAsync(Book book);
        // Task<GetOneResult<Book>> UpdateBookAsync(string id, Book book);
        Task<GetOneResult<Book>> DeleteBook(string bookName);
        Task<RateBookResultDto> RateBookAsync(RateBookRequest request, string UserName);
        Task<UserBookRatingDto> GetUserBookRatingAsync(string bookName, string userName);

    }
}