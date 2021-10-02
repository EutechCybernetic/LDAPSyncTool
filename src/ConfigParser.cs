using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using System.Collections.Generic;
namespace LDAPSyncTool 
{
    public class Config
    {
        public LDAPConfig ldap { get; set; }
        public int pageSize {get;set;}
        public IVIVAConfig iviva { get; set; }
    }
    public class LDAPConfig
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Query { get; set; }
        public Dictionary<string,string> Attributes { get; set; }
        public string Dn {get;set;}
    }
    public class IVIVAConfig
    {
        public string Url {get;set;}
        public string ApiKey {get;set;}
    }

    public class ConfigParser
    {
        public static Config Parse(string path)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            return deserializer.Deserialize<Config>(File.ReadAllText(path));

        }
    }
}