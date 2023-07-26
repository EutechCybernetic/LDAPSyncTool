using System;
using Novell.Directory.Ldap;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LDAPSyncTool
{
    public class LDAP
    {
        private LDAPConfig Configuration {get;set;}
        static Regex LDAPServerRegex = new Regex(@"^(ldap(?:s)?://)?(?<host>[^:/]+)(?::(?<port>\d+))?$", RegexOptions.IgnoreCase);
        bool _IsSecured = false;
        string _Host = null;
        int _Port = 0;
        public LDAP(LDAPConfig config)
        {
            this.Configuration = config;

            ParseLDAPServerAddress(this.Configuration.Server, out this._IsSecured, out this._Host, out this._Port);
        }

        private static string getEntryAsString(LdapEntry entry)
        {
            var attrs = entry.GetAttributeSet();
            var result = new Dictionary<string, object>();

            foreach(var attr in attrs)
            {
                result[attr.Name] = attr.StringValue;
            }

            return result.ToJson();
        }

        /*
            unsecured
            
            ldap.jumpcloud.com
            ldap.jumpcloud.com:389

            secured
            
            ldaps://ldap.jumpcloud.com
            ldaps://ldap.jumpcloud.com:636
        */
        static void ParseLDAPServerAddress(string addr, out bool isSecured, out string host, out int port) {
            isSecured = false;
            host = "localhost";
            port = 389;

            var match = LDAPServerRegex.Match(addr);

            if (match.Success) {
                isSecured = match.Groups[1].Value == "ldaps://";
                var _host = match.Groups["host"].Value;
                var _port = match.Groups["port"].Value;

                if (!string.IsNullOrWhiteSpace(_host)) {
                    host = _host;
                }

                if (!string.IsNullOrWhiteSpace(_port)) {
                    if (!int.TryParse(_port, out port)) {
                        throw new ArgumentException("Not a valid server port");
                    }
                }
                else { // handle "ldaps://ldap.example.com"
                    port = isSecured ? 636 : port;
                }
            }
        }

        public IEnumerable<IDictionary<string,object>> Query()
        {
            var uid = this.Configuration.User;
            var pwd = this.Configuration.Password;
            var batchSize = this.Configuration.BatchSize;

            if (batchSize < 1) {
                Log.Debug("Defaulting to batchSize size of {0}", 1000);

                batchSize = 1000;
            }

            Log.Debug("Using batch size: {0}", batchSize);

            var searchBase = this.Configuration.Dn;
            var attrs = this.Configuration.Attributes;
            var targetAttributes = this.Configuration.GetAllAttributes 
                ? 
                null 
                : 
                this.Configuration.Attributes.Keys.ToArray();
            var filter = this.Configuration.Query;
            var scope = LdapConnection.ScopeSub;
            var searchOptions = new SearchOptions(searchBase, scope, filter, targetAttributes);

            using (var cn = new LdapConnection {
                SecureSocketLayer = this._IsSecured
            })
            {
                cn.Connect(this._Host, this._Port);
                cn.Bind(uid, pwd);

                Log.Debug("Connected");

                var pageControlHandoer = new SimplePagedResultsControlHandler(cn);

                var ldapEntries = pageControlHandoer.SearchWithSimplePaging(searchOptions, batchSize);

                foreach (var entry in ldapEntries)
                {
                    var entrySet = entry.GetAttributeSet();
                    var user = new Dictionary<string,object>(StringComparer.CurrentCultureIgnoreCase);
                    var isValidEntry = false;

                    Log.Debug("Entry: {0}", getEntryAsString(entry));
                    
                    foreach (var kvp in attrs)
                    {
                        if (!entrySet.ContainsKey(kvp.Key))
                        {
                            continue;
                        }

                        var ea = entrySet[kvp.Key];
                        
                        if (ea == null) {
                            continue;
                        }

                        user[kvp.Value] = ea.StringValue;

                        if (string.IsNullOrEmpty(ea.StringValue) && ea.ByteValue.Length!=0)
                        {
                            user[kvp.Value] = ea.ByteValue;
                        }

                        isValidEntry = true;
                    }

                    if (isValidEntry)
                    {
                        yield return user;
                    }
                    else
                    {
                        Log.Debug("Invalid entry: {0} {1}",entry.Dn ,user.ToJson());
                    }
                }

                yield break;
            }
        }
    }
}
