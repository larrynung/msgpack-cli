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
using MsgPack.Serialization.Reflection;

namespace MsgPack.Serialization.EmittingSerializers
{
	/// <summary>
	///		<see cref="EmittingSerializerBuilder{T}"/> implementation which emits as array (default).
	/// </summary>
	/// <typeparam name="TObject">The type of the target object.</typeparam>
	internal sealed class ArrayEmittingSerializerBuilder<TObject> : EmittingSerializerBuilder<TObject>
	{
		public ArrayEmittingSerializerBuilder( SerializationContext context )
			: base( context ) { }

		protected override void EmitPackMembers( SerializerEmitter emitter, TracingILGenerator packerIL, SerializingMember[] entries )
		{
			packerIL.EmitAnyLdarg( 1 );
			packerIL.EmitAnyLdc_I4( entries.Length );
			packerIL.EmitAnyCall( Metadata._Packer.PackArrayHeader );
			packerIL.EmitPop();

			foreach ( var member in entries )
			{
				if ( member.Member == null )
				{
					// missing member, always nil
					packerIL.EmitAnyLdarg( 1 );
					packerIL.EmitAnyCall( Metadata._Packer.PackNull );
					packerIL.EmitPop();
				}
				else
				{
					Emittion.EmitSerializeValue(
						emitter,
						packerIL,
						1,
						member.Member.GetMemberValueType(),
						member.Member.Name,
						member.Contract.NilImplication,
						il =>
						{
							if ( typeof( TObject ).IsValueType )
							{
								il.EmitAnyLdarga( 2 );
							}
							else
							{
								il.EmitAnyLdarg( 2 );
							}

							Emittion.EmitLoadValue( il, member.Member );
						}
					);
				}
			}

			packerIL.EmitRet();
		}
	}

}
