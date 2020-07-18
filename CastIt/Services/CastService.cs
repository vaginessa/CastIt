﻿using CastIt.Common;
using CastIt.Common.Utils;
using CastIt.GoogleCast;
using CastIt.GoogleCast.Enums;
using CastIt.GoogleCast.Interfaces;
using CastIt.GoogleCast.Models.Events;
using CastIt.GoogleCast.Models.Media;
using CastIt.Interfaces;
using MvvmCross.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CastIt.Services
{
    public class CastService : ICastService
    {
        private const int SubTitleDefaultTrackId = 1;

        private readonly IMvxLog _logger;
        private readonly IAppWebServer _appWebServer;
        private readonly IFFMpegService _ffmpegService;
        private readonly IYoutubeUrlDecoder _youtubeUrlDecoder;
        private readonly ITelemetryService _telemetryService;

        private readonly Player _player;
        private readonly Track _subtitle;
        private readonly TextTrackStyle _subtitlesStyle;

        private bool _renderWasSet;
        private string _currentFilePath;

        public IList<IReceiver> AvailableDevices { get; } = new List<IReceiver>();
        public OnCastRendererSetHandler OnCastRendererSet { get; set; }
        public OnCastableDeviceAddedHandler OnCastableDeviceAdded { get; set; }
        public OnCastableDeviceDeletedHandler OnCastableDeviceDeleted { get; set; }
        public OnFileLoadedHandler OnFileLoaded { get; set; }
        public OnPositionChangedHandler OnPositionChanged { get; set; }
        public OnTimeChangedHandler OnTimeChanged { get; set; }
        public OnEndReachedHandler OnEndReached { get; set; }
        public OnQualitiesChangedHandler QualitiesChanged { get; set; }
        public OnPausedHandler OnPaused { get; set; }
        public OnDisconnectedHandler OnDisconnected { get; set; }
        public OnVolumeChangedHandler OnVolumeChanged { get; set; }
        public Func<string> GetSubTitles { get; set; }

        public CastService(
            IMvxLogProvider logProvider,
            IAppWebServer appWebServer,
            IFFMpegService ffmpegService,
            IYoutubeUrlDecoder youtubeUrlDecoder,
            ITelemetryService telemetryService)
        {
            _logger = logProvider.GetLogFor<CastService>();
            _appWebServer = appWebServer;
            _ffmpegService = ffmpegService;
            _youtubeUrlDecoder = youtubeUrlDecoder;
            _telemetryService = telemetryService;
            _player = new Player(logProvider.GetLogFor<Player>(), logMsgs: true);
            _subtitle = new Track
            {
                TrackId = SubTitleDefaultTrackId,
                SubType = TextTrackType.Subtitles,
                Type = TrackType.Text,
                Name = "English",
                Language = "en-US"
            };
            _subtitlesStyle = new TextTrackStyle
            {
                ForegroundColor = Color.WhiteSmoke,
                BackgroundColor = Color.Transparent,
                EdgeColor = Color.Black,
                FontScale = 1.4F,
                WindowType = TextTrackWindowType.Normal,
                EdgeType = TextTrackEdgeType.Raised,
                FontStyle = TextTrackFontStyleType.Bold,
                FontGenericFamily = TextTrackFontGenericFamilyType.Casual,
            };
        }

        public void Init()
        {
            _logger.Info($"{nameof(Init)}: Initializing all...");
            _player.FileLoading += FileLoading;
            _player.DeviceAdded += RendererDiscovererItemAdded;
            _player.EndReached += EndReached;
            _player.TimeChanged += TimeChanged;
            _player.PositionChanged += PositionChanged;
            _player.Paused += Paused;
            _player.Disconnected += Disconnected;
            _player.VolumeLevelChanged += VolumeLevelChanged;
            _player.IsMutedChanged += IsMutedChanged;
            _player.Init();

            _logger.Info($"{nameof(Init)}: Initialize completed");
        }

        public async Task StartPlay(
            string mrl,
            int videoStreamIndex,
            int audioStreamIndex,
            int subtitleStreamIndex,
            int quality,
            double seconds = 0)
        {
            _ffmpegService.KillTranscodeProcess();
            bool isLocal = FileUtils.IsLocalFile(mrl);
            bool isUrlFile = FileUtils.IsUrlFile(mrl);
            bool isVideoFile = FileUtils.IsVideoFile(mrl);
            bool isMusicFile = FileUtils.IsMusicFile(mrl);

            if (!isLocal && !isUrlFile)
            {
                _logger.Warn($"{nameof(StartPlay)}: Invalid = {mrl}. Its not a local file and its not a url file");
                return;
            }

            if (AvailableDevices.Count == 0)
            {
                _logger.Warn($"{nameof(StartPlay)}: No renders were found, file = {mrl}");
                return;
            }

            if (!_renderWasSet && AvailableDevices.Count > 0)
            {
                await SetCastRenderer(AvailableDevices.First()).ConfigureAwait(false);
            }
            // create new media
            _currentFilePath = mrl;
            string title = isLocal ? Path.GetFileName(mrl) : mrl;
            string url = isLocal
                ? _appWebServer.GetMediaUrl(mrl, videoStreamIndex, audioStreamIndex, seconds)
                : mrl;

            var metadata = isVideoFile ? new MovieMetadata
            {
                Title = title,
            } : new GenericMediaMetadata
            {
                Title = title,
            };
            var media = new MediaInformation
            {
                ContentId = url,
                Metadata = metadata,
                //You have to set the contenttype before hand, with that, the album art of a music file will be shown
                ContentType = _ffmpegService.GetOutputTranscodeMimeType(mrl)
            };

            var activeTrackIds = new List<int>();
            bool useSubTitleStream = subtitleStreamIndex >= 0;
            if (useSubTitleStream || !string.IsNullOrEmpty(GetSubTitles?.Invoke()))
            {
                _logger.Info($"{nameof(StartPlay)}: Subtitles were specified, generating a compatible one...");
                string subtitleLocation = useSubTitleStream ? mrl : GetSubTitles.Invoke();
                string subTitleFilePath = FileUtils.GetSubTitleFilePath();
                await _ffmpegService.GenerateSubTitles(
                    subtitleLocation,
                    subTitleFilePath,
                    seconds,
                    useSubTitleStream ? subtitleStreamIndex : 0,
                    default);

                _subtitle.TrackContentId = _appWebServer.GetSubTitlePath(subTitleFilePath);
                _logger.Info($"{nameof(StartPlay)}: Subtitles were generated");
                media.Tracks.Add(_subtitle);
                media.TextTrackStyle = _subtitlesStyle;
                activeTrackIds.Add(SubTitleDefaultTrackId);
            }

            string firstThumbnail = GetFirstThumbnail();
            string imgUrl = string.Empty;
            if (isLocal)
            {
                _logger.Info($"{nameof(StartPlay)}: File is a local one, generating metadata...");
                imgUrl = _appWebServer.GetPreviewPath(firstThumbnail);

                if (isVideoFile)
                    media.StreamType = StreamType.Live;

                var fileInfo = await _ffmpegService.GetFileInfo(mrl, default);
                media.Duration = fileInfo.Format.Duration;
                if (isMusicFile)
                {
                    media.Metadata = new MusicTrackMediaMetadata
                    {
                        Title = title,
                        AlbumName = fileInfo.Format.Tag?.Album,
                        Artist = fileInfo.Format.Tag?.Artist,
                    };
                }
            }
            else if (_youtubeUrlDecoder.IsYoutubeUrl(media.ContentId))
            {
                _logger.Info($"{nameof(StartPlay)}: File is a youtube link, parsing it...");
                var youtubeMedia = await _youtubeUrlDecoder.Parse(media.ContentId, quality);
                QualitiesChanged?.Invoke(youtubeMedia.SelectedQuality, youtubeMedia.Qualities);

                imgUrl = youtubeMedia.ThumbnailUrl;
                media.ContentId = youtubeMedia.Url;
                media.Metadata.Title = youtubeMedia.Title;
                media.Metadata.Subtitle = youtubeMedia.Description;
            }

            if (!string.IsNullOrEmpty(imgUrl))
            {
                media.Metadata.Images.Add(new GoogleCast.Models.Image
                {
                    Url = imgUrl
                });
            }

            _logger.Info($"{nameof(StartPlay)}: Trying to load url = {url}");
            var status = await _player.LoadAsync(media, true, seekedSeconds: seconds, activeTrackIds.ToArray()).ConfigureAwait(false);
            _logger.Info($"{nameof(StartPlay)}: Url was succesfully loaded");

            FileLoaded(metadata.Title, imgUrl, _player.CurrentMediaDuration, _player.CurrentVolumeLevel, _player.IsMuted);
        }

        public string GetFirstThumbnail()
            => GetThumbnail(10);

        public string GetFirstThumbnail(string filePath)
            => GetThumbnail(filePath, 10);

        public string GetThumbnail(int second)
            => GetThumbnail(_currentFilePath, second);

        public string GetThumbnail(string filePath, int second)
            => _ffmpegService.GetThumbnail(filePath, second);

        public void GenerateThumbmnails()
            => GenerateThumbmnails(_currentFilePath);

        public async void GenerateThumbmnails(string filePath)
        {
            await Task.Run(() =>
            {
                _ffmpegService.KillThumbnailProcess();
                _ffmpegService.GenerateThumbmnails(filePath);
            }).ConfigureAwait(false);
        }

        public Task TogglePlayback()
        {
            if (_player.IsPlaying)
            {
                return _player.PauseAsync();
            }

            return _player.PlayAsync();
        }

        public Task StopPlayback()
        {
            StopRunningProcess();
            return _player.StopPlaybackAsync();
        }

        public Task GoToPosition(
            string filePath,
            int videoStreamIndex,
            int audioStreamIndex,
            int subtitleStreamIndex,
            int quality,
            double position,
            double totalSeconds)
        {
            if (position >= 0 && position <= 100)
            {
                double seconds = position * totalSeconds / 100;
                if (FileUtils.IsLocalFile(filePath))
                    return StartPlay(filePath, videoStreamIndex, audioStreamIndex, subtitleStreamIndex, quality, seconds);

                return _player.SeekAsync(seconds);
            }

            _logger.Warn($"{nameof(GoToPosition)} Cant go to position = {position}");
            return Task.CompletedTask;
        }

        public Task GoToSeconds(
            int videoStreamIndex,
            int audioStreamIndex,
            int subtitleStreamIndex,
            int quality,
            double seconds)
        {
            if (seconds >= _player.CurrentMediaDuration)
            {
                _logger.Warn(
                    $"{nameof(GoToSeconds)}: Cant go to = {seconds} because is bigger than " +
                    $"the media duration = {_player.CurrentMediaDuration}");
                return Task.CompletedTask;
            }
            if (seconds < 0)
            {
                _logger.Warn($"{nameof(GoToSeconds)}: Wont go to = {seconds}, instead we will go to 0");
                seconds = 0;
            }

            if (FileUtils.IsLocalFile(_currentFilePath))
                return StartPlay(_currentFilePath, videoStreamIndex, audioStreamIndex, subtitleStreamIndex, quality, seconds);

            return _player.SeekAsync(seconds);
        }

        public Task AddSeconds(
            int videoStreamIndex,
            int audioStreamIndex,
            int subtitleStreamIndex,
            int quality,
            double seconds)
        {
            var current = _player.ElapsedSeconds + seconds;
            if (FileUtils.IsLocalFile(_currentFilePath))
            {
                if (current < 0)
                {
                    _logger.Warn($"{nameof(AddSeconds)}: The seconds to add are = {current}. They will be set to 0");
                    current = 0;
                }
                return StartPlay(_currentFilePath, videoStreamIndex, audioStreamIndex, subtitleStreamIndex, quality, current);
            }
            return _player.SeekAsync(current);
        }

        public async Task<double> SetVolume(double level)
        {
            if (!string.IsNullOrEmpty(_currentFilePath) && _player.CurrentVolumeLevel != level)
            {
                var status = await _player.SetVolumeAsync((float)level).ConfigureAwait(false);
                return (double)(status?.Volume?.Level ?? level);
            }
            return _player.CurrentVolumeLevel;
        }

        public async Task<bool> SetIsMuted(bool isMuted)
        {
            if (!string.IsNullOrEmpty(_currentFilePath) && _player.IsMuted != isMuted)
            {
                var status = await _player.SetIsMutedAsync(isMuted).ConfigureAwait(false);
                return status?.Volume?.IsMuted ?? isMuted;
            }
            return _player.IsMuted;
        }

        public void StopRunningProcess()
        {
            _ffmpegService.KillThumbnailProcess();
            _ffmpegService.KillTranscodeProcess();
        }

        public void CleanThemAll()
        {
            try
            {
                _player.FileLoading -= FileLoading;
                _player.DeviceAdded -= RendererDiscovererItemAdded;
                _player.EndReached -= EndReached;
                _player.TimeChanged -= TimeChanged;
                _player.PositionChanged -= PositionChanged;
                _player.Paused -= Paused;
                _player.Disconnected -= Disconnected;
                _player.VolumeLevelChanged -= VolumeLevelChanged;
                _player.IsMutedChanged -= IsMutedChanged;

                StopRunningProcess();

                _appWebServer.Dispose();
                _player.Dispose();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(CleanThemAll)}: An unknown error ocurred");
                _telemetryService.TrackError(ex);
            }
        }

        public Task SetCastRenderer(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return SetNullCastRenderer();
            }
            var renderer = AvailableDevices.FirstOrDefault(d => d.Id == id);
            if (renderer is null)
            {
                return SetNullCastRenderer();
            }

            return SetCastRenderer(renderer);
        }

        #region Events handlers
        private void FileLoading(object sender, EventArgs e)
        {
            _appWebServer.OnFileLoading?.Invoke();
        }

        private void FileLoaded(
            string title,
            string thumbUrl,
            double duration,
            double volumeLevel,
            bool isMuted)
        {
            OnFileLoaded?.Invoke(title, thumbUrl, duration, volumeLevel, isMuted);
        }

        private void EndReached(object sender, EventArgs e)
        {
            OnEndReached?.Invoke();
            _appWebServer.OnEndReached?.Invoke();
        }

        private void PositionChanged(object sender, double position)
        {
            if (position > 100)
            {
                position = 100;
            }
            OnPositionChanged?.Invoke(position);
            _appWebServer.OnPositionChanged?.Invoke(position);
        }

        private void TimeChanged(object sender, double seconds)
        {
            OnTimeChanged?.Invoke(seconds);
            _appWebServer.OnTimeChanged?.Invoke(seconds);
        }

        private void Paused(object sender, EventArgs e)
        {
            OnPaused?.Invoke();
            _appWebServer.OnPaused?.Invoke();
        }

        private void Disconnected(object sender, EventArgs e)
        {
            _renderWasSet = false;
            OnDisconnected?.Invoke();
            _appWebServer.OnDisconnected?.Invoke();
        }

        private void VolumeLevelChanged(object sender, double e)
        {
            OnVolumeChanged?.Invoke(e, _player.IsMuted);
            _appWebServer.OnVolumeChanged?.Invoke(e, _player.IsMuted);
        }

        private void IsMutedChanged(object sender, bool e)
        {
            OnVolumeChanged?.Invoke(_player.CurrentVolumeLevel, e);
            _appWebServer.OnVolumeChanged?.Invoke(_player.CurrentVolumeLevel, e);
        }

        private void RendererDiscovererItemAdded(object sender, DeviceAddedArgs e)
        {
            _logger.Info(
                $"{nameof(RendererDiscovererItemAdded)}: New item discovered: " +
                $"{e.Receiver.FriendlyName} - Ip = {e.Receiver.Host}:{e.Receiver.Port}");
            AvailableDevices.Add(e.Receiver);

            OnCastableDeviceAdded?.Invoke(e.Receiver);
        }
        #endregion

        //TODO: CHECK IF WE CAN KNOW WHEN A DEVICE IS REMOVED
        //private void RendererDiscovererItemDeleted(object sender, RendererDiscovererItemDeletedEventArgs e)
        //{
        //    _logger.Info(
        //        $"{nameof(RendererDiscovererItemAdded)}: Item removed: " +
        //        $"{e.RendererItem.Name} of type {e.RendererItem.Type}");
        //    _rendererItems.Remove(e.RendererItem);
        //    OnCastableDeviceDeleted?.Invoke(new CastableDevice
        //    {
        //        Name = e.RendererItem.Name,
        //        Type = e.RendererItem.Type
        //    });
        //}

        private Task SetCastRenderer(IReceiver receiver)
        {
            _renderWasSet = true;
            OnCastRendererSet?.Invoke(receiver.Id);
            return _player.ConnectAsync(receiver);
        }

        private Task SetNullCastRenderer()
        {
            _renderWasSet = false;
            return _player.DisconnectAsync();
        }
    }
}
