﻿using CastIt.Domain.Entities;
using CastIt.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CastIt.Server.Interfaces
{
    public interface IAppDataService
    {
        void Close();
        Task<List<PlayList>> GetAllPlayLists();
        Task<PlayList> GetPlayList(long id);
        Task<PlayList> AddNewPlayList(string name, int position);
        Task UpdatePlayList(long id, string name, int position);
        Task DeletePlayList(long id);
        Task DeletePlayLists(List<long> ids);
        Task<List<FileItem>> GetAllFiles(long playlistId);
        Task<FileItem> GetFile(long id);
        Task<FileItem> AddFile(
            long playListId,
            string path,
            int position,
            string name = null,
            string description = null,
            double duration = 0);
        Task<List<FileItem>> AddFiles(List<FileItem> files);
        Task DeleteFile(long id);
        Task DeleteFiles(List<long> ids);
        Task UpdateFile(long id, string name, string description, double duration);

        Task SavePlayListChanges(List<ServerPlayList> playLists);

        Task SaveFileChanges(List<ServerFileItem> files);

        Task<bool> TinyCodeExists(string code);

        Task<string> GetBase64FromTinyCode(string code);

        Task<string> GenerateTinyCode(string base64);

        Task DeleteOldTinyCodes();
    }
}