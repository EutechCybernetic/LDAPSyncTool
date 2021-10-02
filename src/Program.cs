using System;
using System.Collections.Generic;
using CommandLine;


namespace LDAPSyncTool
{
    class Program
    {
        static string GetProgramPath()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
        static string GetDefaultConfigFile()
        {
            return System.IO.Path.Combine(GetProgramPath(), Constants.CONFIG_FILE_NAME);
        }

        static void LogConfig(Config config)
        {
            Log.Info($"Server: {config.ldap.Server}");
            Log.Info($"User: {config.ldap.User}");
            Log.Info($"Query: {config.ldap.Query}");
            Log.Info($"iviva Url: {config.iviva.Url}");
            Log.Info($"Page Size: {config.pageSize}");
            Log.Info($"Attributes: ");
            if (config.ldap.Attributes==null)
            {
                Log.Info("\t(empty)");
            }
            else
            {
                foreach(var kvp in config.ldap.Attributes)
                {
                    Log.Info($"\t{kvp.Key} => {kvp.Value}");
                }                    
            }
        }


        static void SyncUsers(IList<IDictionary<string,object>> users, Config config, Options options)
        {
            if (options.SimulationMode)
            {
                Log.Info($"Sending {users.Count} users to {config.iviva.Url} using {config.iviva.ApiKey}" );
                foreach(var user in users)
                {
                    Log.Info("\t=>"+user.ToJson());
                }
            }
        }

        static void Run(Options options)
        {
            try
            {
                 
                Log.LogLevel = options.Verbose ? 0 : 1;
                var configFile = string.IsNullOrWhiteSpace(options.ConfigFile)? GetDefaultConfigFile() : options.ConfigFile;

                Log.Debug($"Using configuration file: {configFile}");

                var config = ConfigParser.Parse(configFile);

                if (config.pageSize < 1)
                {
                    Log.Debug($"Page Size not specified. Using default ({Constants.DEFAULT_PAGE_SIZE})");
                    config.pageSize = Constants.DEFAULT_PAGE_SIZE;
                }

                LogConfig(config);

                var ldap = new LDAP(config.ldap);

                var sessionID = System.Guid.NewGuid().ToString();

                Log.Info($"Creating new session: ${sessionID}");

                
                
                var userBatch = new List<IDictionary<string,object>>();
                foreach(var user in ldap.Query())
                {
                    userBatch.Add(user);
                    if (userBatch.Count >= config.pageSize)
                    {
                        SyncUsers(userBatch, config,options);
                        userBatch.Clear();
                    }
                }

                Log.Debug($"Flushing out remaining items");

                if (userBatch.Count > 0)
                {
                    SyncUsers(userBatch, config,options);
                }
            }
            catch (System.Exception exp)
            {
                Log.Exception(exp);
                Environment.ExitCode = -1;
            }

        }
        static void Main(string[] args)
        {
             Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Run(o);
                   });
        }
    }
}
