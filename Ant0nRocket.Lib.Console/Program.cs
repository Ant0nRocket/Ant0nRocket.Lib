using Ant0nRocket.Lib.Extensions;
using Ant0nRocket.Lib.Helpers;
using Ant0nRocket.Lib.Networking;
using System.Diagnostics;

namespace Ant0nRocket.Lib.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var l = EnvironmentHelper.MachineName;

            //StartPythonScriptAndWaitInSeparateTask("Networking.UdbBeacon.Listener.py");

            //using var udbBeacon = new UdpBeacon();
            //udbBeacon.Start();

            //for (var i = 0; i < 10; i++) { Thread.Sleep(1000); }

            //udbBeacon.Stop();

            //System.Console.ReadLine();
        }

        private static void StartPythonScriptAndWaitInSeparateTask(string scriptFileName)
        {
            _ = Task.Run(() => {
                // Создаем объект для запуска процесса
                ProcessStartInfo startInfo = new()
                {
                    FileName = Path.Combine("Scripts", scriptFileName),
                    UseShellExecute = true,
                };

                // Запускаем процесс
                using (Process process = Process.Start(startInfo))
                {
                    // Ждем завершения процесса
                    process?.WaitForExit();

                    // Получаем код выхода (опционально)
                    int exitCode = process?.ExitCode ?? 0;
                }
            });
        }
    }
}
