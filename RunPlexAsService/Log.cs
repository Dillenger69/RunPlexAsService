
namespace RunPlexAsService
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    public class Log
    {
        #region Fields
        private EventLog eventLog1;
        #endregion Fields

        #region Constructors        
        /// <summary>
        /// Initializes a new instance of the <see cref="Log" /> class.
        /// </summary>
        /// <param name="eventLog">The event log.</param>
        public Log(EventLog eventLog)
        {
            eventLog1 = eventLog;
            FileName = "PlexAsService.log";
            SetLocalPath();

        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the local path.
        /// </summary>
        /// <value>
        /// The local path.
        /// </value>
        public string LocalPath { get; private set; }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>
        /// The full path.
        /// </value>
        public string FullPath
        {
            get
            {
                return Path.Combine(LocalPath, FileName);
            }
        }
        #endregion Properties

        #region Methods        
        /// <summary>
        /// Sets the local path of this executable.
        /// </summary>
        private void SetLocalPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            LocalPath = Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Initializes the log by writing an empty string to the file.
        /// </summary>
        private void InitializeLog()
        {
            File.WriteAllText(FullPath, string.Empty);
        }

        /// <summary>
        /// Writes an entry to the local log file as well as an information event to the event log.
        /// </summary>
        /// <param name="entryToWrite">The text to log.</param>
        /// <param name="dumpLocal">if set to <c>true</c> [dumps to local log file also]. Defaults to false.</param>
        public void WriteEntry(string entryToWrite, Boolean dumpLocal = false)
        {
            eventLog1.WriteEntry(entryToWrite, EventLogEntryType.Information);
            if (dumpLocal)
            {
                string withTimestamp = DateTime.Now.ToString() + ": " + entryToWrite + Environment.NewLine;
                File.AppendAllText(FullPath, withTimestamp);
            }
        }
        #endregion Methods
    }
}
