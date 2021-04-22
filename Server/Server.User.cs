using System;
using System.IO;
using System.Collections.Generic;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {

        Dictionary<string, ServerUser> users = new Dictionary<string, ServerUser>();

        /// <value>Users on the server.</value>
        public Dictionary<string, ServerUser> Users
        {
            get { return users; }
        }

        void LoadUserProfiles()
        {
            string[] filePaths = Directory.GetFiles($"{workspace}/userdata");
            foreach (string path in filePaths)
            {
                try
                {
                    ServerUser user = ServerUser.CreateFromJson(
                        JsonSharp.JsonObject.Parse(File.ReadAllText(path))
                    );
                    users.Add(user.username, user);
                }
                catch (Exception ex)
                {
                    ScreenIO.Error($"Error occured while loading user profile \"{path}\": {ex.Message}");
                }
            }
        }

        /// <returns>Saved user profile count</returns>
        int SaveUserProfiles()
        {
            int savedCount = 0;
            foreach (var user in users)
            {
                string path = $"{workspace}/userdata/{user.Key}.json";
                try
                {
                    File.WriteAllText(
                        path,
                        user.Value.ToJsonObject().Serialize()
                    );
                    savedCount++;
                }
                catch (Exception ex)
                {
                    ScreenIO.Error($"Error occured while saving user profile \"{path}\": {ex.Message}");
                }
            }
            return savedCount;
        }

    }
}
