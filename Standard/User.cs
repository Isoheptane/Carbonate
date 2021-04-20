using System;
using System.IO;
using System.Text;
using JsonSharp;

namespace Carbonate.Standard
{
    public class User
    {
        public string username;
        public string nickname;
        public bool isAdmin;
        public bool isSuperAdmin;

        /// <summary>
        /// Create a empty user object.
        /// </summary>
        public User()
        {
            username        = "undefined";
            nickname        = "undefined";
            isAdmin         = false;
            isSuperAdmin    = false;
        }
        
        /// <summary>
        /// Create a user object from JSON.
        /// </summary>
        public static User CreateFromJson(JsonObject json)
        {
            User user = new User();
            user.username       = json["username"];
            user.nickname       = json["nickname"];
            user.isAdmin        = json["isAdmin"];
            user.isSuperAdmin   = json["isSuperAdmin"];
            return user;
        }

        /// <summary>
        /// Convert a user object into JSON.
        /// </summary>
        public JsonObject ToJsonObject()
        {
            JsonObject json = new JsonObject;
            json.Add("username"     , username);
            json.Add("nickname"     , nickname);
            json.Add("isAdmin"      , isAdmin);
            json.Add("isSuperAdmin" , isSuperAdmin);
            return json;
        }
    }

}
