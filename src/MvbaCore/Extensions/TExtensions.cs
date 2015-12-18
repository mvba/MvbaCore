//  * **************************************************************************
//  * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution.
//  * By using this source code in any fashion, you are agreeing to be bound by
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **************************************************************************

using JetBrains.Annotations;

// ReSharper disable CheckNamespace
namespace System
// ReSharper restore CheckNamespace
{
	public static class TExtensions
	{
		[Pure]
		[NotNull]
		public static T ToNonNull<T>([CanBeNull] this T input) where T : class, new()
		{
			return ReferenceEquals(input, null) ? new T() : input;
		}

		[NotNull]
		[Pure]
		[ContractAnnotation("item:null => halt")]
		public static TDesiredType TryCastTo<TDesiredType>([NotNull] this object item) where TDesiredType : class
		{
			var desiredType = item as TDesiredType;
			if (ReferenceEquals(desiredType, null))
			{
				throw new InvalidOperationException("Cannot convert a " + item.GetType().Name + " to a " + typeof(TDesiredType).Name + ".");
			}
			return desiredType;
		}

	}
}