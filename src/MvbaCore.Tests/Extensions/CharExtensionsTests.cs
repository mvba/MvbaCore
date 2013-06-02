﻿//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.Globalization;

using JetBrains.Annotations;

using NUnit.Framework;

namespace MvbaCore.Tests.Extensions
{
	[UsedImplicitly]
	public class CharExtensionsTests
	{
		[TestFixture]
		public class When_asked_if_a_character_is_a_vowel
		{
			[Test]
			public void Should_return_false_if_it_is_not_a_vowel()
			{
				var input = 'X';
				Assert.IsFalse(input.IsVowel(), input.ToString(CultureInfo.InvariantCulture));

				input = 'y';
				Assert.IsFalse(input.IsVowel(), input.ToString(CultureInfo.InvariantCulture));
			}

			[Test]
			public void Should_return_true_if_it_is_a_vowel()
			{
				var input = 'A';
				Assert.IsTrue(input.IsVowel(), input.ToString(CultureInfo.InvariantCulture));

				input = 'e';
				Assert.IsTrue(input.IsVowel(), input.ToString(CultureInfo.InvariantCulture));

				input = 'I';
				Assert.IsTrue(input.IsVowel(), input.ToString(CultureInfo.InvariantCulture));

				input = 'o';
				Assert.IsTrue(input.IsVowel(), input.ToString(CultureInfo.InvariantCulture));

				input = 'U';
				Assert.IsTrue(input.IsVowel(), input.ToString(CultureInfo.InvariantCulture));
			}
		}
	}
}