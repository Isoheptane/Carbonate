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
        public int permissionLevel;
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
            permissionLevel = 0;
            registerTime    = DateTime.MinValue;
            lastLoginTime   = DateTime.MinValue;
            lastChatTime    = DateTime.MinValue;
            muteTime        = DateTime.MinValue;
            banTime         = DateTime.MinValue;
        }

        /// <summary>
        /// Create a user object from another user object.
        /// </summary>
        /// <param name="user">Another user object to be copied</param>
        public User(User user)
        {
            username        = user.username;
            nickname        = user.nickname;
            permissionLevel = user.permissionLevel;
            registerTime    = user.registerTime;
            lastLoginTime   = user.lastLoginTime;
            lastChatTime    = user.lastChatTime;
            muteTime        = user.muteTime;
            banTime         = user.banTime;
        }
        
        /// <summary>
        /// Create a user object from JSON object.
        /// </summary>
        /// <param name="json">JSON object</param>
        public static User CreateFromJson(JsonObject json)
        {
            User user = new User();
            user.username           = json["username"];
            user.nickname           = json["nickname"];
            user.permissionLevel    = json["permissionLevel"];
            user.registerTime       = json["registerTime"];
            user.lastLoginTime      = json["lastLoginTime"];
            user.lastChatTime       = json["lastChatTime"];
            user.muteTime           = json["muteTime"];
            user.banTime            = json["banTime"];
            return user;
        }

        /// <summary>
        /// Convert a user object into JSON.
        /// </summary>
        public JsonObject ToJsonObject()
        {
            JsonObject json = new JsonObject();
            json.Add("username"         , username);
            json.Add("nickname"         , nickname);
            json.Add("permissionLevel"  , permissionLevel);
            json.Add("registerTime"     , registerTime);
            json.Add("lastLoginTime"    , lastLoginTime);
            json.Add("lastChatTime"     , lastChatTime);
            json.Add("muteTime"         , muteTime);
            json.Add("banTime"          , banTime);
            return json;
        }
    }

    public class ServerUser : User
    {
        public string password;

        /// <summary>
        /// Create a empty server-side user object.
        /// </summary>
        public ServerUser() : base()
        {
            password = "";
        }

        /// <summary>
        /// Create a server-side user object with a user object.
        /// </summary>
        /// <param name="user">User object template</param>
        public ServerUser(User user) : base(user)
        {
            password = "";
        }

        /// <summary>
        /// Create a server-side user object from another server-side user object.
        /// </summary>
        /// <param name="user">Server-side user object template</param>
        public ServerUser(ServerUser user) : base((User)user)
        {
            password = user.password;
        }

        /// <summary>
        /// Create a server-side user object from JSON object.
        /// </summary>
        /// <param name="json">JSON object</param>
        public new static ServerUser CreateFromJson(JsonObject json)
        {
            ServerUser serverUser = new ServerUser(User.CreateFromJson(json));
            serverUser.password = json["password"];
            return serverUser;
        }

        /// <summary>
        /// Convert a server-side user object into JSON.
        /// </summary>
        public new JsonObject ToJsonObject()
        {
            JsonObject json = ((User)this).ToJsonObject();
            json.Add("password", password);
            return json;
        }
    }

}
