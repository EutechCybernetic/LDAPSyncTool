using System;
using Novell.Directory.Ldap;
using System.Collections.Generic;
using System.Linq;
namespace LDAPSyncTool
{
    public class LDAP
    {
        private LDAPConfig Configuration {get;set;}
        public LDAP(LDAPConfig config)
        {
            this.Configuration = config;
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
        public IEnumerable<IDictionary<string,object>> Query()
        {
            var server  = this.Configuration.Server;
            var uid = this.Configuration.User;
            var pwd = this.Configuration.Password;
            var batchSize = this.Configuration.BatchSize;
            if (batchSize < 1) {
                Log.Debug("Defaulting to batch size of {0}",1000);
                batchSize =1000;
            }

            //TODO: use batch size

            var cs = this.Configuration.Dn;
            var attrs = this.Configuration.Attributes;
            string[] attrList = this.Configuration.GetAllAttributes ? null : this.Configuration.Attributes.Keys.ToArray();
           
            using (var cn = new LdapConnection())
            {
                cn.Connect(server, 389);
                cn.Bind(uid,pwd);

                Log.Debug("Connected");
                var sc = cn.SearchConstraints;
                sc.BatchSize = batchSize;
                Log.Debug("Using batch size: {0}",batchSize);
                var lr = cn.Search(cs, LdapConnection.ScopeSub, this.Configuration.Query, attrList, false);

                while (lr.HasMore())
                {

                    var entry = lr.Next();
                    var entrySet = entry.GetAttributeSet();
                    Log.Debug("Entry: {0}", getEntryAsString(entry));
                    var user = new Dictionary<string,object>(StringComparer.CurrentCultureIgnoreCase);
                    var isValidEntry = false;
                    foreach (var kvp in attrs)
                    {
                        if (!entrySet.ContainsKey(kvp.Key))
                        {
                            continue;
                        }
                        var ea = entrySet[kvp.Key];
                        if (ea == null) continue;
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