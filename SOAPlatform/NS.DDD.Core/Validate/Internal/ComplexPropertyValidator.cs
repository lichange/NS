// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using NS.DDD.Core.Validation;
using NS.Framework.Utility;

namespace NS.DDD.Core.Internal.Validation
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    ///     Validates a property of a given EDM complex type.
    /// </summary>
    /// <remarks>
    ///     This is a composite validator for a complex property of an entity.
    /// </remarks>
    internal class ComplexPropertyValidator : PropertyValidator
    {
        /// <summary>
        ///     The complex type validator.
        /// </summary>
        private readonly ComplexTypeValidator _complexTypeValidator;

        public ComplexTypeValidator ComplexTypeValidator
        {
            get { return _complexTypeValidator; }
        }

        /// <summary>
        ///     Creates an instance of <see cref="ComplexPropertyValidator" /> for a given complex property.
        /// </summary>
        /// <param name="propertyName"> The complex property name. </param>
        /// <param name="propertyValidators"> Validators used to validate the given property. </param>
        /// <param name="complexTypeValidator"> Complex type validator. </param>
        public ComplexPropertyValidator(
            string propertyName,
            IEnumerable<IValidator> propertyValidators,
            ComplexTypeValidator complexTypeValidator)
            : base(propertyName, propertyValidators)
        {
            _complexTypeValidator = complexTypeValidator;
        }

        /// <summary>
        ///     Validates a complex property.
        /// </summary>
        /// <param name="entityValidationContext"> Validation context. Never null. </param>
        /// <param name="property"> Property to validate. Never null. </param>
        /// <returns>
        ///     Validation errors as <see cref="IEnumerable{EntityValidationError}" /> . Empty if no errors. Never null.
        /// </returns>
        public override IEnumerable<EntityValidationError> Validate(
            EntityValidationContext entityValidationContext, InternalValidateProperty property)
        {
            DebugCheck.NotNull(property);

            var validationErrors = new List<EntityValidationError>();
            validationErrors.AddRange(base.Validate(entityValidationContext, property));

            // don't drill into complex types if there were errors or the complex property has not been initialized at all
            if (!validationErrors.Any()
                && property.PropertyValue != null
                &&
                _complexTypeValidator != null)
            {
                validationErrors.AddRange(
                    _complexTypeValidator.Validate(entityValidationContext, property));
            }

            return validationErrors;
        }
    }
}
