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
                switch (command.CommandName)
                {
                    case "RollCommand":
                        await ServiceCommandHandleAsync(command.SpeechRecognitionResult.SemanticInterpretation);
                        break;
                    case "ShowAllCommand":
                        await FindCommandHandleAsync(command.SpeechRecognitionResult.SemanticInterpretation);
                        break;
                    default:
                        break;
                }
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

        private async Task ServiceCommandHandleAsync(SpeechRecognitionSemanticInterpretation interpretation)
        {
            var progressMessage = "Get ready";
            await Helpers.VoiceCommandResponseHelper.ReportProgressAsync(voiceServiceConn, progressMessage, progressMessage);
            var randomInt = new Random((int)DateTime.UtcNow.Ticks).Next() % 6 + 1;
            System.Diagnostics.Debug.WriteLine(randomInt);
            var contentTiles = new List<VoiceCommandContentTile>();
            var tile = new VoiceCommandContentTile();
            try
            {
                tile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                tile.Image = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///ControlPanel.BackgroundServices/Assets/68_Dice_{randomInt}.png"));
                tile.AppContext = randomInt;
                tile.AppLaunchArgument = "DiceResult=" + randomInt;
                tile.Title = $"The dice result is {randomInt}";
                contentTiles.Add(tile);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            var successMessage = $"You got {randomInt}";
            await Helpers.VoiceCommandResponseHelper.ReportSuccessAsync(voiceServiceConn, successMessage, successMessage, contentTiles);
        }

        private async Task FindCommandHandleAsync(SpeechRecognitionSemanticInterpretation interpretation)
        {
            var searchQuery = string.Empty;
            if (interpretation.Properties.ContainsKey("DiceNum"))
                searchQuery = interpretation.Properties["DiceNum"].FirstOrDefault();
            if (!string.IsNullOrEmpty(searchQuery) && !string.IsNullOrWhiteSpace(searchQuery))
            {
                response = VoiceCommandResponse.CreateResponse(new VoiceCommandUserMessage() { SpokenMessage = "Get ready", DisplayMessage = "Get ready" });
                await voiceServiceConn.ReportProgressAsync(response);
                //await DisambiguateAsync("Select a result", "Please select a result");
                var promptStr = "Select a result";
                var repromptStr = "Please select a result";
                var contentTiles = new List<VoiceCommandContentTile>();
                for (var i = 1; i < 7; i++)
                {
                    var tile = new VoiceCommandContentTile();
                    tile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                    tile.Image = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///ControlPanel.BackgroundServices/Assets/68_Dice_{i}.png"));
                    tile.AppContext = i;
                    tile.AppLaunchArgument = $"type={i}";
                    tile.Title = $"The dice result is {i}";
                    contentTiles.Add(tile);
                }
                var result = await Helpers.VoiceCommandResponseHelper.RequestDisambiguationAsync(voiceServiceConn, promptStr, repromptStr, contentTiles);
                if (result != null)
                {
                    contentTiles.Clear();
                    contentTiles.Add(result.SelectedItem);
                    var successStr = "You select a dice";
                    await Helpers.VoiceCommandResponseHelper.ReportSuccessAsync(voiceServiceConn, successStr, successStr, contentTiles);
                }
            }

        }

        private async Task DisambiguateAsync(string promptMessage, string repromptMessage)
        {
            var prompt = new VoiceCommandUserMessage();
            prompt.DisplayMessage = prompt.SpokenMessage = promptMessage;
            var reprompt = new VoiceCommandUserMessage();
            reprompt.DisplayMessage = reprompt.SpokenMessage = repromptMessage;
            var contentTiles = new List<VoiceCommandContentTile>();
            for (var i = 1; i < 7; i++)
            {
                var tile = new VoiceCommandContentTile();
                tile.ContentTileType = VoiceCommandContentTileType.TitleWith68x68IconAndText;
                tile.Image = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///ControlPanel.BackgroundServices/Assets/68_Dice_{i}.png"));
                tile.AppContext = i;
                tile.AppLaunchArgument = $"type={i}";
                tile.Title = $"The dice result is {i}";
                contentTiles.Add(tile);
            }
            response = VoiceCommandResponse.CreateResponseForPrompt(prompt, reprompt, contentTiles);
            try
            {
                var result = await voiceServiceConn.RequestDisambiguationAsync(response);
                if (result != null)
                {
                    System.Diagnostics.Debug.WriteLine(result);
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
