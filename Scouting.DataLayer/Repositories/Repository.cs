using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Scouting.DataLayer.Models;

namespace Scouting.DataLayer
{
    public class Repository<T> : IRepository<T> where T : IEntity
    {
        public Database Database { get; set; }

        public T Find(int id)
        {
            return Database.SingleOrDefault<T>("WHERE Id = @0", id);
        }

        public List<T> GetAll()
        {
            return Database.Query<T>(String.Empty).ToList();
        }

        public T Add(T entity)
        {
            Database.Insert(entity);
            return entity;
        }

        public T Update(T entity)
        {
            Database.Update(entity);
            return entity;
        }

        public void Remove(int id)
        {
            Database.Delete<T>(id);
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
