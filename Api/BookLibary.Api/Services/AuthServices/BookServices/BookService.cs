﻿using BookLibary.Api.Dtos.BookDto;
using BookLibary.Api.Models;
using BookLibary.Api.Repositories;
using DnsClient;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace BookLibary.Api.Services.AuthServices.BookServices
{
    public class BookService : IBookService
    {
        private readonly IBookRepository<Book> _bookRepository;
        private readonly IUserRepository<User> _userRepository;

        public BookService(IBookRepository<Book> bookRepository, IUserRepository<User> userRepository)
        {
            _bookRepository = bookRepository;
            _userRepository = userRepository;
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            return await _bookRepository.InsertOneAsync(book);
        }

        public async Task<GetOneResult<Book>> DeleteBook(string bookName)
        {
            return await _bookRepository.DeleteByNameAsync(bookName);
        }

        public async Task<GetManyResult<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Book> GetByIdAsync(string id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<Book> GetByNameAsync(string name)
        {
            return await _bookRepository.GetByNameAsync(name);
        }

        public async Task<RateBookResultDto> RateBookAsync(RateBookRequest request, string UserName)
        {
            User user = await _userRepository.GetByNameAsync(UserName);
            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı.");
            }

            // Validate BookName
            if (string.IsNullOrEmpty(request.BookName))
            {
                return new RateBookResultDto
                {
                    Success = false,
                    Message = "Book name cannot be empty."
                };
            }

            // Validate Rating value
            if (request.Rating < 1.0 || request.Rating > 5.0)
            {
                return new RateBookResultDto
                {
                    Success = false,
                    Message = "Rating must be between 1.0 and 5.0."
                };
            }

            // Fetch the book by name
            var book = await _bookRepository.GetBookByNameAsync(request.BookName);

            // Check if the book exists
            if (book == null)
            {
                return new RateBookResultDto
                {
                    Success = false,
                    Message = "Book not found."
                };
            }

            // Check if the user has already rated this book
            if (book.Ratings.Any(r => r.UserName == UserName))
            {
                return new RateBookResultDto
                {
                    Success = false,
                    Message = "You have already rated this book."
                };
            }

            // Initialize the Ratings list if it's null (just in case)
            if (book.Ratings == null)
            {
                book.Ratings = new List<Ratings>();
            }

            // Add the new rating to the Ratings list
            book.Ratings.Add(new Ratings
            {
                UserName = UserName,
                Value = request.Rating
            });

            // Update the RatingCount
            book.RatingCount = book.Ratings.Count;

            // Calculate the new AverageRating
            book.AverageRating = book.Ratings.Average(r => r.Value);

            // Update the book with the new rating information
            await _bookRepository.UpdateBookAsync(book.Id, book);

            // Return the result
            return new RateBookResultDto
            {
                Success = true,
                AverageRating = book.AverageRating,
                Message = "Rating successfully added."
            };
        }

    
    }

}
