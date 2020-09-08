using System.Collections.Generic;

namespace utilant_c.Models
{
    public class UserModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string userName { get; set; }
        public string email { get; set; }

        public string phone { get; set; }
        public string website { get; set; }

        public AddressModel address { get; set; }
        public CompanyModel company { get; set; }

        public List<PostModel> postList { get; private set; }
        public void addPost(PostModel post)
        {
            if (postList == null)
            {
                postList = new List<PostModel>(100);
            }

            postList.Add(post);
        }

    }


    public class AddressModel
    {
        public string street { get; set; }
        public string suite { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }

        public GeoModel geo { get; set; }
    }

    public class GeoModel
    {
        public string lat { get; set; }
        public string lon { get; set; }
    }

    public class CompanyModel
    {
        public string name { get; set; }
        public string catchPhrase { get; set; }
        public string bs { get; set; }
    }

}
