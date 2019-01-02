// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;

namespace NS.DDD.Core.Validation
{
    /// <summary>
    ///     Validation error. Can be either entity or property level validation error.
    /// </summary>
    [Serializable]
    public class EntityValidationError
    {
        /// <summary>
        ///     Name of the invalid property. Can be null (e.g. for entity level validations).
        /// </summary>
        private readonly string _propertyName;

        /// <summary>
        ///     Validation error message.
        /// </summary>
        private readonly string _errorMessage;

        /// <summary>
        ///     Creates an instance of <see cref="EntityValidationError" />.
        /// </summary>
        /// <param name="propertyName"> Name of the invalid property. Can be null. </param>
        /// <param name="errorMessage"> Validation error message. Can be null. </param>
        public EntityValidationError(string propertyName, string errorMessage)
        {
            _propertyName = propertyName;
            _errorMessage = errorMessage;
        }

        /// <summary>
        ///     Gets name of the invalid property.
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /// <summary>
        ///     Gets validation error message.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
        }
    }
}
