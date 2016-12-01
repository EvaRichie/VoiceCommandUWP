using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;

namespace ControlPanel.Library.Helpers
{
    public class PushNotificationHelper
    {
        public static IAsyncAction RegisterChannel(User user)
        {
            return AsyncRergisterChannel(user).AsAsyncAction();
        }

        private static async Task AsyncRergisterChannel(User user)
        {
            try
            {
                var channel = await Windows.Networking.PushNotifications.PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                channel.PushNotificationReceived += Channel_PushNotificationReceived;

            }
            catch (Exception ex)
            {

            }
        }

        private static void Channel_PushNotificationReceived(Windows.Networking.PushNotifications.PushNotificationChannel sender, Windows.Networking.PushNotifications.PushNotificationReceivedEventArgs args)
        {
            
        }
    }
}
