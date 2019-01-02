// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using NS.Framework.Utility;

namespace NS.DDD.Core.Internal.Validation
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    ///     Contains information needed to validate an entity or its properties.
    /// </summary>
    internal class EntityValidationContext
    {
        /// <summary>
        ///     The entity being validated or the entity that owns the property being validated.
        /// </summary>
        private readonly InternalValidateEntity _entityEntry;

        /// <summary>
        ///     Initializes a new instance of EntityValidationContext class.
        /// </summary>
        /// <param name="entityEntry"> The entity being validated or the entity that owns the property being validated. </param>
        /// <param name="externalValidationContext"> External context needed for validation. </param>
        public EntityValidationContext(InternalValidateEntity entityEntry, ValidationContext externalValidationContext)
        {
            DebugCheck.NotNull(entityEntry);
            DebugCheck.NotNull(externalValidationContext);

            _entityEntry = entityEntry;
            ExternalValidationContext = externalValidationContext;
        }

        /// <summary>
        ///     External context needed for validation.
        /// </summary>
        public ValidationContext ExternalValidationContext { get; private set; }

        /// <summary>
        ///     Gets the entity being validated or the entity that owns the property being validated.
        /// </summary>
        public InternalValidateEntity InternalEntity
        {
            get { return _entityEntry; }
        }
    }
}
