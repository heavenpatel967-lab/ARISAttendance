using System.Windows;
using ARISAttendance.services; // <-- Notice the lowercase 's' here!

namespace ARISAttendance
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Build the database on startup
            DatabaseInitializer.Initialize();
        }
    }
}