using System.Linq;

using FluentAssert;

using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public partial class NotificationTest
	{
		[TestFixture]
		public class When_asked_for_a_Notification_that_has_an_initial_message_with_Severity_of_Warning
		{
			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_the_given_messageText()
			{
				const string messageText = "text";
				Notification notification = Notification.WarningFor(messageText);

				notification.Messages.Count.ShouldBeEqualTo(1);
				notification.Messages.First().Message.ShouldBeEqualTo(messageText);
			}

			[Test]
			public void Should_return_a_Notification_with_a_message_that_has_Warning_Severity()
			{
				Notification notification = Notification.WarningFor("text");

				notification.Messages.Count.ShouldBeEqualTo(1);
				notification.Messages.First().Severity.ShouldBeEqualTo(NotificationSeverity.Warning);
			}
		}
	}
}