
namespace RunPlexAsService
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.ServiceProcess;
    using Microsoft.Win32;
    using System.IO;

    public partial class RunPlexAsService : ServiceBase
    {
        private int serverPid = 0;
        private EventLog eventLog1;
        private Log log;
        private string defaultPath = @"C:\Program Files (x86)\Plex\Plex Media Server";

        /// <summary>
        /// Sets the service status.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="serviceStatus">The service status.</param>
        /// <returns></returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        /// <summary>
        /// Initializes a new instance of the <see cref="RunPlexAsService"/> class.
        /// </summary>
        public RunPlexAsService()
        {
            InitializeComponent();
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists("RunPlexAsService"))
            {
                EventLog.CreateEventSource("RunPlexAsService", "Application");
            }

            eventLog1.Source = "RunPlexAsService";
            eventLog1.Log = "Application";
            log = new Log(eventLog1);
        }

        /// <summary>
        /// Called when [start]. Starts Plex Media Server.
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected override void OnStart(string[] args)
        {


            // get the path to 'Plex Media Server.exe'
            string plexPath = GetPlexInstallationPath();

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus()
            {
                dwCurrentState = ServiceState.SERVICE_START_PENDING,
                dwWaitHint = 100000
            };

            SetServiceStatus(ServiceHandle, ref serviceStatus);

            // start Plex Media Server
            log.WriteEntry("Starting Plex Media Server as a service");
            serverPid = StartProc(plexPath, string.Empty);
            log.WriteEntry("Plex Media Server started with Process ID " + serverPid);
            System.Threading.Thread.Sleep(3000);

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }

        /// <summary>
        /// Called when [stop]. Kills processes created when the server started. TODO: API call to plex to shut down
        /// </summary>
        protected override void OnStop()
        {
            // Update the service state to Service Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus()
            {
                dwCurrentState = ServiceState.SERVICE_STOP_PENDING,
                dwWaitHint = 100000
            };

            SetServiceStatus(ServiceHandle, ref serviceStatus);

            if (serverPid != 0)
            {
                // TODO: find out if there's a plex shutdown api call we can make
                eventLog1.WriteEntry("Stopping Plex Media Server, process ID " + serverPid, EventLogEntryType.Information);
                StopProc(serverPid);
                eventLog1.WriteEntry("Process ID " + serverPid + " stopped", EventLogEntryType.Information);
            }

            // Update the service state to stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(ServiceHandle, ref serviceStatus);
        }

        /// <summary>
        /// Starts a given executable(bat file etc.) with the given command line options.
        /// </summary>
        /// <param name="pathToExe">The path to executable.</param>
        /// <param name="commandLine">The command line.</param>
        /// <returns></returns>
        private int StartProc(string pathToExe, string commandLine)
        {
            // put quotes around path to exe if necessary
            if (!pathToExe.StartsWith("\"") && pathToExe.Contains(" "))
            {
                pathToExe = "\"" + pathToExe + "\"";
            }

            // start the process and return the pid
            try
            {
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.EnableRaisingEvents = false;
                process.StartInfo.FileName = pathToExe;
                process.StartInfo.Arguments = commandLine;
                process.Start();
                return process.Id;
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Exception caught starting '" + pathToExe + commandLine + "' :" + ex.Message, EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Kills the process of a given ID.
        /// </summary>
        /// <param name="pid">The process ID.</param>
        private void StopProc(int pid)
        {
            try
            {
                Process p = Process.GetProcessById(pid);
                p.Kill();
            }
            catch (Exception ex)
            {
                eventLog1.WriteEntry("Exception caught killing process ID '" + pid + "' :" + ex.Message, EventLogEntryType.Error);
                throw;
            }
        }

        /// <summary>
        /// Gets the 'Plex Media Server.exe' installation path.
        /// </summary>
        /// <returns>string containing the full path to 'Plex Media Server.exe'</returns>
        private string GetPlexInstallationPath()
        {
            var returnValue = string.Empty;

            // look in the registry for an installation path
            var ourKey = Registry.LocalMachine;
            ourKey = ourKey.OpenSubKey(@"SOFTWARE\WOW6432Node\Plex, Inc.\Plex Media Server"); // \Folders
            var folder = ourKey.GetValue("InstallFolder").ToString();
            returnValue = Path.Combine(folder, "Plex Media Server.exe");

            // if we don't find it there, look in the default installation path
            if (string.IsNullOrWhiteSpace(returnValue))
            {
                var defaultPath = @"C:\Program Files (x86)\Plex\Plex Media Server";

                var files = Directory.GetFiles(defaultPath);
                foreach (var fileName in files)
                {
                    if (fileName.EndsWith("Plex Media Server.exe"))
                    {
                        returnValue = fileName; // the file names are returned as full paths, so there's nothing to change.
                        break;
                    }
                }
            }

            //TODO: look elsewhere and other ways

            // if still not found, throw exception to stop service.
            if (string.IsNullOrWhiteSpace(returnValue))
            {
                eventLog1.WriteEntry("Could not determine 'Plex Media Server.exe' location. Service not started.", EventLogEntryType.Error);
                throw new RunPlexAsServiceException("Could not determine 'Plex Media Server.exe' location. Service not started.");
            }

            return returnValue;
        }
    }
}
