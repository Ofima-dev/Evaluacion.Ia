using Evaluacion.IA.Application.Interfaces;
using Evaluacion.IA.Domain.Entities;
using Evaluacion.IA.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Evaluacion.IA.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private IDbContextTransaction? _transaction;

        private IRepository<User>? _users;
        private IRepository<Role>? _roles;
        private IRepository<Category>? _categories;
        private IRepository<Product>? _products;
        private IRepository<ProductImage>? _productImages;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        public IRepository<User> Users =>
            _users ??= new Repository<User>(_context);

        public IRepository<Role> Roles =>
            _roles ??= new Repository<Role>(_context);

        public IRepository<Category> Categories =>
            _categories ??= new Repository<Category>(_context);

        public IRepository<Product> Products =>
            _products ??= new Repository<Product>(_context);

        public IRepository<ProductImage> ProductImages =>
            _productImages ??= new Repository<ProductImage>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
