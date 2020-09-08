using System.Collections.Generic;

namespace utilant_c.Models
{

    public class AlbumModel
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string title { get; set; }

        public UserModel user { get; set; }
        public List<PhotoModel> photoList { get; private set; }

        public void addPhoto(PhotoModel photo)
        {
            if (photoList == null)
            {
                photoList = new List<PhotoModel>(100);
            }

            photoList.Add(photo);
        }


    }
}
