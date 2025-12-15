using System.Diagnostics;


namespace VoiceAssistant.Utils;


public static class AudioHelper
{
    
    public static void PlayWav(string path)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "aplay",
            Arguments = $"\"{path}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        });
    }
}