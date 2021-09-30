using Novell.Directory.Ldap;
namespace LDAPSyncTool
{
    public class LDAP
    {
        private LDAPConfig Configuration {get;set;}
        public LDAP(LDAPConfig config)
        {
            this.Configuration = config;
        }
        public void Query()
        {
            var server  = this.Configuration.Server;
            var uid = this.Configuration.User;
            var pwd = this.Configuration.Password;
            var cs = this.Configuration.OU;
            using (var cn = new LdapConnection())
            {
                cn.Connect(server, 389);
                cn.Bind(uid,pwd);
                Log.Info("Connected");

                var lr = cn.Search(cs, LdapConnection.ScopeSub, this.Configuration.Query, this.Configuration.Attributes, false);
                while (lr.HasMore())
                {

                    var entry = lr.Next();
                    var um = new Dictionary<string, object>();
                    var entrySet = entry.GetAttributeSet();
                    
                }
            }
        }
    }
}