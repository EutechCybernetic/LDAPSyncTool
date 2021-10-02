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

        static void Run(Options o)
        {
            try
            {
                 
                Log.LogLevel = o.Verbose ? 0 : 1;
                var configFile = string.IsNullOrWhiteSpace(o.ConfigFile)? GetDefaultConfigFile() : o.ConfigFile;

                Log.Debug($"Using configuration file: {configFile}");

                var config = ConfigParser.Parse(configFile);

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

                var ldap = new LDAP(config.ldap);
                foreach(var user in ldap.Query())
                {
                    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(user));
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
