using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using utilant_c.Models;

namespace utilant_c.Services
{

    public interface IAlbumServices
    {
        Task<List<AlbumModel>> getAlbums();
        Task<List<UserModel>> getUsers();

        Task<List<UserModel>> getUser(int userId);
        Task<List<PostModel>> getPosts( int userId);

        Task<List<PhotoModel>> getPhotos(int albumId );

        Task<List<PhotoModel>> getPhotos();
    }
    public class AlbumServices : IAlbumServices
    {
        private static readonly HttpClient _hC = new HttpClient();
        private readonly ILogger<AlbumServices> _logger;

        public AlbumServices(ILogger<AlbumServices> logger)
        {
            _logger = logger;
        }

        public async Task<List<AlbumModel>> getAlbums()
        {
            return await JsonSerializer.DeserializeAsync<List<AlbumModel>>(await _hC.GetStreamAsync("https://jsonplaceholder.typicode.com/albums"));
        }

        public async Task<List<UserModel>> getUsers()
        {
            return await JsonSerializer.DeserializeAsync<List<UserModel>>(await _hC.GetStreamAsync("https://jsonplaceholder.typicode.com/users"));
        }

        public async Task<List<UserModel>> getUser( int userId)
        {
            return await JsonSerializer.DeserializeAsync<List<UserModel>> (await _hC.GetStreamAsync("https://jsonplaceholder.typicode.com/users?id=" + userId ));
        }

        public async Task<List<PostModel>> getPosts(int userId)
        {
            return await JsonSerializer.DeserializeAsync<List<PostModel>>(await _hC.GetStreamAsync("https://jsonplaceholder.typicode.com/posts?userId=" + userId));
        }

        public async Task<List<PhotoModel>> getPhotos(int albumId)
        {
            _logger.LogInformation(("https://jsonplaceholder.typicode.com/photos/" + albumId));
            return await JsonSerializer.DeserializeAsync<List<PhotoModel>>(await _hC.GetStreamAsync("https://jsonplaceholder.typicode.com/photos?albumId=" + albumId));
        }

        public async Task<List<PhotoModel>> getPhotos()
        {
            return await JsonSerializer.DeserializeAsync<List<PhotoModel>>(await _hC.GetStreamAsync("https://jsonplaceholder.typicode.com/photos"));
        }


    }
}
