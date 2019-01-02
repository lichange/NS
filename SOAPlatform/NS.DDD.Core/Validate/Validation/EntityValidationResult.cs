// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using NS.Framework.Utility;

namespace NS.DDD.Core.Validation
{
    /// <summary>
    ///     Represents validation results for single entity.
    /// </summary>
    [Serializable]
    public class EntityValidationResult
    {
        /// <summary>
        ///     Entity entry the results applies to. Never null.
        /// </summary>
        [NonSerialized]
        private readonly IAggregateRoot _entry;

        /// <summary>
        ///     List of <see cref="EntityValidationError" /> instances. Never null. Can be empty meaning the entity is valid.
        /// </summary>
        private readonly List<EntityValidationError> _validationErrors;

        /// <summary>
        ///     Creates an instance of <see cref="EntityValidationResult" /> class.
        /// </summary>
        /// <param name="entry"> Entity entry the results applies to. Never null. </param>
        /// <param name="validationErrors">
        ///     List of <see cref="EntityValidationError" /> instances. Never null. Can be empty meaning the entity is valid.
        /// </param>
        public EntityValidationResult(IAggregateRoot entry, IEnumerable<EntityValidationError> validationErrors)
        {
            //Check.NotNull(entry, "entry");
            Check.NotNull(validationErrors, "validationErrors");

            _entry = entry;
            _validationErrors = validationErrors.ToList();
        }

        /// <summary>
        ///     Gets an instance of <see cref="DbEntityEntry" /> the results applies to.
        /// </summary>
        public IAggregateRoot Entry
        {
            get
            {
                // The entry can be null when a DbEntityValidationResult instance was serialized and then deserialized 
                // with DbEntityValidationException it was a part of.
                return _entry;
            }
        }

        /// <summary>
        ///     Gets validation errors. Never null.
        /// </summary>
        public ICollection<EntityValidationError> ValidationErrors
        {
            get { return _validationErrors; }
        }

        /// <summary>
        ///     Gets an indicator if the entity is valid.
        /// </summary>
        public bool IsValid
        {
            get { return !_validationErrors.Any(); }
        }
    }
}
