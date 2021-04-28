using System;
using System.Net;
using System.Net.Sockets;
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
        bool allowRegister; //< Allow register
        int autosave;       //< Autosave inverval

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

        /// <value>If the server allows register.</value>
        public bool AllowRegister
        {
            get { return allowRegister; }
            set { allowRegister = value; }
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
            string workspace,
            bool allowRegister,
            int autosave
        )
        {
            this.name           = name;
            this.description    = description;
            this.maxOnline      = maxOnline;
            this.port           = port;
            this.workspace      = workspace;
            this.allowRegister  = allowRegister;
            this.autosave       = autosave;
        }
        
        /// <summary>
        /// Start the server.
        /// </summary>
        public void Start()
        {
            serverOn = true;    //< Set tag
            Info("Starting server...");
            Info("Server Information:");    //< Write Server Info
            Write($"  Name: {Name}\n");
            Write("  Description:\n");
            foreach(string lines in Description)
                Write($"  - {lines}\n");
            Write($"  Max Online: {MaxOnline}\n");
            Write($"  Port: {Port}\n");
            Write($"  Workspace Directory: {WorkspaceDirectory}\n");
            Write($"  Allow Register: {AllowRegister}\n");
            // Initialize listener
            TcpListener listener = new TcpListener(IPAddress.IPv6Any, Port);
            listenerThread = new System.Threading.Thread(() => { StartListener(listener); });
            listenerThread.Name = "Listener Thread";
            listenerThread.Start();

            LoadUserProfiles();
            Info($"{users.Count} user profiles loaded.");
            Info($"Server started at {CurrentTimeString}.");

            AutoSave();
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
