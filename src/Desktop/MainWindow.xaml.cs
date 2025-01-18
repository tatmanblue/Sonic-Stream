using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SonicStream.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private string youtubeUrl = "https://www.youtube.com/watch?v=E3VDW4aSn30";
    public string YoutubeUrl
    {
        get => youtubeUrl;
        set
        {
            youtubeUrl = value;
            OnPropertyChanged();
        }
    }
    
    private SongInfo songInfo;
    public SongInfo SongInfo
    {
        get => songInfo;
        set
        {
            songInfo = value;
            OnPropertyChanged();
        }
    }

    private string message = "hello";

    public string Message
    {
        get => message;
        set
        {
            message = value;
            Console.WriteLine($"Message updated: {message}");
            OnPropertyChanged();
        }
    }
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this; // Set DataContext for binding
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        Console.WriteLine($"PropertyChanged: {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private async void CheckUrlButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Message = $"Checking: {youtubeUrl}";
            YouTubeExplodeWrapper details = new YouTubeExplodeWrapper(YoutubeUrl);
            var result = details.CheckUrl().Result;
            this.SongInfo = result;
            /*
            details.CheckUrl().ContinueWith(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    var result = task.Result;
                    this.SongInfo = result;
                }
                else if (task.IsFaulted)
                {
                    MessageBox.Show(task.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });            
            */
            Message = $"...done";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}