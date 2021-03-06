﻿//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

// ReSharper disable CheckNamespace

using JetBrains.Annotations;

namespace System.Collections
// ReSharper restore CheckNamespace
{
	public static class ArrayListExtensions
	{
		[Pure]
		public static bool Any([NotNull] this ArrayList collection)
		{
			return collection.Count > 0;
		}
	}
}