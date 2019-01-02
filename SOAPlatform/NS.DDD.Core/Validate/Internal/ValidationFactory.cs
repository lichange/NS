// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using NS.Framework.Utility;

namespace NS.DDD.Core.Internal.Validation
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     create validator factory.
    /// </summary>
    internal class ValidationFactory
    {
        private static ValidationProvider _validationProvider;
        private  static  readonly object _lockflag = new  object();

        /// <summary>
        ///     Initializes a new instance of <see cref="ValidationProvider" /> class.
        /// </summary>
        public static ValidationProvider Create()
        {
            lock (_lockflag)
            {
                if (_validationProvider == null)
                {
                    _validationProvider = new ValidationProvider(null,new AttributeProvider());
                }
            }

            return _validationProvider;
        }

    }
}
