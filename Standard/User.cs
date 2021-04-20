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
        public DateTime registerTime;
        public DateTime lastLoginTime;
        public DateTime lastChatTime;
        public DateTime muteTime;
        public DateTime banTime;

        /// <summary>
        /// Create a empty user object.
        /// </summary>
        public User()
        {
            username        = "undefined";
            nickname        = "undefined";
            isAdmin         = false;
            isSuperAdmin    = false;
            registerTime    = DateTime.MinValue;
            lastLoginTime   = DateTime.MinValue;
            lastChatTime    = DateTime.MinValue;
            muteTime        = DateTime.MinValue;
            banTime         = DateTime.MinValue;
        }
        
        /// <summary>
        /// Create a user object from JSON object.
        /// </summary>
        /// <param name="json">JSON object</param>
        public static User CreateFromJson(JsonObject json)
        {
            User user = new User();
            user.username       = json["username"];
            user.nickname       = json["nickname"];
            user.isAdmin        = json["isAdmin"];
            user.isSuperAdmin   = json["isSuperAdmin"];
            user.registerTime   = json["registerTime"];
            user.lastLoginTime  = json["lastLoginTime"];
            user.lastChatTime   = json["lastChatTime"];
            user.muteTime       = json["muteTime"];
            user.banTime        = json["banTime"];
            return user;
        }

        /// <summary>
        /// Convert a user object into JSON.
        /// </summary>
        public JsonObject ToJsonObject()
        {
            JsonObject json = new JsonObject();
            json.Add("username"     , username);
            json.Add("nickname"     , nickname);
            json.Add("isAdmin"      , isAdmin);
            json.Add("isSuperAdmin" , isSuperAdmin);
            json.Add("registerTime" , registerTime);
            json.Add("lastLoginTime", lastLoginTime);
            json.Add("lastChatTime" , lastChatTime);
            json.Add("muteTime"     , muteTime);
            json.Add("banTime"      , banTime);
            return json;
        }
    }

}
