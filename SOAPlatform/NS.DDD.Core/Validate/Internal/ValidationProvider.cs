// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using NS.Framework.Utility;

namespace NS.DDD.Core.Internal.Validation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     Used to cache and retrieve generated validators and to create context for validating entities or properties.
    /// </summary>
    internal class ValidationProvider
    {
        /// <summary>
        ///     Collection of validators keyed by the entity CLR type. Note that if there's no validation for a given type
        ///     it will be associated with a null validator.
        /// </summary>
        private readonly Dictionary<Type, EntityValidator> _entityValidators;

        private readonly EntityValidatorBuilder _entityValidatorBuilder;

        /// <summary>
        ///     Initializes a new instance of <see cref="ValidationProvider" /> class.
        /// </summary>
        public ValidationProvider(EntityValidatorBuilder builder = null, AttributeProvider attributeProvider = null)
        {
            _entityValidators = new Dictionary<Type, EntityValidator>();
            _entityValidatorBuilder = builder ?? new EntityValidatorBuilder(attributeProvider ?? new AttributeProvider());
        }

        /// <summary>
        ///     Returns a validator to validate <paramref name="entityEntry" />.
        /// </summary>
        /// <param name="entityEntry"> Entity the validator is requested for. </param>
        /// <returns>
        ///     <see cref="EntityValidator" /> to validate <paramref name="entityEntry" /> . Possibly null if no validation has been specified for the entity.
        /// </returns>
        public virtual EntityValidator GetEntityValidator(InternalValidateEntity entityEntry)
        {
            DebugCheck.NotNull(entityEntry);

            var entityType = entityEntry.EntityType;
            EntityValidator validator = null;

            if (entityType == null)
                return validator;

            if (_entityValidators.TryGetValue(entityType, out validator))
            {
                return validator;
            }
            else
            {
                validator = _entityValidatorBuilder.BuildEntityValidator(entityEntry);
                _entityValidators[entityType] = validator;
                return validator;
            }
        }

        /// <summary>
        ///     Returns a validator to validate <paramref name="property" />.
        /// </summary>
        /// <param name="owningEntity"> </param>
        /// <param name="property"> Navigation property the validator is requested for. </param>
        /// <returns>
        ///     Validator to validate <paramref name="property" /> . Possibly null if no validation has been specified for the requested property.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1614:ElementParameterDocumentationMustHaveText")]
        public virtual PropertyValidator GetPropertyValidator(
            InternalValidateEntity owningEntity, InternalValidateProperty property)
        {
            DebugCheck.NotNull(owningEntity);
            DebugCheck.NotNull(property);

            var entityValidator = GetEntityValidator(owningEntity);

            return entityValidator != null ? GetValidatorForProperty(entityValidator, property) : null;
        }

        /// <summary>
        ///     Gets a validator for the <paramref name="memberEntry" />.
        /// </summary>
        /// <param name="entityValidator"> Entity validator. </param>
        /// <param name="memberEntry"> Property to get a validator for. </param>
        /// <returns>
        ///     Validator to validate <paramref name="memberEntry" /> . Possibly null if there is no validation for the
        ///     <paramref
        ///         name="memberEntry" />
        ///     .
        /// </returns>
        /// <remarks>
        ///     For complex properties this method walks up the type hierarchy to get to the entity level and then goes down
        ///     and gets a validator for the child property that is an ancestor of the property to validate. If a validator
        ///     returned for an ancestor is null it means that there is no validation defined beneath and the method just
        ///     propagates (and eventually returns) null.
        /// </remarks>
        protected virtual PropertyValidator GetValidatorForProperty(
            EntityValidator entityValidator, InternalValidateProperty memberEntry)
        {
            var complexPropertyEntry = memberEntry;
            if (complexPropertyEntry == null)
            {
                //var propertyValidator =
                //    GetValidatorForProperty(entityValidator, complexPropertyEntry.ParentPropertyEntry) as
                //    ComplexPropertyValidator;
                //// if a validator for parent property is null there is no validation for child properties.  
                //// just propagate the null.
                //return propertyValidator != null && propertyValidator.ComplexTypeValidator != null
                //           ? propertyValidator.ComplexTypeValidator.GetPropertyValidator(memberEntry.Name)
                //           : null;
                return null;
            }
            else
            {
                return entityValidator.GetPropertyValidator(memberEntry.PropertyName);
            }
        }

        /// <summary>
        ///     Creates <see cref="EntityValidationContext" /> for <paramref name="entityEntry" />.
        /// </summary>
        /// <param name="entityEntry"> Entity entry for which a validation context needs to be created. </param>
        /// <param name="items"> User defined dictionary containing additional info for custom validation. This parameter is optional and can be null. </param>
        /// <returns>
        ///     An instance of <see cref="EntityValidationContext" /> class.
        /// </returns>
        /// <seealso cref="DbContext.ValidateEntity" />
        public virtual EntityValidationContext GetEntityValidationContext(
            InternalValidateEntity entityEntry, IDictionary<object, object> items)
        {
            DebugCheck.NotNull(entityEntry);

            return new EntityValidationContext(entityEntry, new ValidationContext(entityEntry.EntityValue, null, items));
        }
    }
}
