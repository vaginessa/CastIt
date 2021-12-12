﻿using CastIt.Domain.Enums;
using CastIt.GoogleCast.Shared.Device;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CastIt.Server.Interfaces
{
    public interface ICastService
    {
        List<IReceiver> AvailableDevices { get; }
        bool IsPlayingOrPaused { get; }

        Task Init();
        Task StopAsync();
        Task StopRunningProcess();
        Task AddSeconds(double seconds);
        Task GoToSeconds(double seconds);
        Task GoToPosition(double position, double totalSeconds);
        Task GoToPosition(double position);
        Task<double> SetVolume(double level);
        Task<bool> SetIsMuted(bool isMuted);
        Task<string> GetFirstThumbnail();
        Task<string> GetFirstThumbnail(string filePath);
        Task<string> GetThumbnail();
        Task<string> GetThumbnail(string filePath);
        Task TogglePlayback();
        void GenerateThumbnails();
        void GenerateThumbnails(string filePath);
        Task SetCastRenderer(string id);
        Task SetCastRenderer(string host, int port);

        void SendEndReached();

        void SendPositionChanged(double position);

        void SendTimeChanged(double seconds);

        void SendPaused();

        void SendDisconnected();

        void SendVolumeLevelChanged(double newValue);

        void SendIsMutedChanged(bool isMuted);

        void SendRendererDiscovererItemAdded(IReceiver item);

        void SendErrorLoadingFile();

        void SendNoDevicesFound();

        void SendNoInternetConnection();

        void SendPlayListNotFound();

        void SendFileNotFound();

        void SendInvalidRequest();

        void SendServerMsg(AppMessageType type);

        Task RefreshCastDevices(TimeSpan? ts);
    }
}