﻿using CastIt.Common;
using CastIt.Interfaces;
using CastIt.Models.Messages;
using CastIt.ViewModels.Items;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CastIt.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        #region Members
        private readonly ICastService _castService;
        private readonly IPlayListsService _playListsService;
        private readonly IAppSettingsService _settingsService;
        private FileItemViewModel _currentlyPlayedFile;
        private bool _isPaused;
        private bool _isCurrentlyPlaying;
        private string _currentlyPlayingFilename;
        private bool _isExpanded = true;
        private int _selectedPlayListIndex;
        private MvxNotifyTask _durationTaskNotifier;
        private double _playedPercentage;
        private bool _showSettingsPopUp;
        private bool _showDevicesPopUp;
        private string _elapsedTimeString;
        private string _currentFileThumbnail;

        private readonly MvxInteraction _closeApp = new MvxInteraction();
        private readonly MvxInteraction<(double, double)> _setWindowWidthAndHeight = new MvxInteraction<(double, double)>();
        #endregion

        #region Properties
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public bool IsPaused
        {
            get => _isPaused;
            set => SetProperty(ref _isPaused, value);
        }

        public bool IsCurrentlyPlaying
        {
            get => _isCurrentlyPlaying;
            set => SetProperty(ref _isCurrentlyPlaying, value);
        }

        public string CurrentlyPlayingFilename
        {
            get => _currentlyPlayingFilename;
            set => SetProperty(ref _currentlyPlayingFilename, value);
        }

        public int SelectedPlayListIndex
        {
            get => _selectedPlayListIndex;
            set => SetProperty(ref _selectedPlayListIndex, value);
        }

        public double PlayedPercentage
        {
            get => _playedPercentage;
            set
            {
                if (value == _playedPercentage)
                    return;
                SetProperty(ref _playedPercentage, value);
            }
        }

        public bool ShowSettingsPopUp
        {
            get => _showSettingsPopUp;
            set => SetProperty(ref _showSettingsPopUp, value);
        }

        public bool ShowDevicesPopUp
        {
            get => _showDevicesPopUp;
            set => SetProperty(ref _showDevicesPopUp, value);
        }

        public string ElapsedTimeString
        {
            get => _elapsedTimeString;
            set
            {
                if (value == _elapsedTimeString)
                    return;
                SetProperty(ref _elapsedTimeString, value);
            }
        }

        public string CurrentFileThumbnail
        {
            get => _currentFileThumbnail;
            set => SetProperty(ref _currentFileThumbnail, value);
        }

        public MvxObservableCollection<PlayListItemViewModel> PlayLists { get; set; }
            = new MvxObservableCollection<PlayListItemViewModel>();

        public MvxNotifyTask DurationTaskNotifier
        {
            get => _durationTaskNotifier;
            private set => SetProperty(ref _durationTaskNotifier, value);
        }
        #endregion

        #region Commands
        public IMvxCommand TogglePlaylistVisibilityCommand { get; private set; }
        public IMvxAsyncCommand CloseAppCommand { get; private set; }
        public IMvxCommand PreviousCommand { get; private set; }
        public IMvxCommand NextCommand { get; private set; }
        public IMvxCommand TogglePlayBackCommand { get; private set; }
        public IMvxCommand StopPlayBackCommand { get; private set; }
        public IMvxCommand<int> SkipCommand { get; private set; }
        public IMvxCommand SwitchPlayListsCommand { get; private set; }
        public IMvxAsyncCommand AddNewPlayListCommand { get; private set; }
        public IMvxAsyncCommand<PlayListItemViewModel> DeletePlayListCommand { get; private set; }
        public IMvxAsyncCommand<PlayListItemViewModel> DeleteAllPlayListsExceptCommand { get; private set; }
        public IMvxCommand OpenSettingsCommand { get; private set; }
        public IMvxCommand OpenDevicesCommand { get; private set; }
        #endregion

        #region Interactors
        public IMvxCommand WindowLoadedCommand { get; private set; }
        public IMvxInteraction CloseApp
            => _closeApp;
        public IMvxInteraction<(double, double)> SetWindowWidthAndHeight
            => _setWindowWidthAndHeight;
        #endregion

        public MainViewModel(
            ITextProvider textProvider,
            IMvxMessenger messenger,
            IMvxLogProvider logger,
            ICastService castService,
            IPlayListsService playListsService,
            IAppSettingsService settingsService)
            : base(textProvider, messenger, logger.GetLogFor<MainViewModel>())
        {
            _castService = castService;
            _playListsService = playListsService;
            _settingsService = settingsService;
        }

        #region Methods
        public override async Task Initialize()
        {
            IsExpanded = _settingsService.IsPlayListExpanded;
            _castService.Init();

            var playLists = await _playListsService.GetAllPlayLists();
            PlayLists.AddRange(playLists);

            DurationTaskNotifier = MvxNotifyTask.Create(SetFileDurations);

            _castService.OnTimeChanged += OnFileDurationChanged;
            _castService.OnPositionChanged += OnFilePositionChanged;
            _castService.OnEndReached += OnFileEndReached;
            await base.Initialize();
        }

        public override void SetCommands()
        {
            base.SetCommands();
            WindowLoadedCommand = new MvxCommand(() =>
            {
                var tuple = (_settingsService.WindowWidth, _settingsService.WindowHeight);
                _setWindowWidthAndHeight.Raise(tuple);
            });

            TogglePlaylistVisibilityCommand = new MvxCommand(() =>
            {
                IsExpanded = !IsExpanded;
            });

            CloseAppCommand = new MvxAsyncCommand(HandleCloseApp);

            PreviousCommand = new MvxCommand(() => GoTo(false));

            NextCommand = new MvxCommand(() => GoTo(true));

            TogglePlayBackCommand = new MvxCommand(() =>
            {
                _castService.TogglePlayback();
                IsPaused = !IsPaused;
            });

            StopPlayBackCommand = new MvxCommand(StopPlayBack);

            SkipCommand = new MvxCommand<int>((seconds) =>
            {
                _castService.AddSeconds(seconds);
            });

            SwitchPlayListsCommand = new MvxCommand(SwitchPlayLists);

            AddNewPlayListCommand = new MvxAsyncCommand(AddNewPlayList);

            DeletePlayListCommand = new MvxAsyncCommand<PlayListItemViewModel>(DeletePlayList);

            DeleteAllPlayListsExceptCommand = new MvxAsyncCommand<PlayListItemViewModel>(DeleteAllPlayLists);

            OpenSettingsCommand = new MvxCommand(() => ShowSettingsPopUp = true);

            OpenDevicesCommand = new MvxCommand(() => ShowDevicesPopUp = true);
        }

        public override void RegisterMessages()
        {
            base.RegisterMessages();
            SubscriptionTokens.AddRange(new List<MvxSubscriptionToken>
            {
                Messenger.Subscribe<PlayFileMsg>(async(msg) => await PlayFile(msg.File))
            });
        }

        public void SaveWindowWidthAndHeight(double width, double height)
        {
            _settingsService.WindowWidth = width;
            _settingsService.WindowHeight = height;
            _settingsService.IsPlayListExpanded = IsExpanded;
            _settingsService.SaveSettings();
        }

        private async Task SetFileDurations()
        {
            foreach (var playlist in PlayLists)
            {
                foreach (var item in playlist.Items)
                {
                    await item.SetDuration();
                }
            }
        }

        private async Task AddNewPlayList()
        {
            var vm = Mvx.IoCProvider.Resolve<PlayListItemViewModel>();
            vm.Name = $"New PlayList {PlayLists.Count}";
            vm.Position = PlayLists.Max(pl => pl.Position) + 1;

            var playList = await _playListsService.AddNewPlayList(vm.Name, vm.Position);
            vm.Id = playList.Id;

            PlayLists.Add(vm);
            SelectedPlayListIndex = PlayLists.Count - 1;
        }

        private async Task DeletePlayList(PlayListItemViewModel playlist)
        {
            if (playlist == null)
                return;

            await _playListsService.DeletePlayList(playlist.Id);
            PlayLists.Remove(playlist);
        }

        private async Task DeleteAllPlayLists(PlayListItemViewModel except)
        {
            if (PlayLists.Count <= 1)
                return;
            var exceptIndex = PlayLists.IndexOf(except);

            var items = new List<PlayListItemViewModel>();
            for (int i = 0; i < PlayLists.Count; i++)
            {
                if (i == exceptIndex)
                    continue;
                items.Add(PlayLists[i]);
            }

            await _playListsService.DeletePlayLists(items.Select(pl => pl.Id).ToList());
            PlayLists.RemoveItems(items);
        }

        private async Task HandleCloseApp()
        {
            await _playListsService.SavePlayLists(PlayLists.ToList());
            _castService.OnTimeChanged -= OnFileDurationChanged;
            _castService.OnPositionChanged -= OnFilePositionChanged;
            _castService.OnEndReached -= OnFileEndReached;
            _castService.StopPlayback();
            _castService.CleanThemAll();
            _currentlyPlayedFile?.CleanUp();
            _closeApp.Raise();
        }

        private void SwitchPlayLists()
        {
            int tentativeIndex = SelectedPlayListIndex + 1;
            if (PlayLists.ElementAtOrDefault(tentativeIndex) != null)
            {
                SelectedPlayListIndex = tentativeIndex;
            }
            else
            {
                SelectedPlayListIndex = 0;
            }
        }

        private void GoTo(bool nextTrack)
        {
            if (_currentlyPlayedFile == null)
                return;

            var playlist = PlayLists.First(p => p.Id == _currentlyPlayedFile.PlayListId);
            var fileIndex = playlist.Items.IndexOf(_currentlyPlayedFile);
            if (fileIndex < 0)
                return;

            if (nextTrack)
                fileIndex++;
            else
                fileIndex--;
            var file = playlist.Items.ElementAtOrDefault(fileIndex);

            if (file is null)
            {
                return;
            }

            StopPlayBack();
            file.PlayCommand.Execute();
        }

        private async Task PlayFile(FileItemViewModel file)
        {
            if (!file.Exists)
                return;
            SetCurrentlyPlayingInfo(file.Filename, true);
            _currentlyPlayedFile?.CleanUp();
            _currentlyPlayedFile = file;
            _currentlyPlayedFile.ListenEvents();
            await _castService.StartPlay(file.Path);
            CurrentFileThumbnail = _castService.GetFirstThumbnail();
            if (file.PlayedPercentage != 0)
            {
                _castService.GoToPosition((float)file.PlayedPercentage);
            }
        }

        private void StopPlayBack()
        {
            _castService.StopPlayback();
            SetCurrentlyPlayingInfo(null, false);
            IsPaused = false;
        }

        private void SetCurrentlyPlayingInfo(string filename, bool isPlaying, float playedPercentage = 0)
        {
            OnFilePositionChanged(playedPercentage);
            CurrentlyPlayingFilename = filename;
            IsCurrentlyPlaying = isPlaying;
        }

        private void OnFileDurationChanged(long seconds)
        {
            if (_currentlyPlayedFile is null)
                return;
            var elapsed = TimeSpan.FromSeconds(seconds)
                .ToString(AppConstants.FullElapsedTimeFormat);
            var total = TimeSpan.FromSeconds(_currentlyPlayedFile.TotalSeconds)
                .ToString(AppConstants.FullElapsedTimeFormat);
            if (_currentlyPlayedFile.IsUrlFile)
                ElapsedTimeString = $"{elapsed}";
            else
                ElapsedTimeString = $"{elapsed} / {total}";
        }

        private void OnFilePositionChanged(float playedPercentage)
            => PlayedPercentage = playedPercentage;

        private void OnFileEndReached()
        {
            ElapsedTimeString = string.Empty;
            SetCurrentlyPlayingInfo(null, false);
            GoTo(true);
        }
        #endregion
    }
}
