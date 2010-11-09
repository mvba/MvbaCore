//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using System.Linq;

using CodeQuery;

using JetBrains.Annotations;

namespace MvbaCore.Extensions
{
	public static class NamedConstantExtensions
	{
		[CanBeNull]
		public static T DefaultValue<T>() where T : NamedConstant<T>
		{
			var fields = typeof(T).GetFields().ThatAreStatic();
			var defaultField = fields.WithAttributeOfType<DefaultKeyAttribute>().FirstOrDefault();
			if (defaultField == null)
			{
				return null;
			}
// ReSharper disable AssignNullToNotNullAttribute
			return (T)defaultField.GetValue(null);
// ReSharper restore AssignNullToNotNullAttribute
		}

		[CanBeNull]
		public static T OrDefault<T>([CanBeNull] this T value) where T : NamedConstant<T>
		{
			if (value != null)
			{
				return value;
			}
			return DefaultValue<T>();
		}
	}
}