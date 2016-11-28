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
            await BackgroundVoiceCommandHelper.ReportProgressAsync(voiceServiceConn, "Repot progress", "Try to do!");
        }
    }
}
