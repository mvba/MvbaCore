using MvbaCore;

using NUnit.Framework;

namespace MvbaCoreTests
{
	public partial class NotificationTest
	{
		[TestFixture]
		public class When_asked_if_IsValid
		{
			[Test]
			public void Should_return_false_if_Messages_contains_only_messages_with_Error_Severity()
			{
				Notification notification = new Notification();
				NotificationMessage messageTest = new NotificationMessage(NotificationSeverity.Error, "");
				notification.Add(messageTest);

				Assert.IsFalse(notification.IsValid);
			}

			[Test]
			public void Should_return_false_if_Messages_contains_only_messages_with_Warning_Severity()
			{
				Notification notification = new Notification();
				NotificationMessage messageTest = new NotificationMessage(NotificationSeverity.Warning, "");
				notification.Add(messageTest);

				Assert.IsFalse(notification.IsValid);
			}

			[Test]
			public void Should_return_true_if_Messages_contains_only_messages_with_Info_Severity()
			{
				Notification notification = new Notification();
				NotificationMessage messageTest = new NotificationMessage(NotificationSeverity.Info, "");
				notification.Add(messageTest);
				Assert.IsTrue(notification.IsValid);
			}

			[Test]
			public void Should_return_true_if_Messages_is_empty()
			{
				Notification notification = new Notification();
				Assert.IsTrue(notification.IsValid);
			}
		}
	}
}