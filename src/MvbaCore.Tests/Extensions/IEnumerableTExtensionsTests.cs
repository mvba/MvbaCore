//   * **************************************************************************
//   * Copyright (c) McCreary, Veselka, Bragg & Allen, P.C.
//   * This source code is subject to terms and conditions of the MIT License.
//   * A copy of the license can be found in the License.txt file
//   * at the root of this distribution.
//   * By using this source code in any fashion, you are agreeing to be bound by
//   * the terms of the MIT License.
//   * You must not remove this notice from this software.
//   * **************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FluentAssert;

using JetBrains.Annotations;

using NUnit.Framework;

namespace MvbaCore.Tests.Extensions
{
	[UsedImplicitly]
	public class IEnumerableTExtensionsTest
	{
		public interface ITestItem
		{
			int KeyId { get; set; }
			string Name { get; set; }
		}

		public class OtherItem : ITestItem
		{
			public int KeyId { get; set; }
			public string Name { get; set; }
		}

		public abstract class SelectWhereNotInOtherTestBase<TListAType, TListBType>
			where TListAType : ITestItem, new()
			where TListBType : ITestItem, new()
		{
			public abstract IEnumerable<TListAType> CallExtension(IEnumerable<TListAType> listA, IEnumerable<TListBType> listB);

			[Test]
			public void Should_return_empty_if_the_list_being_selected_from_is_empty()
			{
				var listA = new List<TListAType>();
				var listB = new List<TListBType>
					            {
						            new TListBType()
					            };

				var result = CallExtension(listA, listB);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Any());
			}

			[Test]
			public void Should_return_only_the_items_in_list_being_selected_from_that_are_not_in_the_other_list()
			{
				var itemA1 = new TListAType
					             {
						             KeyId = 1,
						             Name = "A"
					             };
				var itemA2 = new TListAType
					             {
						             KeyId = 2,
						             Name = "A & B"
					             };
				var itemA3 = new TListAType
					             {
						             KeyId = 3,
						             Name = "A & B"
					             };
				var itemB2 = new TListBType
					             {
						             KeyId = 2,
						             Name = "A & B"
					             };
				var itemB3 = new TListBType
					             {
						             KeyId = 3,
						             Name = "A & B"
					             };
				var itemB4 = new TListBType
					             {
						             KeyId = 4,
						             Name = "B"
					             };
				var listA = new List<TListAType>
					            {
						            itemA1,
						            itemA2,
						            itemA3
					            };
				var listB = new List<TListBType>
					            {
						            itemB2,
						            itemB3,
						            itemB4
					            };

				var result = CallExtension(listA, listB).ToList();
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Any());
				Assert.IsTrue(result.All(item => item.Name == "A"));
			}

			[Test]
			public void Should_return_the_list_being_selected_from_if_the_other_list_is_empty()
			{
				var listA = new List<TListAType>
					            {
						            new TListAType()
					            };
				var listB = new List<TListBType>();

				var result = CallExtension(listA, listB);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Any());
			}

			[Test]
			public void Should_return_the_list_being_selected_from_if_the_other_list_is_null()
			{
				var listA = new List<TListAType>
					            {
						            new TListAType()
					            };
				const List<TListBType> listB = null;

				var result = CallExtension(listA, listB);
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Any());
			}

			[Test]
			public void Should_throw_an_exception_if_the_list_being_selected_from_is_null()
			{
				const List<TListAType> listA = null;
				var listB = new List<TListBType>();

				Assert.Throws<ArgumentNullException>(() => CallExtension(listA, listB));
			}
		}

		public class TestItem : ITestItem
		{
			public int KeyId { get; set; }
			public string Name { get; set; }
		}

		[TestFixture]
		public class When_asked_if_an_IEnumerable_T_IsNullOrEmpty
		{
			[Test]
			public void Should_return_false_if_the_input_contains_items()
			{
				IList<int> input = new List<int>
					                   {
						                   6
					                   };
				input.IsNullOrEmpty().ShouldBeFalse();
			}

			[Test]
			public void Should_return_true_if_the_input_is_empty()
			{
				IList<int> input = new List<int>();
				input.IsNullOrEmpty().ShouldBeTrue();
			}

			[Test]
			public void Should_return_true_if_the_input_is_null()
			{
				const IList<int> input = null;
				input.IsNullOrEmpty().ShouldBeTrue();
			}
		}

		[TestFixture]
		public class When_asked_to_SelectWhereInOther
		{
			[Test]
			public void Should_return_empty_if_the_list_being_selected_from_is_empty()
			{
				var itemA = new List<TestItem>();
				var itemB = new List<TestItem>
					            {
						            new TestItem()
					            };

				var result = itemA.Intersect(itemB, a => a.KeyId);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Any());
			}

			[Test]
			public void Should_return_empty_if_the_other_list_is_empty()
			{
				var itemA = new List<TestItem>
					            {
						            new TestItem()
					            };
				var itemB = new List<TestItem>();

				var result = itemA.Intersect(itemB, a => a.KeyId);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Any());
			}

			[Test]
			public void Should_return_empty_if_the_other_list_is_null()
			{
				var itemA = new List<TestItem>
					            {
						            new TestItem()
					            };
				const List<TestItem> itemB = null;

				var result = itemA.Intersect(itemB, a => a.KeyId);
				Assert.IsNotNull(result);
				Assert.IsFalse(result.Any());
			}

			[Test]
			public void Should_return_only_the_items_in_list_being_selected_from_that_are_also_in_the_other_list()
			{
				var item1 = new TestItem
					            {
						            KeyId = 1,
						            Name = "A"
					            };
				var item2 = new TestItem
					            {
						            KeyId = 2,
						            Name = "A & B"
					            };
				var item3 = new TestItem
					            {
						            KeyId = 3,
						            Name = "A & B"
					            };
				var item4 = new TestItem
					            {
						            KeyId = 4,
						            Name = "B"
					            };
				var itemA = new List<TestItem>
					            {
						            item1,
						            item2,
						            item3
					            };
				var itemB = new List<TestItem>
					            {
						            item2,
						            item3,
						            item4
					            };

				var result = itemA.Intersect(itemB, a => a.KeyId).ToList();
				Assert.IsNotNull(result);
				Assert.IsTrue(result.Any());
				Assert.IsTrue(result.All(item => item.Name == "A & B"));
			}

			[Test]
			public void Should_throw_an_exception_if_the_list_being_selected_from_is_null()
			{
				const List<TestItem> itemA = null;
				var itemB = new List<TestItem>();
				Assert.Throws<ArgumentNullException>(() => itemA.Intersect(itemB, a => a.KeyId));
			}
		}

		[TestFixture]
		public class When_asked_to_SelectWhereNotInOther_where_T_is_class : SelectWhereNotInOtherTestBase<TestItem, TestItem>
		{
			public override IEnumerable<TestItem> CallExtension(IEnumerable<TestItem> listA, IEnumerable<TestItem> listB)
			{
				return listA.Except(listB, a => a.KeyId);
			}
		}

		[TestFixture]
		public class When_asked_to_SelectWhereNotInOther_where_lists_contain_different_types :
			SelectWhereNotInOtherTestBase<TestItem, OtherItem>
		{
			public override IEnumerable<TestItem> CallExtension(IEnumerable<TestItem> listA, IEnumerable<OtherItem> listB)
			{
				return listA.Except(listB, a => a.KeyId, b => b.KeyId, b => b == null);
			}
		}

		[TestFixture]
		public class When_asked_to_SelectWhereNotInOther_where_lists_contain_the_same_type :
			SelectWhereNotInOtherTestBase<TestItem, TestItem>
		{
			public override IEnumerable<TestItem> CallExtension(IEnumerable<TestItem> listA, IEnumerable<TestItem> listB)
			{
				return listA.Except(listB, a => a.KeyId, a => a == null);
			}
		}

		[TestFixture]
		public class When_asked_to_convert_an_enumerable_list_of_items_to_page_sets
		{
			private int _firstPageSize;
			private List<int> _listOfItems;
			private int _nthPageSize;

			[SetUp]
			public void SetUp()
			{
				_listOfItems = new List<int>();
				_firstPageSize = 3;
				_nthPageSize = 5;
			}

			[Test]
			public void Should_return_a_set_of_pages()
			{
				var pageSets = _listOfItems.ToPageSets(_firstPageSize, _nthPageSize);
				pageSets.ShouldNotBeNull();
			}

			[Test]
			public void Should_return_an_empty_set_of_pages_when_the_list_is_null()
			{
				_listOfItems = null;
				var pageSets = _listOfItems.ToPageSets(_firstPageSize, _nthPageSize);
				pageSets.ShouldNotBeNull();
			}

			[Test]
			public void Should_return_multiple_pages_when_the_list_is_longer_than_the__firstPageSize()
			{
				_listOfItems = new List<int>
					               {
						               1,
						               2,
						               3,
						               4,
						               5,
						               6,
						               7,
						               8,
						               9
					               };

				var pageSets = _listOfItems.ToPageSets(_firstPageSize, _nthPageSize);
				pageSets.Count().ShouldBeGreaterThan(1);
			}

			[Test]
			public void Should_return_multiple_pages_with_the_last_page_size_equal_to___nthPageSize()
			{
				_listOfItems = new List<int>
					               {
						               1,
						               2,
						               3,
						               4,
						               5,
						               6,
						               7,
						               8
					               };

				var pageSets = _listOfItems.ToPageSets(_firstPageSize, _nthPageSize);
				var last = pageSets.Last().ToList();
				last.Count.ShouldBeEqualTo(_nthPageSize);
				last[0].ShouldBeEqualTo(_listOfItems[3]);
				last[1].ShouldBeEqualTo(_listOfItems[4]);
				last[2].ShouldBeEqualTo(_listOfItems[5]);
				last[3].ShouldBeEqualTo(_listOfItems[6]);
				last[4].ShouldBeEqualTo(_listOfItems[7]);
			}

			[Test]
			public void Should_return_multiple_pages_with_the_last_page_size_equal_to_the_remaining_elements_in_the_list()
			{
				_listOfItems = new List<int>
					               {
						               1,
						               2,
						               3,
						               4,
						               5,
						               6,
						               7,
						               8,
						               9,
						               10
					               };

				var pageSets = _listOfItems.ToPageSets(_firstPageSize, _nthPageSize);
				pageSets.Last().Count().ShouldBeEqualTo(_listOfItems.Count() - (_firstPageSize + _nthPageSize));
			}

			[Test]
			public void Should_return_multiple_pages_with_the_pages_other_than_first_and_last_page_equal_to__nthPageSize()
			{
				_listOfItems = new List<int>
					               {
						               1,
						               2,
						               3,
						               4,
						               5,
						               6,
						               7,
						               8,
						               9,
						               10
					               };

				var pageSets = _listOfItems.ToPageSets(_firstPageSize, _nthPageSize);
				pageSets.Skip(1).First().Count().ShouldBeEqualTo(_nthPageSize);
			}

			[Test]
			public void Should_return_one_page_if_the_length_of_the_list_is_equal_to__firstPageSize()
			{
				_listOfItems = new List<int>
					               {
						               1,
						               2,
						               3
					               };
				var pageSets = _listOfItems.ToPageSets(_firstPageSize, _nthPageSize).ToList();
				pageSets.First().Count.ShouldBeEqualTo(_firstPageSize);
				var first = pageSets.First().ToList();
				first.Count.ShouldBeEqualTo(_firstPageSize);
				first[0].ShouldBeEqualTo(_listOfItems[0]);
				first[1].ShouldBeEqualTo(_listOfItems[1]);
				first[2].ShouldBeEqualTo(_listOfItems[2]);
			}

			[Test]
			public void Should_return_one_page_if_the_length_of_the_list_is_less_than__firstPageSize()
			{
				_listOfItems = new List<int>
					               {
						               1,
						               2
					               };
				var pageSets = _listOfItems.ToPageSets(_firstPageSize, _nthPageSize);
				pageSets.First().Count.ShouldBeEqualTo(_listOfItems.Count());
			}
		}

		[TestFixture]
		public class When_asked_to_flatten_ranges
		{
			private List<Range<string>> _ranges;
			private List<KeyValuePair<int, string>> _result;

			[Test]
			public void Given_a_list_with_1_range_having_end_equal_to_start_plus_1_should_return_2_results()
			{
				Test.Verify(
					with_a_list_of_ranges,
					that_has_one_range,
					having_end_equal_to_start_plus_1,
					when_asked_to_flatten,
					should_return_2_result,
					first_result_should_have_key_equal_to_range_start,
					last_result_should_have_key_equal_to_range_end,
					all_results_should_have_value_equal_to_range_payload
					);
			}

			[Test]
			public void Given_a_list_with_1_range_having_end_equal_to_start_should_return_1_result()
			{
				Test.Verify(
					with_a_list_of_ranges,
					that_has_one_range,
					having_end_equal_to_start,
					when_asked_to_flatten,
					should_return_1_result,
					first_result_should_have_key_equal_to_range_start,
					all_results_should_have_value_equal_to_range_payload
					);
			}

			[Test]
			public void Given_an_empty_list_of_ranges_should_return_empty_result()
			{
				Test.Verify(
					with_a_list_of_ranges,
					that_is_empty,
					when_asked_to_flatten,
					should_return_empty_result
					);
			}

			private void all_results_should_have_value_equal_to_range_payload()
			{
				_result.All(x => x.Value == _ranges.Last().Payload).ShouldBeTrue();
			}

			private void first_result_should_have_key_equal_to_range_start()
			{
				_result.First().Key.ShouldBeEqualTo(_ranges.Last().Start);
			}

			private void having_end_equal_to_start()
			{
				var range = _ranges.Last();
				range.Start = 23;
				range.End = range.Start;
			}

			private void having_end_equal_to_start_plus_1()
			{
				var range = _ranges.First();
				range.Start = 42;
				range.End = range.Start + 1;
			}

			private void last_result_should_have_key_equal_to_range_end()
			{
				_result.Last().Key.ShouldBeEqualTo(_ranges.First().End);
			}

			private void should_return_1_result()
			{
				_result.Count.ShouldBeEqualTo(1);
			}

			private void should_return_2_result()
			{
				_result.Count.ShouldBeEqualTo(2);
			}

			private void should_return_empty_result()
			{
				_result.ShouldBeEmpty();
			}

			private void that_has_one_range()
			{
				_ranges.Add(new Range<string>
					            {
						            Payload = _ranges.Count.ToString()
					            });
			}

			private void that_is_empty()
			{
				_ranges.Clear();
			}

			private void when_asked_to_flatten()
			{
				_result = _ranges.FlattenRanges().ToList();
			}

			private void with_a_list_of_ranges()
			{
				_ranges = new List<Range<string>>();
			}
		}

		[TestFixture]
		public class When_asked_to_group_a_list
		{
			[Test]
			public void Given_same_items_together_should_be_grouped__should_group_the_items()
			{
				var input = new[] { "a", "b", "b", "b", "c", "c", "d" };
				var grouped = input.Group((current, previous) => current == previous);
				var sb = new StringBuilder();
				foreach (var @group in grouped)
				{
					sb.Append(String.Join(", ", @group));
					sb.Append('|');
				}
				sb.ToString().ShouldBeEqualTo(@"a|b, b, b|c, c|d|");
			}

			[Test]
			public void Given_items_with_a_header_should_be_grouped__should_group_the_items()
			{
				var input = new[] { "a", "b", "b", "a", "c", "c", "a" };
				var grouped = input.Group((current, previous) => current == previous || current != "a");
				var sb = new StringBuilder();
				foreach (var @group in grouped)
				{
					sb.Append(String.Join(", ", @group));
					sb.Append('|');
				}
				sb.ToString().ShouldBeEqualTo(@"a, b, b|a, c, c|a|");
			}
		}

		[TestFixture]
		public class When_asked_to_join_an_enumerable_list_of_items
		{
			[Test]
			public void Should_return_a_string_containing_the_list_items_separated_by_the_delimiter()
			{
				const int one = 1;
				const int item = 3;
				var items = new List<int>
					            {
						            one,
						            item
					            };
				const string delimiter = "','";
				var expect = one + delimiter + item;

				Assert.AreEqual(expect, items.Join(delimiter));
			}

			[Test]
			public void Should_return_an_empty_string_if_the_list_is_empty()
			{
				var items = new List<string>();
				Assert.IsEmpty(items.Join("x"));
			}

			[Test]
			public void Should_return_an_empty_string_if_the_list_is_null()
			{
				const List<string> items = null;
				Assert.IsEmpty(items.Join("x"));
			}

			[Test]
			public void Should_use_empty_string_if_the_delimiter_is_null()
			{
				const int one = 1;
				const int item = 3;
				var items = new List<int>
					            {
						            one,
						            item
					            };
				const string delimiter = null;
				var expect = one + "" + item;

				Assert.AreEqual(expect, items.Join(delimiter));
			}
		}

		[TestFixture]
		public class When_asked_to_return_items_in_sets
		{
			[Test]
			public void Should_fill_empty_spots_in_the_last_set_with_the_default_value_if_fill_is_requested()
			{
				var input = "abcdefghijklm".Select(x => x.ToString()).ToList();
				var result = input.InSetsOf(5, true, "x").ToList();
				result.Count.ShouldBeEqualTo(3);
				result.First().Count.ShouldBeEqualTo(5);
				var last = result.Last();
				last.Count.ShouldBeEqualTo(5);
				last.Join("").ShouldBeEqualTo("klmxx");
			}

			[Test]
			public void Should_not_fill_the_last_set_if_fill_is_not_requested()
			{
				var input = "abcdefghijkm".Select(x => x.ToString()).ToList();
				var result = input.InSetsOf(5, false, "x").ToList();
				result.Count.ShouldBeEqualTo(3);
				result.First().Count.ShouldBeEqualTo(5);
				result.Last().Count.ShouldBeEqualTo(2);
			}

			[Test]
			public void Should_return_the_correct_number_of_sets_if_the_input_contains_a_multiple_of_the_setSize()
			{
				var input = "abcdefghij".Select(x => x.ToString()).ToList();
				var result = input.InSetsOf(5).ToList();
				result.Count().ShouldBeEqualTo(2);
				result.First().Count.ShouldBeEqualTo(5);
				result.Last().Count.ShouldBeEqualTo(5);
			}

			[Test]
			public void Should_separate_the_input_into_sets_of_size_requested()
			{
				var input = "abcdefghijklm".Select(x => x.ToString()).ToList();
				var result = input.InSetsOf(5).ToList();
				result.Count.ShouldBeEqualTo(3);
				result.First().Count.ShouldBeEqualTo(5);
				result.Last().Count.ShouldBeEqualTo(3);
			}
		}
	}
}