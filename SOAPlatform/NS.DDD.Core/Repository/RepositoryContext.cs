using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NS.Framework.Utility;
using NS.Framework.Utility.Reflection;

namespace NS.DDD.Core.Repository
{
    /// <summary>
    /// Represents the base class for repository contexts.
    /// </summary>
    public abstract class RepositoryContext : DisposableObject, IRepositoryContext
    {
        #region Private Fields
        private readonly string id = Guid.NewGuid().ToString();
        private readonly ThreadLocal<Dictionary<object, object>> localNewCollection = new ThreadLocal<Dictionary<object, object>>(() => new Dictionary<object, object>());
        private readonly ThreadLocal<Dictionary<object, object>> localModifiedCollection = new ThreadLocal<Dictionary<object, object>>(() => new Dictionary<object, object>());
        private readonly ThreadLocal<Dictionary<object, object>> localDeletedCollection = new ThreadLocal<Dictionary<object, object>>(() => new Dictionary<object, object>());
        private readonly ThreadLocal<bool> localCommitted = new ThreadLocal<bool>(() => true);
        private bool isDispose = false;
        #endregion

        #region Protected Methods
        /// <summary>
        /// Clears all the registration in the repository context.
        /// </summary>
        /// <remarks>Note that this can only be called after the repository context has successfully committed.</remarks>
        protected void ClearRegistrations()
        {
            //this.newCollection.Clear();
            //this.modifiedCollection.Clear();
            //this.localDeletedCollection.Value.Clear();
            this.localNewCollection.Value.Clear();
            this.localModifiedCollection.Value.Clear();
            this.localDeletedCollection.Value.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            isDispose = disposing;
            if (disposing)
            {
                this.localCommitted.Dispose();
                this.localDeletedCollection.Dispose();
                this.localModifiedCollection.Dispose();
                this.localNewCollection.Dispose();
            }
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be added to the repository.
        /// </summary>
        protected IEnumerable<KeyValuePair<object, object>> NewCollection
        {
            get { return localNewCollection.Value; }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be modified in the repository.
        /// </summary>
        protected IEnumerable<KeyValuePair<object, object>> ModifiedCollection
        {
            get { return localModifiedCollection.Value; }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be deleted from the repository.
        /// </summary>
        protected IEnumerable<KeyValuePair<object, object>> DeletedCollection
        {
            get { return localDeletedCollection.Value; }
        }
        #endregion

        #region IRepositoryContext Members
        /// <summary>
        /// Gets the ID of the repository context.
        /// </summary>
        public string AggregateID
        {
            get { return id; }
        }
        /// <summary>
        /// Registers a new object to the repository context.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterNew<TAggregateRoot>(TAggregateRoot obj) where TAggregateRoot : class, IAggregateRoot
        {
            var tempKey = obj.GetAggregateID();
            if (tempKey.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            //if (modifiedCollection.ContainsKey(obj.ID))
            if (localModifiedCollection.Value.ContainsKey(tempKey))
                throw new InvalidOperationException("The object cannot be registered as a new object since it was marked as modified.");
            if (localNewCollection.Value.ContainsKey(tempKey))
                throw new InvalidOperationException("The object has already been registered as a new object.");
            localNewCollection.Value.Add(tempKey, obj);
            localCommitted.Value = false;
        }
        /// <summary>
        /// Registers a modified object to the repository context.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterModified<TAggregateRoot>(TAggregateRoot obj) where TAggregateRoot : class, IAggregateRoot
        {
            var tempKey = obj.GetAggregateID();
            if (tempKey.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            if (localDeletedCollection.Value.ContainsKey(tempKey))
                throw new InvalidOperationException("The object cannot be registered as a modified object since it was marked as deleted.");
            if (!localModifiedCollection.Value.ContainsKey(tempKey) && !localNewCollection.Value.ContainsKey(tempKey))
                localModifiedCollection.Value.Add(tempKey, obj);
            localCommitted.Value = false;
        }
        /// <summary>
        /// Registers a deleted object to the repository context.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterDeleted<TAggregateRoot>(TAggregateRoot obj) where TAggregateRoot : class, IAggregateRoot
        {
            var tempKey = obj.GetAggregateID();
            if (tempKey.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            if (localNewCollection.Value.ContainsKey(tempKey))
            {
                if (localNewCollection.Value.Remove(tempKey))
                    return;
            }
            bool removedFromModified = localModifiedCollection.Value.Remove(tempKey);
            bool addedToDeleted = false;
            if (!localDeletedCollection.Value.ContainsKey(tempKey))
            {
                localDeletedCollection.Value.Add(tempKey, obj);
                addedToDeleted = true;
            }
            localCommitted.Value = !(removedFromModified || addedToDeleted);
        }

        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates whether the UnitOfWork
        /// was committed.
        /// </summary>
        public bool Committed
        {
            get { return isDispose ? true : localCommitted.Value; }
            protected set { localCommitted.Value = value; }
        }
        /// <summary>
        /// Commits the UnitOfWork.
        /// </summary>
        public abstract void Commit();
        /// <summary>
        /// Rolls-back the UnitOfWork.
        /// </summary>
        public abstract void Rollback();

        public virtual void BaseCommit()
        {
            localNewCollection.Value.Clear();
            localModifiedCollection.Value.Clear();
            localDeletedCollection.Value.Clear();
        }

        #endregion

    }

    /// <summary>
    /// Represents the base class for repository contexts.
    /// </summary>
    public abstract class RepositoryContextBase : DisposableObject, IRepositoryContextBase
    {
        #region Private Fields
        private readonly string id = Guid.NewGuid().ToString();
        private readonly ThreadLocal<Dictionary<object, object>> localNewCollection = new ThreadLocal<Dictionary<object, object>>(() => new Dictionary<object, object>());
        private readonly ThreadLocal<Dictionary<object, object>> localModifiedCollection = new ThreadLocal<Dictionary<object, object>>(() => new Dictionary<object, object>());
        private readonly ThreadLocal<Dictionary<object, object>> localDeletedCollection = new ThreadLocal<Dictionary<object, object>>(() => new Dictionary<object, object>());
        private readonly ThreadLocal<bool> localCommitted = new ThreadLocal<bool>(() => true);
        private bool isDispose = false;
        #endregion

        #region Protected Methods
        /// <summary>
        /// Clears all the registration in the repository context.
        /// </summary>
        /// <remarks>Note that this can only be called after the repository context has successfully committed.</remarks>
        protected void ClearRegistrations()
        {
            //this.newCollection.Clear();
            //this.modifiedCollection.Clear();
            //this.localDeletedCollection.Value.Clear();
            this.localNewCollection.Value.Clear();
            this.localModifiedCollection.Value.Clear();
            this.localDeletedCollection.Value.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            isDispose = disposing;
            if (disposing)
            {
                this.localCommitted.Dispose();
                this.localDeletedCollection.Dispose();
                this.localModifiedCollection.Dispose();
                this.localNewCollection.Dispose();
            }
        }
        #endregion

        #region Protected Properties
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be added to the repository.
        /// </summary>
        protected IEnumerable<KeyValuePair<object, object>> NewCollection
        {
            get
            {
                return localNewCollection.Value;
            }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be modified in the repository.
        /// </summary>
        protected IEnumerable<KeyValuePair<object, object>> ModifiedCollection
        {
            get
            {
                return localModifiedCollection.Value;
            }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be deleted from the repository.
        /// </summary>
        protected IEnumerable<KeyValuePair<object, object>> DeletedCollection
        {
            get
            {
                return localDeletedCollection.Value;
            }
        }
        #endregion

        #region IRepositoryContext Members
        /// <summary>
        /// Gets the ID of the repository context.
        /// </summary>
        public string AggregateID
        {
            get
            {
                return id;
            }
        }
        /// <summary>
        /// Registers a new object to the repository context.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterNew<TAggregateRoot>(TAggregateRoot obj) where TAggregateRoot : class, IAggregateRoot
        {
            var tempKey = obj.GetAggregateID();
            if (tempKey.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            //if (modifiedCollection.ContainsKey(obj.ID))
            if (localModifiedCollection.Value.ContainsKey(tempKey))
                throw new InvalidOperationException("The object cannot be registered as a new object since it was marked as modified.");
            if (localNewCollection.Value.ContainsKey(tempKey))
                throw new InvalidOperationException("The object has already been registered as a new object.");
            localNewCollection.Value.Add(tempKey, obj);
            localCommitted.Value = false;
        }
        /// <summary>
        /// Registers a modified object to the repository context.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterModified<TAggregateRoot>(TAggregateRoot obj) where TAggregateRoot : class, IAggregateRoot
        {
            var tempKey = obj.GetAggregateID();
            if (tempKey.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            if (localDeletedCollection.Value.ContainsKey(tempKey))
                throw new InvalidOperationException("The object cannot be registered as a modified object since it was marked as deleted.");
            if (!localModifiedCollection.Value.ContainsKey(tempKey) && !localNewCollection.Value.ContainsKey(tempKey))
                localModifiedCollection.Value.Add(tempKey, obj);
            localCommitted.Value = false;
        }
        /// <summary>
        /// Registers a deleted object to the repository context.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterDeleted<TAggregateRoot>(TAggregateRoot obj) where TAggregateRoot : class, IAggregateRoot
        {
            var tempKey = obj.GetAggregateID();
            if (tempKey.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            if (localNewCollection.Value.ContainsKey(tempKey))
            {
                if (localNewCollection.Value.Remove(tempKey))
                    return;
            }
            bool removedFromModified = localModifiedCollection.Value.Remove(tempKey);
            bool addedToDeleted = false;
            if (!localDeletedCollection.Value.ContainsKey(tempKey))
            {
                localDeletedCollection.Value.Add(tempKey, obj);
                addedToDeleted = true;
            }
            localCommitted.Value = !(removedFromModified || addedToDeleted);
        }

        #endregion

        #region IUnitOfWork Members
        /// <summary>
        /// Gets a <see cref="System.Boolean"/> value which indicates whether the UnitOfWork
        /// was committed.
        /// </summary>
        public bool Committed
        {
            get
            {
                return isDispose ? true : localCommitted.Value;
            }
            protected set
            {
                localCommitted.Value = value;
            }
        }
        /// <summary>
        /// Commits the UnitOfWork.
        /// </summary>
        public abstract void Commit();
        /// <summary>
        /// Rolls-back the UnitOfWork.
        /// </summary>
        public abstract void Rollback();

        public virtual void BaseCommit()
        {
            localNewCollection.Value.Clear();
            localModifiedCollection.Value.Clear();
            localDeletedCollection.Value.Clear();
        }

        public void RegisterNew(object obj)
        {
            var tempEntity = obj as Entity;
            if (tempEntity == null)
                throw new ArgumentException("object type must Entity or AggregateRoot");
            var tempId= tempEntity.GetBaseAggregateID();
            //ReflectHelper.GetPropertyValue(obj, "AggregateID");
            if (tempId.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            //if (modifiedCollection.ContainsKey(obj.ID))
            if (localModifiedCollection.Value.ContainsKey(tempId))
                throw new InvalidOperationException("The object cannot be registered as a new object since it was marked as modified.");
            if (localNewCollection.Value.ContainsKey(tempId))
                throw new InvalidOperationException("The object has already been registered as a new object.");
            localNewCollection.Value.Add(tempId, obj);
            localCommitted.Value = false;
        }

        public void RegisterModified(object obj)
        {
            var tempEntity = obj as Entity;
            if (tempEntity == null)
                throw new ArgumentException("object type must Entity or AggregateRoot");
            var tempId = tempEntity.GetBaseAggregateID();

            //var tempId = ReflectHelper.GetPropertyValue(obj, "AggregateID");
            if (tempId.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            if (localDeletedCollection.Value.ContainsKey(tempId))
                throw new InvalidOperationException("The object cannot be registered as a modified object since it was marked as deleted.");
            if (!localModifiedCollection.Value.ContainsKey(tempId) && !localNewCollection.Value.ContainsKey(tempId))
                localModifiedCollection.Value.Add(tempId, obj);
            localCommitted.Value = false;
        }

        public void RegisterDeleted(object obj)
        {
            var tempEntity = obj as Entity;
            if (tempEntity == null)
                throw new ArgumentException("object type must Entity or AggregateRoot");
            var tempId = tempEntity.GetBaseAggregateID();

            //var tempId = ReflectHelper.GetPropertyValue(obj, "AggregateID");
            if (tempId.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            if (localNewCollection.Value.ContainsKey(tempId))
            {
                if (localNewCollection.Value.Remove(tempId))
                    return;
            }
            bool removedFromModified = localModifiedCollection.Value.Remove(tempId);
            bool addedToDeleted = false;
            if (!localDeletedCollection.Value.ContainsKey(tempId))
            {
                localDeletedCollection.Value.Add(tempId, obj);
                addedToDeleted = true;
            }
            localCommitted.Value = !(removedFromModified || addedToDeleted);
        }

        #endregion

    }
}
