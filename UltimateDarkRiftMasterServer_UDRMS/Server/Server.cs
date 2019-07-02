using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Threading;

using DarkRift;
using DarkRift.Server;

using UDRMS_Server_Plugin;

namespace UltimateDarkRiftMasterServer_UDRMS
{
    public class Server
    {
        public static DarkRiftServer server;

        /// <summary>
        ///     Main entry point of the server which starts a single server.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            string[] rawArguments = CommandEngine.ParseArguments(string.Join(" ", args));
            string[] arguments = CommandEngine.GetArguments(rawArguments);
            NameValueCollection variables = CommandEngine.GetFlags(rawArguments);

            string configFile;
            if (arguments.Length == 0)
            {
                configFile = "Server.config";
            }
            else if (arguments.Length == 1)
            {
                configFile = arguments[0];
            }
            else
            {
                System.Console.Error.WriteLine("Invalid comand line arguments.");
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
                return;
            }

            ServerSpawnData spawnData;

            try
            {
                spawnData = ServerSpawnData.CreateFromXml(configFile, variables);
            }
            catch (IOException e)
            {
                System.Console.Error.WriteLine("Could not load the config file needed to start (" + e.Message + "). Are you sure it's present and accessible?");
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
                return;
            }
            catch (XmlConfigurationException e)
            {
                System.Console.Error.WriteLine(e.Message);
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
                return;
            }
            catch (KeyNotFoundException e)
            {
                System.Console.Error.WriteLine(e.Message);
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
                return;
            }
            spawnData.PluginSearch.PluginTypes.Add(typeof(Lobby_Plugin));

            //spawnData.Cache.MaxCachedMessages = 1600;
            //////Console.WriteLine(spawnData.Cache.MaxCachedMessages);
            //spawnData.Cache.MaxCachedReaders = 1600;
            //////Console.WriteLine(spawnData.Cache.MaxCachedReaders);
            //spawnData.Cache.MaxCachedWriters = 1600;
            //////Console.WriteLine(spawnData.Cache.MaxCachedWriters);
            //spawnData.Cache.MaxActionDispatcherTasks = 3200;
            //////Console.WriteLine(spawnData.Cache.MaxActionDispatcherTasks);
            //spawnData.Cache.MaxCachedSocketAsyncEventArgs = 6400;
            //////Console.WriteLine(spawnData.Cache.MaxCachedSocketAsyncEventArgs);

            server = new DarkRiftServer(spawnData);

            //spawnData.Server.MaxStrikes
            
            server.Start();

            new Thread(new ThreadStart(ConsoleLoop)).Start();

            //server.PluginManager.GetPluginByType<Lobby_Plugin>().GetLobbysPerPage(5);
            while (true)
            {
                server.DispatcherWaitHandle.WaitOne();
            }
        }

        /// <summary>
        ///     Invoked from another thread to repeatedly execute commands from the console.
        /// </summary>
        static void ConsoleLoop()
        {
            while (true)
            {
                string input = System.Console.ReadLine();

                server.ExecuteCommand(input);
            }
        }
    }
}
