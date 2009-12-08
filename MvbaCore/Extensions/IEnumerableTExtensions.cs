using System.Linq;
using System.Text;

using JetBrains.Annotations;

namespace System.Collections.Generic
{
	public static class IEnumerableTExtensions
	{
		[NotNull]
		public static IEnumerable<T> ForEach<T>([NotNull] this IEnumerable<T> items, [NotNull] Action<T> action)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items", "collection cannot be null");
			}

			foreach (var item in items)
			{
				action(item);
			}
			return items;
		}

		[NotNull]
		public static IEnumerable<List<T>> InSetsOf<T>([NotNull] this IEnumerable<T> items, int setSize)
		{
			return items.InSetsOf(setSize, false, default(T));
		}

		[NotNull]
		public static IEnumerable<List<T>> InSetsOf<T>([NotNull] this IEnumerable<T> items, int setSize, bool fillPartialSetWithDefaultItems, T defaultItemToFillGroups)
		{
			var set = new List<T>();
			foreach (var item in items)
			{
				set.Add(item);
				if (set.Count == setSize)
				{
					yield return set.ToList();
					set.Clear();
				}
			}

			if (set.Count > 0)
			{
				if (fillPartialSetWithDefaultItems)
				{
					while (set.Count < setSize)
					{
						set.Add(defaultItemToFillGroups);
					}
				}
				yield return set;
			}
		}

		public static bool IsNullOrEmpty<T>([CanBeNull] this IEnumerable<T> list)
		{
			return list == null || !list.Any();
		}

		[NotNull]
		public static string Join<T>([CanBeNull] this IEnumerable<T> items, [CanBeNull] string delimiter)
		{
			StringBuilder result = new StringBuilder();
			if (items != null && items.Any())
			{
				delimiter = delimiter ?? "";
				foreach (var item in items)
				{
					result.Append(item);
					result.Append(delimiter);
				}
				result.Length = result.Length - delimiter.Length;
			}
			return result.ToString();
		}

		[NotNull]
		public static HashSet<T> ToHashSet<T>([NotNull] this IEnumerable<T> items)
		{
			return new HashSet<T>(items);
		}
	}
}