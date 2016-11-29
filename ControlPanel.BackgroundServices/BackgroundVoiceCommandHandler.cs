using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.SpeechRecognition;

namespace ControlPanel.BackgroundServices
{
    public sealed class BackgroundVoiceCommandHandler : IBackgroundTask
    {
        private BackgroundTaskDeferral deferral;
        private VoiceCommandServiceConnection voiceServiceConn;
        private VoiceCommandResponse response;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;
            try
            {
                var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;
                voiceServiceConn = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
                var command = await voiceServiceConn?.GetVoiceCommandAsync();
                System.Diagnostics.Debug.WriteLine(command.CommandName);
                await HandleAsync(command.SpeechRecognitionResult.SemanticInterpretation);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                deferral?.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            deferral?.Complete();
        }

        private async Task HandleAsync(SpeechRecognitionSemanticInterpretation interpretation)
        {
            //await BackgroundVoiceCommandHelper.ReportProgressAsync(voiceServiceConn, "Repot progress", "Try to do!");
            response = VoiceCommandResponse.CreateResponse(new VoiceCommandUserMessage() { SpokenMessage = "Get ready", DisplayMessage = "Get ready" });
            await voiceServiceConn.ReportProgressAsync(response);
            await Task.Delay(500);
            var random = new Random((int)DateTime.UtcNow.Ticks);
            var randomInt = (random.Next() % 6) + 1;
            var contentTiles = new List<VoiceCommandContentTile>();
            var tile = new VoiceCommandContentTile();
            try
            {
                tile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                tile.Image = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///ControlPanel.BackgroundServices/Assets/Dice_{randomInt}.png"));
                tile.AppContext = null;
                tile.AppLaunchArgument = "type=" + randomInt;
                tile.Title = $"The dice result is {randomInt}";
                contentTiles.Add(tile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            response = VoiceCommandResponse.CreateResponse(new VoiceCommandUserMessage() { DisplayMessage = "Result is", SpokenMessage = $"{randomInt}" }, contentTiles);
            await voiceServiceConn.ReportSuccessAsync(response);
            //await BackgroundVoiceCommandHelper.ReportSuccessAsync(voiceServiceConn, "GG", $"Success {randomInt}", contentTiles);
        }
    }
}
