using System.IO;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace SonicStream.Desktop;

/// <summary>
/// Wraps access to
///
/// uses: https://github.com/Tyrrrz/YoutubeExplode
/// </summary>
public class YouTubeExplodeWrapper(string videoUrl)
{
    public async Task<(string, string)> ExtractAudio()
    {
        var youtube = new YoutubeClient();
        var video = await youtube.Videos.GetAsync(videoUrl);
        
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);
        var audioStreamInfo = streamManifest
            .GetAudioStreams()
            .Where(s => s.Container == Container.Mp4)
            .GetWithHighestBitrate();
            
        // not perfect as it leaves a file on the system
        string temporaryAudioFile = $"{Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())}.mp3";
        if (File.Exists(temporaryAudioFile))
            File.Delete(temporaryAudioFile);
            
        await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, temporaryAudioFile);
        
        return (temporaryAudioFile, video.Title);
    }

    public async Task<SongInfo> CheckUrl()
    {
        var youtube = new YoutubeClient();
        var video = await youtube.Videos.GetAsync(videoUrl);

        var song = new SongInfo();
        song.Title = video.Title; 
        song.Artist = video.Author.ChannelTitle; 
        song.Duration = video.Duration.ToString();
        
        return song;
    }
}