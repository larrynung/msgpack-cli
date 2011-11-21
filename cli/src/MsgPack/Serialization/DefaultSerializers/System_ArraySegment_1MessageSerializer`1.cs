﻿#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2010 FUJIWARA, Yusuke
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
#endregion -- License Terms --

using System;

namespace MsgPack.Serialization.DefaultSerializers
{
	internal class System_ArraySegment_1MessageSerializer<T> : MessagePackSerializer<ArraySegment<T>>
	{
		private static readonly Action<Packer, ArraySegment<T>, SerializationContext> _packing;
		private static readonly Func<Unpacker, SerializationContext, ArraySegment<T>> _unpacking;

		static System_ArraySegment_1MessageSerializer()
		{
			if ( typeof( T ) == typeof( byte ) )
			{
				_packing =
					Delegate.CreateDelegate(
						typeof( Action<Packer, ArraySegment<T>, SerializationContext> ),
						ArraySegmentMessageSerializer.PackByteArraySegmentToMethod
					) as Action<Packer, ArraySegment<T>, SerializationContext>;
				_unpacking =
					Delegate.CreateDelegate(
						typeof( Func<Unpacker, SerializationContext, ArraySegment<T>> ),
						ArraySegmentMessageSerializer.UnpackByteArraySegmentFromMethod
					) as Func<Unpacker, SerializationContext, ArraySegment<T>>;
			}
			else if ( typeof( T ) == typeof( char ) )
			{
				_packing = Delegate.CreateDelegate(
					typeof( Action<Packer, ArraySegment<T>, SerializationContext> ),
					ArraySegmentMessageSerializer.PackCharArraySegmentToMethod
				) as Action<Packer, ArraySegment<T>, SerializationContext>;
				_unpacking = Delegate.CreateDelegate(
					typeof( Func<Unpacker, SerializationContext, ArraySegment<T>> ),
					ArraySegmentMessageSerializer.UnpackCharArraySegmentFromMethod
				) as Func<Unpacker, SerializationContext, ArraySegment<T>>;
			}
			else
			{
				_packing =
					Delegate.CreateDelegate(
						typeof( Action<Packer, ArraySegment<T>, SerializationContext> ),
						ArraySegmentMessageSerializer.PackGenericArraySegmentTo1Method.MakeGenericMethod( typeof( T ) )
					) as Action<Packer, ArraySegment<T>, SerializationContext>;
				_unpacking =
					Delegate.CreateDelegate(
						typeof( Func<Unpacker, SerializationContext, ArraySegment<T>> ),
						ArraySegmentMessageSerializer.UnpackGenericArraySegmentFrom1Method.MakeGenericMethod( typeof( T ) )
					) as Func<Unpacker, SerializationContext, ArraySegment<T>>;
			}
		}

		private readonly SerializationContext _context;

		public System_ArraySegment_1MessageSerializer()
			: this( null, null ) { }

		public System_ArraySegment_1MessageSerializer( MarshalerRepository marshalers, SerializerRepository serializers )
		{
			this._context = new SerializationContext( marshalers ?? MarshalerRepository.Default, serializers ?? SerializerRepository.Default );
		}

		protected sealed override void PackToCore( Packer packer, ArraySegment<T> objectTree )
		{
			_packing( packer, objectTree, this._context );
		}

		protected sealed override ArraySegment<T> UnpackFromCore( Unpacker unpacker )
		{
			return _unpacking( unpacker, this._context );
		}
	}
}