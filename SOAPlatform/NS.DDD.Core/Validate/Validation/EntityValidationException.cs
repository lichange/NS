// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Data;
using NS.Framework.Utility;

namespace NS.DDD.Core.Validation
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    ///     Exception thrown from Repository when validating entities fails.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors",
        Justification = "SerializeObjectState used instead")]
    [Serializable]
    public class EntityValidationException : DataException
    {
        [NonSerialized]
        private DbEntityValidationExceptionState _state = new DbEntityValidationExceptionState();

        /// <summary>
        ///     Initializes a new instance of DbEntityValidationException.
        /// </summary>
        public EntityValidationException()
            : this("数据格式验证失败")
        {
        }

        /// <summary>
        ///     Initializes a new instance of DbEntityValidationException.
        /// </summary>
        /// <param name="message"> The exception message. </param>
        public EntityValidationException(string message)
            : this(message, Enumerable.Empty<EntityValidationResult>())
        {
        }

        /// <summary>
        ///     Initializes a new instance of DbEntityValidationException.
        /// </summary>
        /// <param name="message"> The exception message. </param>
        /// <param name="entityValidationResults"> Validation results. </param>
        public EntityValidationException(
            string message, IEnumerable<EntityValidationResult> entityValidationResults)
            : base(message)
        {
            // Users should be able to set the errors to null but we should not
            Check.NotNull(entityValidationResults, "entityValidationResults");

            _state.InititializeValidationResults(entityValidationResults);
            SubscribeToSerializeObjectState();
        }

        /// <summary>
        ///     Initializes a new instance of DbEntityValidationException.
        /// </summary>
        /// <param name="message"> The exception message. </param>
        /// <param name="innerException"> The inner exception. </param>
        public EntityValidationException(string message, Exception innerException)
            : this(message, Enumerable.Empty<EntityValidationResult>(), innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of DbEntityValidationException.
        /// </summary>
        /// <param name="message"> The exception message. </param>
        /// <param name="entityValidationResults"> Validation results. </param>
        /// <param name="innerException"> The inner exception. </param>
        public EntityValidationException(
            string message, IEnumerable<EntityValidationResult> entityValidationResults, Exception innerException)
            : base(message, innerException)
        {
            // Users should be able to set the errors to null but we should not. 
            Check.NotNull(entityValidationResults, "entityValidationResults");

            _state.InititializeValidationResults(entityValidationResults);
            SubscribeToSerializeObjectState();
        }

        /// <summary>
        ///     Validation results.
        /// </summary>
        public IEnumerable<EntityValidationResult> EntityValidationErrors
        {
            get { return _state.EntityValidationErrors; }
        }

        /// <summary>
        ///     Subscribes the SerializeObjectState event.
        /// </summary>
        private void SubscribeToSerializeObjectState()
        {
            SerializeObjectState += (exception, eventArgs) => eventArgs.AddSerializedState(_state);
        }

        /// <summary>
        ///     Holds exception state that will be serialized when the exception is serialized.
        /// </summary>
        [Serializable]
        private class DbEntityValidationExceptionState : ISafeSerializationData
        {
            /// <summary>
            ///     Validation results.
            /// </summary>
            private IList<EntityValidationResult> _entityValidationResults;

            internal void InititializeValidationResults(IEnumerable<EntityValidationResult> entityValidationResults)
            {
                _entityValidationResults = entityValidationResults == null
                                               ? new List<EntityValidationResult>()
                                               : entityValidationResults.ToList();
            }

            /// <summary>
            ///     Validation results.
            /// </summary>
            public IEnumerable<EntityValidationResult> EntityValidationErrors
            {
                get { return _entityValidationResults; }
            }

            /// <summary>
            ///     Completes the deserialization.
            /// </summary>
            /// <param name="deserialized"> The deserialized object. </param>
            public void CompleteDeserialization(object deserialized)
            {
                var validationException = (EntityValidationException)deserialized;
                
                validationException._state = this;
                validationException.SubscribeToSerializeObjectState();
            }
        }
    }
}
