using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        protected readonly Database _db = new Database("localDB"); // TODO: Use IOC

        public T Find(int id)
        {
            return _db.SingleOrDefault<T>("WHERE Id = @0", id);
        }

        public List<T> GetAll()
        {
            return _db.Query<T>(String.Empty).ToList();
        }

        public T Add(T entity)
        {
            _db.Insert(entity);
            return entity;
        }

        public T Update(T entity)
        {
            _db.Update(entity);
            return entity;
        }

        public void Remove(int id)
        {
            _db.Delete<T>(id);
        }

        public void Save(T entity)
        {
            using (var txScope = new TransactionScope())
            {
                if (entity.IsNew)
                {
                    Add(entity);
                }
                else
                {
                    Update(entity);
                }

                txScope.Complete();
            }
        }
    }
}
