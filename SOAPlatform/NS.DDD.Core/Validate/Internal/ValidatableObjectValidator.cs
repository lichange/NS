// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using NS.DDD.Core.Validation;
using NS.Framework.Utility;

namespace NS.DDD.Core.Internal.Validation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    ///     Validates entities or complex types implementing IValidatableObject interface.
    /// </summary>
    internal class ValidatableObjectValidator : IValidator
    {
        /// <summary>
        ///     Display attribute used to specify the display name for an entity or complex property.
        /// </summary>
        private readonly DisplayAttribute _displayAttribute;

        public ValidatableObjectValidator(DisplayAttribute displayAttribute)
        {
            _displayAttribute = displayAttribute;
        }

        /// <summary>
        ///     Validates an entity or a complex type implementing IValidatableObject interface.
        ///     This method is virtual to allow mocking.
        /// </summary>
        /// <param name="entityValidationContext"> Validation context. Never null. </param>
        /// <param name="property"> Property to validate. Null if this is the entity that will be validated. Never null if this is the complex type that will be validated. </param>
        /// <returns>
        ///     Validation error as <see cref="IEnumerable{EntityValidationError}" /> . Empty if no errors. Never null.
        /// </returns>
        /// <remarks>
        ///     Note that <paramref name="property" /> is used to figure out what needs to be validated. If it not null the complex
        ///     type will be validated otherwise the entity will be validated.
        ///     Also if this is an IValidatableObject complex type but the instance (.CurrentValue) is null we won't validate
        ///     anything and will not return any errors. The reason for this is that Validation is supposed to validate using
        ///     information the user provided and not some additional implicit rules. (ObjectContext will throw for operations
        ///     that involve null complex properties).
        /// </remarks>
        public virtual IEnumerable<EntityValidationError> Validate(
            EntityValidationContext entityValidationContext, InternalValidateProperty property)
        {
            DebugCheck.NotNull(entityValidationContext);

            Debug.Assert(
                (property == null && entityValidationContext.InternalEntity.EntityValue is IValidatableObject) ||
                (property != null && (property.PropertyValue == null || property.PropertyValue is IValidatableObject)),
                "Neither entity nor complex type implements IValidatableObject.");

            if (property != null
                && property.PropertyValue == null)
            {
                return Enumerable.Empty<EntityValidationError>();
            }

            var validationContext = entityValidationContext.ExternalValidationContext;

            //validationContext.SetDisplayName(property, _displayAttribute);

            var validatableObject = (IValidatableObject)(property == null
                                                             ? entityValidationContext.InternalEntity.EntityValue
                                                             : property.PropertyValue);

            IEnumerable<ValidationResult> validationResults = null;
            try
            {
                validationResults = validatableObject.Validate(validationContext);
            }
            catch (Exception ex)
            {
                //throw new DbUnexpectedValidationException(
                //    Strings.DbUnexpectedValidationException_IValidatableObject(
                //        validationContext.DisplayName, ObjectContextTypeCache.GetObjectType(validatableObject.GetType())),
                //    ex);
                throw new DbUnexpectedValidationException(
                 ex.Message);
            }

            return this.SplitValidationResults(
                validationContext.MemberName,
                validationResults ?? Enumerable.Empty<ValidationResult>());
        }

        public IEnumerable<EntityValidationError> SplitValidationResults(
           string propertyName, IEnumerable<ValidationResult> validationResults)
        {
            DebugCheck.NotNull(validationResults);

            foreach (var validationResult in validationResults)
            {
                if (validationResult == null)
                {
                    continue;
                }
                // let's treat null or empty .MemberNames the same way as one undefined (null) memberName
                var memberNames = validationResult.MemberNames == null || !validationResult.MemberNames.Any()
                                      ? new string[] { null }
                                      : validationResult.MemberNames;

                foreach (var memberName in memberNames)
                {
                    yield return new EntityValidationError(memberName ?? propertyName, validationResult.ErrorMessage);
                }
            }
        }
    }
}
