using CommandLine;

namespace LDAPSyncTool 
{
    
    public class Options
    {
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        [Option('c',"configuration",Required=false,HelpText="The configuration file to use. Defaults to LDAPSyncTool.yaml in the same folder where the program is")]
        public string ConfigFile { get; set; }

    }
}