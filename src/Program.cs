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
                Log.Info($"Attributes: {config.ldap.Attributes}");
            }
            catch (System.Exception exp)
            {
                Log.Exception(exp);
            }

        }
        static void Main(string[] args)
        {
             Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Run(o);
                   });
            /*
            var cs = "o=5c946a1460eb472406c26115,dc=jumpcloud,dc=com";
            var uq = "uid=haran,ou=Users,o=5c946a1460eb472406c26115,dc=jumpcloud,dc=com";
            var server = "ldap.jumpcloud.com";
            var user = "haran@ecyber.com";
            var pwd = "Eutech@123";
            var query = "(sAMAccountName=*)";
            var attributes = new string[]{"email"};
            using (var cn = new LdapConnection())
            {
                cn.Connect(server, 389);
                cn.Bind(uq, pwd);
                Console.WriteLine("Connected");
                var lr = cn.Search(cs, LdapConnection.ScopeSub, query, attributes, false);
                while (lr.HasMore())
                {

                    var entry = lr.Next();
                    var um = new Dictionary<string, object>();
                    var entrySet = entry.GetAttributeSet();
                    Console.WriteLine("{0}",entrySet);
                }
            }*/
        }
    }
}
