// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using NS.DDD.Core.Validation;

namespace NS.DDD.Core.Internal.Validation
{
    using System.Collections.Generic;

    /// <summary>
    ///     Abstracts simple validators used to validate entities and properties.
    /// </summary>
    internal interface IValidator
    {
        /// <summary>
        ///     Validates an entity or a property.
        /// </summary>
        /// <param name="entityValidationContext"> Validation context. Never null. </param>
        /// <param name="property"> Property to validate. Can be null for type level validation. </param>
        /// <returns>
        ///     Validation error as <see cref="IEnumerable{EntityValidationError}" /> . Empty if no errors. Never null.
        /// </returns>
        IEnumerable<EntityValidationError> Validate(
            EntityValidationContext entityValidationContext, InternalValidateProperty property);
    }
}
