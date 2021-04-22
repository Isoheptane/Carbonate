using System;
using Carbonate.Standard;
using static Carbonate.Standard.ScreenIO;

namespace Carbonate.Server
{
    public partial class Server
    {
        bool serverOn;      //< Server is On
        string name;        //< Server Name
        string[] description; //< Server Description
        int maxOnline;      //< Max Online User Count
        int port;           //< Server Port
        string workspace;   //< Workspace Directory

        /// <value>Is the server running</value>
        public bool Running
        {
            get { return serverOn; }
        }

        /// <value>The name of the server.</value>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <value>The description of the server.</value>
        public string[] Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <value>The max online user count of the server.</value>
        public int MaxOnline
        {
            get { return maxOnline; }
            set { maxOnline = value; }
        }

        /// <value>The port of the server.</value>
        public int Port
        {
            get { return port; }
            set
            {
                if(Running)
                    throw new Exception("Server is running, port is immutable.");
                else
                    port = value;
            }
        }

        /// <value>The workspace directory of the server.</value>
        public string WorkspaceDirectory
        {
            get { return workspace; }
        }
        
        /// <summary>
        /// Create a empty object.
        /// </summary>
        public Server() {}
        
        /// <summary>
        /// Create a server object with indicated information.
        /// </summary>
        /// <param name="maxOnline">Max online user count</param>
        /// <param name="workspace">Workspace directory, where the files are stored</param>
        public Server(
            string name,
            string[] description,
            int maxOnline,
            int port,
            string workspace
        )
        {
            this.name           = name;
            this.description    = description;
            this.maxOnline      = maxOnline;
            this.port           = port;
            this.workspace      = workspace;
        }
        
        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            serverOn = true;            //< Set tag
            Info("Starting server...");
            Info("Server Information:");
            Write($"  Name: {Name}\n");
            Write("  Description:\n");
            foreach(string lines in Description)
                Write($"  - {lines}\n");
            Write($"  Max Online: {MaxOnline}\n");
            Write($"  Port: {Port}\n");
            Write($"  Workspace Directory: {WorkspaceDirectory}\n");

            LoadUserProfiles();
            Info($"{users.Count} user profiles loaded.");
            Info($"Server started at {CurrentTimeString}.");
        }

        /// <summary>
        /// Stop the server.
        /// </summary>
        public void Stop()
        {
            serverOn = false;
            Info($"{SaveUserProfiles()} user profiles saved.");
            Info($"Server stopped at {CurrentTimeString}.");
        }

    }
}
