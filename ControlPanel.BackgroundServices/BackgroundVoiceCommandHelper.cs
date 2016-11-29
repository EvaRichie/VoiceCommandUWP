using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;

namespace ControlPanel.BackgroundServices
{
    public sealed class BackgroundVoiceCommandHelper
    {
        public static IAsyncAction ReportProgressAsync(VoiceCommandServiceConnection connection, string spokenMessage, string displayMessage)
        {
            return AsyncReportProgress(connection, spokenMessage, displayMessage).AsAsyncAction();
        }

        private static async Task AsyncReportProgress(VoiceCommandServiceConnection connection, string spokenMessage, string displayMessage)
        {
            var responseMsg = new VoiceCommandUserMessage { SpokenMessage = spokenMessage, DisplayMessage = displayMessage };
            var response = VoiceCommandResponse.CreateResponse(responseMsg);
            await connection.ReportProgressAsync(response);
        }

        public static IAsyncAction ReportSuccessAsync(VoiceCommandServiceConnection connection, string spokenMessage, string displayMessage, IEnumerable<VoiceCommandContentTile> contentTiles)
        {
            return AsyncReportSuccess(connection, spokenMessage, displayMessage, contentTiles).AsAsyncAction();
        }

        private static async Task AsyncReportSuccess(VoiceCommandServiceConnection connection, string spokenMessage, string displayMessage, IEnumerable<VoiceCommandContentTile> contentTiles)
        {
            var responseMsg = new VoiceCommandUserMessage { SpokenMessage = spokenMessage, DisplayMessage = displayMessage };
            var response = VoiceCommandResponse.CreateResponse(responseMsg, contentTiles);
            await connection.ReportSuccessAsync(response);
        }
    }
}
