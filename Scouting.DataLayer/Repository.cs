using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        protected readonly Database Db = new Database("localDB"); // TODO: Use IOC

        public T Find(int id)
        {
            return Db.SingleOrDefault<T>("WHERE Id = @0", id);
        }

        public List<T> GetAll()
        {
            return Db.Query<T>(String.Empty).ToList();
        }

        public T Add(T entity)
        {
            Db.Insert(entity);
            return entity;
        }

        public T Update(T entity)
        {
            Db.Update(entity);
            return entity;
        }

        public void Remove(int id)
        {
            Db.Delete<T>(id);
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
