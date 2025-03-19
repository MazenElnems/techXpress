using DataAccessLayer.Abstraction;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        public void Create(T entity)
        {
            if(entity is Product p)
            {
                p.SellerId = 1;
            }
            _dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet;
        }

        public IEnumerable<T> GetAllWhere(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public T? GetById(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
