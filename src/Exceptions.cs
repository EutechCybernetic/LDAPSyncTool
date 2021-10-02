using System;
namespace LDAPSyncTool 
{
    public class APIException : Exception
    {
        public APIException(string message) : base(message)
        {
            
        }
    }
}