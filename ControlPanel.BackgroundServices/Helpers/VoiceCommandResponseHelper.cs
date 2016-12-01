using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;

namespace ControlPanel.BackgroundServices.Helpers
{
    public sealed class VoiceCommandResponseHelper
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

        public static IAsyncOperation<VoiceCommandDisambiguationResult> RequestDisambiguationAsync(VoiceCommandServiceConnection connection, string promptMessage, string repromptMessage, IEnumerable<VoiceCommandContentTile> contentTiles)
        {
            return AsyncRequestDisambiguation(connection, promptMessage, repromptMessage, contentTiles).AsAsyncOperation();
        }

        private static async Task<VoiceCommandDisambiguationResult> AsyncRequestDisambiguation(VoiceCommandServiceConnection connection, string promptMessage, string repromptMessage, IEnumerable<VoiceCommandContentTile> contentTiles)
        {
            var prompt = new VoiceCommandUserMessage { DisplayMessage = promptMessage, SpokenMessage = promptMessage };
            var reprompt = new VoiceCommandUserMessage { DisplayMessage = repromptMessage, SpokenMessage = repromptMessage };
            var response = VoiceCommandResponse.CreateResponseForPrompt(prompt, reprompt, contentTiles);
            try
            {
                var result = await connection.RequestDisambiguationAsync(response);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static IAsyncOperation<VoiceCommandDisambiguationResult> RequestDisambiguationAsync(VoiceCommandServiceConnection connection, string promptSpokenMessage, string promptDisplayMessage, string repromptSpokenMessage, string repromptDisplayMessage, IEnumerable<VoiceCommandContentTile> contentTiles)
        {
            return AsyncRequestDisambiguation(connection, promptSpokenMessage, promptDisplayMessage, repromptSpokenMessage, repromptDisplayMessage, contentTiles).AsAsyncOperation();
        }

        private static async Task<VoiceCommandDisambiguationResult> AsyncRequestDisambiguation(VoiceCommandServiceConnection connection, string promptSpokenMessage, string promptDisplayMessage, string repromptSpokenMessage, string repromptDisplayMessage, IEnumerable<VoiceCommandContentTile> contentTiles)
        {
            var prompt = new VoiceCommandUserMessage { DisplayMessage = promptDisplayMessage, SpokenMessage = promptSpokenMessage };
            var reprompt = new VoiceCommandUserMessage { DisplayMessage = repromptDisplayMessage, SpokenMessage = repromptSpokenMessage };
            var response = VoiceCommandResponse.CreateResponseForPrompt(prompt, reprompt, contentTiles);
            try
            {
                var result = await connection.RequestDisambiguationAsync(response);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }
}
