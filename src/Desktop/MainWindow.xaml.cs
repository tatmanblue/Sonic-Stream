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
public partial class MainWindow : Window, INotifyPropertyChanged
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
        Message = "App Started ok";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        Console.WriteLine($"PropertyChanged: {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        Console.WriteLine($"PropertyChanged event for {propertyName} was invoked.");
    }

    private void Stupid(object sender, RoutedEventArgs e)
    {
        Console.WriteLine($"Stupid to check: {youtubeUrl}");
        Message = $"Stupid: {youtubeUrl}";
    }
    
    private async void CheckUrlButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Console.WriteLine($"Clicked to check: {youtubeUrl}");
            Message = $"Checking: {youtubeUrl}";
            YouTubeExplodeWrapper details = new YouTubeExplodeWrapper(YoutubeUrl);
            var result = await details.CheckUrl();
            this.SongInfo = result;
            Message = $"...done";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    
    private async void ExtractButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Console.WriteLine($"extracting: {youtubeUrl}");
            Message = $"extracting audio: {youtubeUrl}";
            YouTubeExplodeWrapper details = new YouTubeExplodeWrapper(YoutubeUrl);
            (string file, string title) = await details.ExtractAudio();
            Console.WriteLine($"saved to: {file}");
            Message = $"...done";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}