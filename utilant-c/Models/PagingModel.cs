using Microsoft.AspNetCore.Mvc.RazorPages;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;

namespace utilant_c.Models
{

    public class PagingModel :PageModel
    {
        public string errorMessage { get; set; }
        public string searchAlbum { get; set; }
        public string searchUser { get; set; }

        private static readonly int PAGE_SIZE = 4;
        public IPagedList<KeyValuePair<int,AlbumModel>> albumList { get; private set; }
        public Dictionary<int, AlbumModel> albumMap { get; private set; }
        private Dictionary<int, UserModel> userMap;

        public PagingModel()
        {
            albumMap = new Dictionary<int, AlbumModel>(100);
            userMap = new Dictionary<int, UserModel>(25);
        }
        public AlbumModel getAlbum(int id)
        {
            AlbumModel a;
            albumMap.TryGetValue(id, out a);
            return a;
        }

        public UserModel getUser(int id)
        {
            UserModel u;
            userMap.TryGetValue(id, out u);
            return u;

        }

        public void addUser(UserModel u)
        {
            userMap.Add(u.id, u);
        }

        public void addAlbum(AlbumModel a)
        {
            albumMap.Add(a.id, a);
        }

        public void setPage( int page )
        {
           albumList = albumMap.ToPagedList(page, PAGE_SIZE);

        }



    }
}
