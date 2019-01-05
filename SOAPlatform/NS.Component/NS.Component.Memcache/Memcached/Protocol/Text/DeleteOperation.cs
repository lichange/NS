using System;
using NS.Component.Memcache.Caching.Memcached.Results;
using NS.Component.Memcache.Caching.Memcached.Results.Extensions;

namespace NS.Component.Memcache.Caching.Memcached.Protocol.Text
{
	public class DeleteOperation : SingleItemOperation, IDeleteOperation
	{
		internal DeleteOperation(string key) : base(key) { }

		protected internal override System.Collections.Generic.IList<ArraySegment<byte>> GetBuffer()
		{
			var command = "delete " + this.Key + TextSocketHelper.CommandTerminator;

			return TextSocketHelper.GetCommandBuffer(command);
		}

		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			return new TextOperationResult
			{
				Success = String.Compare(TextSocketHelper.ReadResponse(socket), "DELETED", StringComparison.Ordinal) == 0
			};
		}

		protected internal override bool ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
		{
			throw new System.NotSupportedException();
		}
	}
}

#region [ License information          ]
/* ************************************************************
 * 
 *    Copyright (c) 2010 Attila Kisk? enyim.com
 *    
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *    
 *        http://www.apache.org/licenses/LICENSE-2.0
 *    
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *    
 * ************************************************************/
#endregion