﻿using System.Collections;
using System.Collections.Generic;
using Framesharp.Collection;
using Framesharp.Core.Interfaces;
using Framesharp.Domain;
using Framesharp.Data.Collection;
using Framesharp.Data.Interfaces;
using NHibernate;
using NHibernate.Criterion;

namespace Framesharp.Repository
{
    public class RepositoryBase<T> : IRepository<T> where T : class, IDomainObject
    {
        protected readonly IOperationCallContext OperationCallContext;

        protected ISession Session { get { return ((ISessionProvider)OperationCallContext.SessionContainer).GetSession(); } }

        public RepositoryBase(IOperationCallContext context)
        {
            OperationCallContext = context;
        }

        public virtual void Save(T entity)
        {
            Session.Save(entity);
        }

        public virtual void Update(T entity)
        {
            Session.Update(entity);
        }

        public virtual void Delete(T entity)
        {
            Session.Delete(entity);
        }

        public virtual void SaveOrUpdate(T entity)
        {
            Session.SaveOrUpdate(entity);
        }

        public virtual void Refresh(T entity)
        {
            Session.Flush();
            Session.Evict(entity);

            Session.Refresh(entity);
        }

        public virtual T Get(object id)
        {
            return Session.Get<T>(id);
        }

        public virtual T Get(string columnName, object columnValue)
        {
            return Session.CreateCriteria<T>().Add(Restrictions.Eq(columnName, columnValue)).UniqueResult<T>();
        }

        public virtual T GetByCriteria(IDictionary criteriaCollection)
        {
            var criteria = Session.CreateCriteria<T>();

            criteria.Add(Restrictions.AllEq(criteriaCollection));

            return criteria.UniqueResult<T>();
        }

        public virtual T Get(object id, bool updateLock)
        {
            if (id == null) return default(T);

            if (updateLock)
            {
                return Session.Get<T>(id, LockMode.Upgrade);
            }

            return Session.Get<T>(id);
        }

        public virtual void Evict(T entity)
        {
            Session.Evict(entity);
        }
        
        public virtual bool VerifyId(object id)
        {
            T entity = Get(id);

            return entity != null;
        }

        public virtual IList<T> ListAllAscending(IDictionary criteriaCollection)
        {
            return ListAllAscending(criteriaCollection, null);
        }

        public virtual IList<T> ListAllAscending(IDictionary criteriaCollection, string orderByPropertyName)
        {
            Order order = string.IsNullOrEmpty(orderByPropertyName) ? null : Order.Asc(orderByPropertyName);

            return ListAll(criteriaCollection, order);
        }

        public virtual IList<T> ListAllAscending(string orderByPropertyName)
        {
            return ListAllAscending(null, orderByPropertyName);
        }

        public virtual IList<T> ListAllDescending(IDictionary criteriaCollection)
        {
            return ListAllDescending(criteriaCollection, null);
        }

        public virtual IList<T> ListAllDescending(IDictionary criteriaCollection, string orderByPropertyName)
        {
            Order order = string.IsNullOrEmpty(orderByPropertyName) ? null : Order.Desc(orderByPropertyName);

            return ListAll(criteriaCollection, order);
        }

        public virtual IList<T> ListAllDescending(string orderByPropertyName)
        {
            return ListAllDescending(null, orderByPropertyName);
        }

        public virtual IList<T> ListAll()
        {
            return ListAll(null, null);
        }

        public virtual IList<T> ListAll(IDictionary criteriaCollection)
        {
            return ListAll(criteriaCollection, null);
        }

        private IList<T> ListAll(IDictionary criteriaCollection, Order order)
        {
            return CreateCriteria(criteriaCollection, order).List<T>();
        }

        public virtual IPagedList<T> ListAllAscending(IDictionary criteriaCollection, int pageNumber, int pageSize)
        {
            return ListAllAscending(criteriaCollection, null, pageNumber, pageSize);
        }

        public virtual IPagedList<T> ListAllAscending(IDictionary criteriaCollection, string orderByPropertyName, int pageNumber, int pageSize)
        {
            Order order = string.IsNullOrEmpty(orderByPropertyName) ? null : Order.Asc(orderByPropertyName);

            return ListAll(criteriaCollection, order, pageNumber, pageSize);
        }

        public virtual IPagedList<T> ListAllAscending(string orderByPropertyName, int pageNumber, int pageSize)
        {
            return ListAllAscending(null, orderByPropertyName, pageNumber, pageSize);
        }

        public virtual IPagedList<T> ListAllDescending(IDictionary criteriaCollection, int pageNumber, int pageSize)
        {
            return ListAllDescending(criteriaCollection, null, pageNumber, pageSize);
        }

        public virtual IPagedList<T> ListAllDescending(IDictionary criteriaCollection, string orderByPropertyName, int pageNumber, int pageSize)
        {
            Order order = string.IsNullOrEmpty(orderByPropertyName) ? null : Order.Desc(orderByPropertyName);

            return ListAll(criteriaCollection, order, pageNumber, pageSize);
        }

        public virtual IPagedList<T> ListAllDescending(string orderByPropertyName, int pageNumber, int pageSize)
        {
            return ListAllDescending(null, orderByPropertyName, pageNumber, pageSize);
        }

        public virtual IPagedList<T> ListAll(int pageNumber, int pageSize)
        {
            return ListAll(null, null, pageNumber, pageSize);
        }

        public virtual IPagedList<T> ListAll(IDictionary criteriaCollection, int pageNumber, int pageSize)
        {
            return ListAll(criteriaCollection, null, pageNumber, pageSize);
        }

        private IPagedList<T> ListAll(IDictionary criteriaCollection, Order order, int pageNumber, int pageSize)
        {
            return new PagedResult<T>(CreateCriteria(criteriaCollection, order), pageNumber, pageSize);
        }
        
        private ICriteria CreateCriteria(IDictionary criteriaCollection, Order order)
        {
            ICriteria criteria = Session.CreateCriteria<T>();

            if (criteriaCollection != null && criteriaCollection.Count > 0)
                criteria.Add(Restrictions.AllEq(criteriaCollection));

            if (order != null)
                criteria.AddOrder(order);

            return criteria;
        }
    }
}
