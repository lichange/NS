﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using NS.DDD.Core.Validation;
using NS.Framework.Utility;

namespace NS.DDD.Core.Internal.Validation
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    ///     Validator used to validate an entity of a given EDM EntityType.
    /// </summary>
    /// <remarks>
    ///     This is a top level, composite validator. This is also an entry point to getting an entity
    ///     validated as validation of an entity is always started by calling Validate method on this type.
    /// </remarks>
    internal class EntityValidator : TypeValidator
    {
        /// <summary>
        ///     Creates an instance <see cref="EntityValidator" /> for a given EDM entity type.
        /// </summary>
        /// <param name="propertyValidators"> Property validators. </param>
        /// <param name="typeLevelValidators"> Entity type level validators. </param>
        public EntityValidator(
            IEnumerable<PropertyValidator> propertyValidators, IEnumerable<IValidator> typeLevelValidators)
            :
                base(propertyValidators, typeLevelValidators)
        {
        }

        /// <summary>
        ///     Validates an entity.
        /// </summary>
        /// <param name="entityValidationContext"> Entity validation context. Must not be null. </param>
        /// <returns>
        ///     <see cref="DbEntityValidationResult" /> instance. Never null.
        /// </returns>
        public EntityValidationResult Validate(EntityValidationContext entityValidationContext)
        {
            DebugCheck.NotNull(entityValidationContext);
            Debug.Assert(entityValidationContext.InternalEntity != null);

            var validationErrors = Validate(entityValidationContext, null);

            return new EntityValidationResult((IAggregateRoot)entityValidationContext.InternalEntity.EntityValue, validationErrors);
        }

        /// <summary>
        ///     Validates type properties. Any validation errors will be added to <paramref name="validationErrors" />
        ///     collection.
        /// </summary>
        /// <param name="entityValidationContext"> Validation context. Must not be null. </param>
        /// <param name="parentProperty"> The entry for the complex property. Null if validating an entity. </param>
        /// <param name="validationErrors"> Collection of validation errors. Any validation errors will be added to it. </param>
        /// <remarks>
        ///     Note that <paramref name="validationErrors" /> will be modified by this method. Errors should be only added,
        ///     never removed or changed. Taking a collection as a modifiable parameter saves a couple of memory allocations
        ///     and a merge of validation error lists per entity.
        /// </remarks>
        protected override void ValidateProperties(
            EntityValidationContext entityValidationContext, InternalValidateProperty parentProperty,
            List<EntityValidationError> validationErrors)
        {
            DebugCheck.NotNull(entityValidationContext);
            DebugCheck.NotNull(validationErrors);

            var entityEntry = entityValidationContext.InternalEntity;

            foreach (var validator in PropertyValidators)
            {
                validationErrors.AddRange(
                    validator.Validate(entityValidationContext, entityEntry.GetProperty(validator.PropertyName)));
            }
        }
    }
}
