﻿//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System.Collections.Generic;

namespace MvbaCore.FileSystem
{
	public interface IPipeDelimitedDataConverter
	{
		IEnumerable<Dictionary<string, string>> Convert(IEnumerable<string> lines);
	}

	public class PipeDelimitedDataConverter : DelimitedDataConverter, IPipeDelimitedDataConverter
	{
		public IEnumerable<Dictionary<string, string>> Convert(IEnumerable<string> lines)
		{
			var result = Convert(lines, "|");
			return result;
		}
	}
}