namespace FotoFlow
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            var settings = FotoFlow.Core.AppUserSettings.Load();
            var startMode = settings.LastMode ?? FotoFlow.Core.FotoFlowMode.Basic;
            FotoFlow.Core.AppRuntimeState.LastMode = startMode;

            Application.ApplicationExit += (_, _) =>
            {
                FotoFlow.Core.AppUserSettings.Update(s => s.LastMode = FotoFlow.Core.AppRuntimeState.LastMode);
            };

            Form startForm = startMode == FotoFlow.Core.FotoFlowMode.Advance
                ? new FrmFotoFlowAdvance()
                : new FrmFotoFlow();

            Application.Run(startForm);
        }
    }
}
