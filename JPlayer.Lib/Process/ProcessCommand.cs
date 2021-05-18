using System.Diagnostics;

namespace JPlayer.Lib.Process
{
    public static class ProcessCommand
    {
        /// <summary>
        ///     Run window cmd command
        /// </summary>
        /// <param name="command">command to run</param>
        /// <returns>Standard output</returns>
        public static string WindowsRun(string command)
        {
            System.Diagnostics.Process proc = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            proc.WaitForExit();
            return proc.StandardOutput.ReadToEnd().Trim();
        }
    }
}