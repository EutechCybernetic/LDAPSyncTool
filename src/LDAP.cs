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
        public IEnumerable<IDictionary<string,object>> Query()
        {
            var server  = this.Configuration.Server;
            var uid = this.Configuration.User;
            var pwd = this.Configuration.Password;

            var cs = this.Configuration.Dn;
            var attrs = this.Configuration.Attributes;
            using (var cn = new LdapConnection())
            {
                cn.Connect(server, 389);
                cn.Bind(uid,pwd);

                Log.Debug("Connected");

                var lr = cn.Search(cs, LdapConnection.ScopeSub, this.Configuration.Query, this.Configuration.Attributes.Keys.ToArray(), false);
                
                while (lr.HasMore())
                {

                    var entry = lr.Next();
                    var entrySet = entry.GetAttributeSet();
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
                        user[kvp.Key] = ea.StringValue;
                        if (string.IsNullOrEmpty(ea.StringValue) && ea.ByteValue.Length!=0)
                        {
                             user[kvp.Key] = ea.ByteValue;
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