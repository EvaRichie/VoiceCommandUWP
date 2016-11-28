using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Storage;

namespace ControlPanel.Library
{
    public class VoiceCommandHelper
    {
        public static IAsyncAction TryToRegistVoiceCommandsAsync(string fileNameOrPath)
        {
            return AsyncTryToRegistVoiceCommands(fileNameOrPath).AsAsyncAction();
        }

        private static async Task AsyncTryToRegistVoiceCommands(string fileNameOrPath)
        {
            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileNameOrPath));
                await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(file);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
