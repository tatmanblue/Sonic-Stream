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

    private ExtractStates state = ExtractStates.Gathering;
    public ExtractStates State
    {
        get => state;
        set
        {
            state = value;
            OnPropertyChanged();
        }
    }
    
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

    private string errorMessage = "";
    public string ErrorMessage
    {
        get => errorMessage;
        set
        {
            errorMessage = value;
            OnPropertyChanged();
        }
    }
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this; // Set DataContext for binding
        ChangeState(ExtractStates.Gathering);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        Console.WriteLine($"PropertyChanged: {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        Console.WriteLine($"PropertyChanged event for {propertyName} was invoked.");
    }
    
    private void ChangeState(ExtractStates state)
    {
        State = state;
        string visualStateName = $"{state}View";
        Console.WriteLine($"State changed to {state}/'{visualStateName}'");
        VisualStateManager.GoToElementState(this.Body, visualStateName, true);
    }
    
    private async void CheckUrlButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Console.WriteLine($"Clicked to check: {youtubeUrl}");
            ChangeState(ExtractStates.Checking);
            YouTubeExplodeWrapper details = new YouTubeExplodeWrapper(YoutubeUrl);
            var result = await details.CheckUrl();
            this.SongInfo = result;
        }
        catch (Exception ex)
        {
            ChangeState(ExtractStates.Error);
            ErrorMessage = ex.Message;
            Console.WriteLine(ex);
        }
    }
    
    private async void ExtractButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Console.WriteLine($"extracting: {youtubeUrl}");
            ChangeState(ExtractStates.Extracting);
            YouTubeExplodeWrapper details = new YouTubeExplodeWrapper(YoutubeUrl);
            (string file, string title) = await details.ExtractAudio();
            Console.WriteLine($"saved to: {file}");
            SongInfo.FileName = file;
            ChangeState(ExtractStates.Results);
        }
        catch (Exception ex)
        {
            ChangeState(ExtractStates.Error);
            ErrorMessage = ex.Message;
            Console.WriteLine(ex);
        }
    }

    private void RestartButton_Click(object sender, RoutedEventArgs e)
    {
        ChangeState(ExtractStates.Gathering);
    }
}