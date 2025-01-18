using System.Configuration;
using System.Data;
using System.Windows;

namespace SonicStream.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            MessageBox.Show($"Unhandled exception: {((Exception)args.ExceptionObject).Message}");
        };

        DispatcherUnhandledException += (s, args) =>
        {
            MessageBox.Show($"UI thread exception: {args.Exception.Message}");
            args.Handled = true;
        };
    }
}