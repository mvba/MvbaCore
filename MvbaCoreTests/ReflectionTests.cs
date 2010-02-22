using System;
using System.Linq;
using System.Linq.Expressions;

using FluentAssert;

using MvbaCore;

using NUnit.Framework;

using Rhino.Mocks;

namespace MvbaCoreTests
{
	public class ReflectionTests
	{
		[TestFixture]
		public class When_asked_to_get_method_call_data
		{
			[Test]
			public void Should_get_the_correct_class_name()
			{
				var methodCallData = Reflection.GetMethodCallData((TestCalculator c) => c.Add(6));
				methodCallData.ClassName.ShouldBeEqualTo("TestCalculator");
			}

			[Test]
			public void Should_get_the_correct_method_name()
			{
				var methodCallData = Reflection.GetMethodCallData((TestCalculator c) => c.Add(6));
				methodCallData.MethodName.ShouldBeEqualTo("Add");
			}

			[Test]
			public void Should_get_the_correct_parameter_names()
			{
				var methodCallData = Reflection.GetMethodCallData((TestCalculator c) => c.Add(6));
				methodCallData.ParameterValues.Count.ShouldBeEqualTo(1);
				methodCallData.ParameterValues.Keys.First().ShouldBeEqualTo("addend");
			}

			[Test]
			public void Should_get_the_correct_parameter_values()
			{
				const int expected = 6;
				var methodCallData = Reflection.GetMethodCallData((TestCalculator c) => c.Add(expected));
				methodCallData.ParameterValues.Count.ShouldBeEqualTo(1);
				methodCallData.ParameterValues.Values.First().ShouldBeEqualTo(expected.ToString());
			}

			private class Address
			{
				public string City { get; private set; }

				public static class BoundPropertyNames
				{
					public static string City
					{
						get { return "City"; }
					}
				}
			}

			private class Bar
			{
				public Office Office { get; private set; }

				public static class BoundPropertyNames
				{
					public static string Office
					{
						get { return "Office"; }
					}
				}
			}

			private class Foo
			{
				public Bar PrimaryBar { get; private set; }

				public static class BoundPropertyNames
				{
					public static string PrimaryBar
					{
						get { return "PrimaryBar"; }
					}
				}
			}

			private class Office
			{
				public Address MailingAddress { get; private set; }

				public static class BoundPropertyNames
				{
					public static string MailingAddress
					{
						get { return "MailingAddress"; }
					}
				}
			}

			public class TestCalculator
			{
				private int _total;

				public int Add(int addend)
				{
					_total += addend;
					return _total;
				}
			}

			[TestFixture]
			public class When_asked_for_a_camel_case_property
			{
				[Test]
				public void Should_get_the_name_using_a_parameterized_lambda_to_a_property_multi_level()
				{
					string fullPropertyName = Reflection.GetCamelCaseMultiLevelPropertyName(Foo.BoundPropertyNames.PrimaryBar,
					                                                                        Bar.BoundPropertyNames.Office);
					Assert.AreEqual(Foo.BoundPropertyNames.PrimaryBar.ToCamelCase() + "." + Bar.BoundPropertyNames.Office,
					                fullPropertyName);
				}
			}

			[TestFixture]
			public class When_asked_for_a_formatted_multilevel_property
			{
				[Test]
				public void Should_give_the_full_name_of_the_property()
				{
					string fullPropertyName = Reflection.GetCamelCaseMultiLevelPropertyName(Foo.BoundPropertyNames.PrimaryBar,
					                                                                        Bar.BoundPropertyNames.Office);
					Assert.AreEqual(Foo.BoundPropertyNames.PrimaryBar.ToCamelCase() + "." + Bar.BoundPropertyNames.Office,
					                fullPropertyName);
				}
			}

			[TestFixture]
			public class When_asked_for_a_multilevel_property_with_strings
			{
				[Test]
				public void Should_give_the_full_name_of_the_property()
				{
					string fullPropertyName = Reflection.GetMultiLevelPropertyName(Foo.BoundPropertyNames.PrimaryBar,
					                                                               Bar.BoundPropertyNames.Office);
					Assert.AreEqual(Foo.BoundPropertyNames.PrimaryBar + "." + Bar.BoundPropertyNames.Office, fullPropertyName);
				}
			}

			[TestFixture]
			public class When_asked_to_get_the_name_of_a_method
			{
				[Test]
				public void Should_get_the_name_using_a_parameterized_lambda_to_a_method()
				{
					string name = Reflection.GetMethodName((ISample s) => s.TheProperty());
					Assert.AreEqual("TheProperty", name);
				}

				private interface ISample
				{
					int TheProperty();
				}
			}

			[TestFixture]
			public class When_asked_to_get_the_name_of_a_property
			{
				[Test]
				public void Should_be_able_to_get_the_name_if_it_is_being_boxed()
				{
					Expression<Func<TestObject, object>> getter = t => t.Id;
					string fullPropertyName = Reflection.GetPropertyName(getter);
					fullPropertyName.ShouldBeEqualTo("Id");
				}

				[Test]
				public void Should_get_the_name_using_a_parameterized_lambda_to_a_property_multi_level()
				{
					string fullPropertyName =
						Reflection.GetPropertyName((Foo jurisdiction) => jurisdiction.PrimaryBar.Office.MailingAddress.City);
					Assert.AreEqual(
						Foo.BoundPropertyNames.PrimaryBar + "." + Bar.BoundPropertyNames.Office + "." +
						Office.BoundPropertyNames.MailingAddress + "." + Address.BoundPropertyNames.City, fullPropertyName);
				}

				[Test]
				public void Should_get_the_name_using_a_parameterized_lambda_to_a_property_single_level()
				{
					string name = Reflection.GetPropertyName((ISample s) => s.TheProperty);
					Assert.AreEqual("TheProperty", name);
				}

				[Test]
				public void Should_get_the_name_using_a_parameterless_lambda_to_a_property_multi_level()
				{
					var sample = new Foo();
					string name = Reflection.GetPropertyName(() => sample.PrimaryBar.Office.MailingAddress.City);
					Assert.AreEqual(Foo.BoundPropertyNames.PrimaryBar + "." + Bar.BoundPropertyNames.Office + "." +
					                Office.BoundPropertyNames.MailingAddress + "." + Address.BoundPropertyNames.City, name);
				}

				[Test]
				public void Should_get_the_name_using_a_parameterless_lambda_to_a_property_multi_level_null_input()
				{
// ReSharper disable ConvertToConstant.Local
					Foo sample = null;
// ReSharper restore ConvertToConstant.Local
					string name = Reflection.GetPropertyName(() => sample.PrimaryBar.Office.MailingAddress.City);
					Assert.AreEqual(Foo.BoundPropertyNames.PrimaryBar + "." + Bar.BoundPropertyNames.Office + "." +
					                Office.BoundPropertyNames.MailingAddress + "." + Address.BoundPropertyNames.City, name);
				}

				[Test]
				public void Should_get_the_name_using_a_parameterless_lambda_to_a_property_single_level()
				{
					var sample = MockRepository.GenerateStub<ISample>();
					string name = Reflection.GetPropertyName(() => sample.TheProperty);
					Assert.AreEqual("TheProperty", name);
				}

				[Test]
				public void Should_get_the_name_using_a_parameterless_lambda_to_a_property_single_level_null_input()
				{
					const ISample sample = null;
					string name = Reflection.GetPropertyName(() => sample.TheProperty);
					Assert.AreEqual("TheProperty", name);
				}

				public interface ISample
				{
					int TheProperty { get; }
				}

				public class TestObject
				{
					public int Id { get; set; }
				}
			}
		}

		[TestFixture]
		public class When_asked_to_get_the_value_of_an_expression
		{
			[Test]
			public void Should_be_able_to_get_the_value_if_it_is_a_constant()
			{
				Expression<Func<int>> expr = () => TestClass.Id;
				Reflection.GetValueAsString(expr.Body).ShouldBeEqualTo(TestClass.Id.ToString());
			}

			[Test]
			public void Should_be_able_to_get_the_value_if_it_is_a_property()
			{
				var testClass = new TestClass();
				Expression<Func<int>> expr = () => testClass.MyId;
				Reflection.GetValueAsString(expr.Body).ShouldBeEqualTo(TestClass.Id.ToString());
			}

			[Test]
			public void Should_be_able_to_get_the_value_if_it_is_a_static_method()
			{
				Expression<Func<int>> expr = () => TestClass.GetId();
				Reflection.GetValueAsString(expr.Body).ShouldBeEqualTo(TestClass.Id.ToString());
			}

			public class TestClass
			{
				public const int Id = 1234;

				public int MyId
				{
					get { return Id; }
				}

				public static int GetId()
				{
					return Id;
				}
			}
		}
	}
}