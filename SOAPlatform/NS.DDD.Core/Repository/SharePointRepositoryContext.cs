using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NS.Framework.Utility;

namespace NS.DDD.Core.Repository
{
    /// <summary>
    /// Represents the base class for repository contexts. --- SharePoint
    /// </summary>
    public abstract class SharePointRepositoryContext : DisposableObject, ISharePointRepositoryContext
    {
        #region Private Fields
        private readonly string id = Guid.NewGuid().ToString();
        private readonly ThreadLocal<Dictionary<string, object>> localNewCollection = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());
        private readonly ThreadLocal<Dictionary<string, object>> localModifiedCollection = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());
        private readonly ThreadLocal<Dictionary<string, object>> localDeletedCollection = new ThreadLocal<Dictionary<string, object>>(() => new Dictionary<string, object>());
        private readonly ThreadLocal<bool> localCommitted = new ThreadLocal<bool>(() => true);
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
        protected IEnumerable<KeyValuePair<string, object>> NewCollection
        {
            get { return localNewCollection.Value; }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be modified in the repository.
        /// </summary>
        protected IEnumerable<KeyValuePair<string, object>> ModifiedCollection
        {
            get { return localModifiedCollection.Value; }
        }
        /// <summary>
        /// Gets an enumerator which iterates over the collection that contains all the objects need to be deleted from the repository.
        /// </summary>
        protected IEnumerable<KeyValuePair<string, object>> DeletedCollection
        {
            get { return localDeletedCollection.Value; }
        }
        #endregion

        #region IRepositoryContext Members
        /// <summary>
        /// Gets the ID of the repository context.
        /// </summary>
        public string ID
        {
            get { return id; }
        }
        /// <summary>
        /// Registers a new object to the repository context.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterNew<TAggregateRoot>(TAggregateRoot obj, string id) where TAggregateRoot : class, new()
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            //if (modifiedCollection.ContainsKey(obj.ID))
            if (localModifiedCollection.Value.ContainsKey(id))
                throw new InvalidOperationException("The object cannot be registered as a new object since it was marked as modified.");
            if (localNewCollection.Value.ContainsKey(id))
                throw new InvalidOperationException("The object has already been registered as a new object.");
            localNewCollection.Value.Add(id, obj);
            localCommitted.Value = false;
        }
        /// <summary>
        /// Registers a modified object to the repository context.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterModified<TAggregateRoot>(TAggregateRoot obj, string id) where TAggregateRoot : class, new()
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            if (localDeletedCollection.Value.ContainsKey(id))
                throw new InvalidOperationException("The object cannot be registered as a modified object since it was marked as deleted.");
            if (!localModifiedCollection.Value.ContainsKey(id) && !localNewCollection.Value.ContainsKey(id))
                localModifiedCollection.Value.Add(id, obj);
            localCommitted.Value = false;
        }
        /// <summary>
        /// Registers a deleted object to the repository context.
        /// </summary>
        /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
        /// <param name="obj">The object to be registered.</param>
        public virtual void RegisterDeleted<TAggregateRoot>(TAggregateRoot obj, string id) where TAggregateRoot : class, new()
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentException("The ID of the object is empty.", "obj");
            if (localNewCollection.Value.ContainsKey(id))
            {
                if (localNewCollection.Value.Remove(id))
                    return;
            }
            bool removedFromModified = localModifiedCollection.Value.Remove(id);
            bool addedToDeleted = false;
            if (!localDeletedCollection.Value.ContainsKey(id))
            {
                localDeletedCollection.Value.Add(id, obj);
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
            get { return localCommitted.Value; }
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

        #endregion
    }
}
