using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using utilant_c.Models;
using utilant_c.Services;

namespace utilant_c.Controllers
{
    public class HomeController : Controller
    {
        private static readonly int ALBUM_CACHE_CUTOFF = 5;
        private readonly ILogger<HomeController> _logger;
        private readonly IAlbumServices _aS;
        private readonly PagingModel pM = new PagingModel();


        public HomeController(ILogger<HomeController> logger, IAlbumServices aS)
        {
            _logger = logger;
            this._aS = aS;
        }

        public async Task<IActionResult> Index( string searchTitle, string searchUser, int? page )
        {
            try
            {
                await loadData(searchTitle, searchUser, page);
                return View(pM);
            } catch( Exception e )
            {
                _logger.LogError("Error searching", e);
                pM.errorMessage = "Could not search albums: " + e.Message;
                return View(pM);
            }
            
        }

        public async Task<IActionResult> GetPhotosPartial(int albumId)
        {
            try
            {
                return PartialView(await _aS.getPhotos(albumId));
            }
            catch( Exception e )
            {
                _logger.LogError("Error getting photos", e);
                return PartialView();
            }
            
        }

        public async Task<IActionResult> GetPostsPartial(int userId)
        {
            try
            {
                return PartialView(await _aS.getPosts(userId));
            } catch( Exception e )
            {
                _logger.LogError("Error getting user posts", e);
                return PartialView();
            }
            
        }

        public async Task<IActionResult> GetUserDetails(int userId)
        {
            try
            {
                List<UserModel> userList = await _aS.getUser(userId);
                if (userList != null && userList.Count > 0)
                {
                    return PartialView(userList[0]);
                }
                else
                {
                    return PartialView();
                }
            }
            catch( Exception e )
            {
                _logger.LogError("Error getting user details", e);
                return PartialView();
            }
            
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task loadData(string searchTitle, string searchUser, int? page)
        {
            if (string.IsNullOrEmpty(searchTitle) && string.IsNullOrEmpty(searchUser) )
            {
                return;
            }

            pM.searchAlbum = searchTitle;
            pM.searchUser = searchUser;
            //
            // Going to use some maps to store the album and user data
            //
            List<AlbumModel> albumList;
            List<UserModel> userList;

            _logger.LogInformation("Loading User List");
            userList = await _aS.getUsers();
            foreach (var u in userList)
            {
                _logger.LogInformation("Adding User " + u.email);
                pM.addUser(u);
            }

            _logger.LogInformation("Loading Album List");
            albumList = await _aS.getAlbums();
            foreach (var a in albumList)
            {
                a.user = pM.getUser(a.userId);
                if (!string.IsNullOrEmpty(searchTitle))
                {
                    if (a.title.Contains(searchTitle))
                    {
                        _logger.LogInformation("Adding Album " + a.title);
                        pM.addAlbum(a);
                    }
                }
                else if (!string.IsNullOrEmpty(searchUser))
                {
                    if (a.user.name.Contains(searchUser))
                    {
                        _logger.LogInformation("Adding Album " + a.title);
                        pM.addAlbum(a);
                    }
                }
            }

            //
            // Rather than making (n) separate calls to get the first photo of each album we are going to cache them all.  In a real world scenario, I would change the backend data source lookups to allow for a pull of the first photo with the album lookup
            // For now, we are goign to check the album count.  If there were more than 5 albums we will load all of the photos to save on round trips.  Otherwise, we will make the separate calls to load photos for the specified albums
            // need to be careful with separate as the backend end throttles successive calls
            //
            _logger.LogInformation("Loading Photo List");

            if ( pM.albumMap.Count < ALBUM_CACHE_CUTOFF )
            {
                //
                // Loading photos via individual calls
                //
                foreach (KeyValuePair<int, AlbumModel> entry in pM.albumMap)
                {
                    List<PhotoModel> _photoList = await _aS.getPhotos( entry.Value.id );
                    foreach (var p in _photoList)
                    {
                       entry.Value.addPhoto(p);
                    }
                }
                   
            } 
            else
            {
                //
                // Loading all photos at once to reduce round-trip calls
                //
                List<PhotoModel> _photoList = await _aS.getPhotos();
                foreach (var p in _photoList)
                {
                    AlbumModel a = pM.getAlbum(p.albumId);
                    if (a != null)
                    {
                        a.addPhoto(p);
                    }
                }
            }

            pM.setPage(page ?? 1);
            _logger.LogInformation(pM.albumList.ToString());
        }
    }
}
