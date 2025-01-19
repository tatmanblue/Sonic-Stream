using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;


namespace SonicStream.Desktop;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{

    #region ViewModel fields
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
    
    private string saveToDir = String.Empty;
    public string SaveToDir
    {
        get => saveToDir;
        set
        {
            saveToDir = value;
            OnPropertyChanged();
            FullPath = saveToDir; // hacky way to OnPropertyChanged() for FullPath
        }
    }
    
    private string fileName = String.Empty;
    public string FileName
    {
        get => fileName;
        set
        {
            fileName = value;
            OnPropertyChanged();
            FullPath = fileName; // hacky way to OnPropertyChanged() for FullPath
        }
    }
    
    private string fullPath = String.Empty;

    public string FullPath
    {
        get => fullPath;
        set
        {
            fullPath = System.IO.Path.Combine(SaveToDir, FileName);
            OnPropertyChanged();
        }
    }
    #endregion
    
    #region View methods and handlers
    public MainWindow()
    {
        string userDir = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        SaveToDir = System.IO.Path.Combine(userDir, "SonicStream");
        
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
    
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
        {
            this.DragMove();
        }
    }
    #endregion
    
    private void ChangeState(ExtractStates state)
    {
        State = state;
        string visualStateName = $"{state}View";
        Console.WriteLine($"State changed to {state}/'{visualStateName}'");
        VisualStateManager.GoToElementState(this.Body, visualStateName, true);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
    
    private void ChooseDirectory_Click(object sender, RoutedEventArgs e)
    {
        var folderBrowser = new OpenFolderDialog();
        
        if (false == Directory.Exists(SaveToDir))
            Directory.CreateDirectory(SaveToDir);
        
        folderBrowser.InitialDirectory = SaveToDir;
        folderBrowser.Title = "Choose Directory";

        if (folderBrowser.ShowDialog() == true)
        {
            SaveToDir = folderBrowser.FolderName;
        }
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
            FileName = $"{SongInfo.Artist}-{SongInfo.Title}.mp3";
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
            (string file, string title) = await details.ExtractAudio(System.IO.Path.Combine(SaveToDir, FileName));
            Console.WriteLine($"saved to: {file}");
            SongInfo.FileName = file;
            SongInfo = SongInfo;
            ChangeState(ExtractStates.Results);
        }
        catch (Exception ex)
        {
            ChangeState(ExtractStates.Error);
            ErrorMessage = ex.Message;
            Console.WriteLine(ex);
        }
    }

    private void BrowseSaveLocation_Click(object sender, RoutedEventArgs e)
    {
        Process.Start("explorer.exe" , SaveToDir);
    }

    private void RestartButton_Click(object sender, RoutedEventArgs e)
    {
        ChangeState(ExtractStates.Gathering);
    }
}