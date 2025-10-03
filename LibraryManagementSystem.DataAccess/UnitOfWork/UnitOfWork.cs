using LibraryManagementSystem.DataAccess.Contract;
using LibraryManagementSystem.DataAccess.Data;
using LibraryManagementSystem.DataAccess.Entities;
using LibraryManagementSystem.DataAccess.Entities.People;
using LibraryManagementSystem.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.UnitOfWork
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly  Lazy<IAuthorRepository> _authorRepository;
        private readonly  Lazy<IBookRepository> _bookRepository ;
        private readonly  Lazy<IBorrowerRepository> _borrowerRepository;
        private readonly  Lazy<IPersonRepository> _personRepository;
        private readonly  Lazy<IBorrowingRepository> _borrowingRepository;

        private readonly AppDbContext _appDbContext ;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

            _authorRepository = new Lazy<IAuthorRepository>(() => new AuthorRepository(appDbContext));
            _bookRepository = new Lazy<IBookRepository>(() => new BookRepository(appDbContext));
            _borrowerRepository = new Lazy<IBorrowerRepository>(() => new BorrowerRepository(appDbContext));
            _personRepository = new Lazy<IPersonRepository>(() => new PersonRepository(appDbContext));
            _borrowingRepository = new Lazy<IBorrowingRepository>(() => new BorrowingRepository(appDbContext));

        }


        public IAuthorRepository AuthorRepository => _authorRepository.Value;

        public IBookRepository BookRepository => _bookRepository.Value;
        public IBorrowerRepository BorrowerRepository => _borrowerRepository.Value;
        public IBorrowingRepository BorrowingRepository => _borrowingRepository.Value;
        public IPersonRepository PersonRepository => _personRepository.Value;

        public async Task<int> CompleteAsync() => await _appDbContext.SaveChangesAsync();

        public void Dispose() => _appDbContext.Dispose();
    }
}
