namespace RunPlexAsService
{
    using System.ServiceProcess;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;

            ServicesToRun = new ServiceBase[] 
            {
                new RunPlexAsService()
            };

            ServiceBase.Run(ServicesToRun);
        }
    }
}
