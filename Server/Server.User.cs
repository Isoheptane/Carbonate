using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {

        Thread autosaveThread;

        ConcurrentDictionary<string, ServerUser> users = new ConcurrentDictionary<string, ServerUser>();

        /// <value>Users on the server.</value>
        public ConcurrentDictionary<string, ServerUser> Users
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
                    users.TryAdd(user.username, user);
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

        void BeginAutoSave()
        {
            autosaveThread = new Thread(() => 
            {
                DateTime nextSave = DateTime.Now;
                while(serverOn)
                {
                    if(DateTime.Now > nextSave)
                    {
                        nextSave = DateTime.Now.AddSeconds(autosave);
                        Info($"\\8rAuto saved {SaveUserProfiles()} user profiles.");
                    }
                    else
                    {
                        Thread.Sleep(1000);
                    }
                }
            });
            autosaveThread.Start();
        }

    }
}
