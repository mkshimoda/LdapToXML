using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace LdapToXML
{
    class Program
    {
        static void Main(string[] args)
        {

            string toUpdate = (args.Length > 0) ? args[0].ToLower() : String.Empty;
            string filter = ConfigurationManager.AppSettings["filter"];
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);

            string propertiesToUpdate;
            string xmlToUpdate;
            string ldapServer;
            string user;
            string pwd;

            switch (toUpdate)
            {
                case "bws":
                    propertiesToUpdate = "BWSEmployeeProperties";
                    xmlToUpdate = "BWSEmployeeXMLPath";
                    ldapServer = "ldapServerBws";
                    user = "usernameBws";
                    pwd = "passwordBws";
                    break;
                case "city":
                    propertiesToUpdate = "CityEmployeeProperties";
                    xmlToUpdate = "CityEmployeeXMLPath";
                    ldapServer = "ldapServerCity";
                    user = "usernameCity";
                    pwd = "passwordCity";
                    break;
                default:
                    propertiesToUpdate = "TestProperties";
                    xmlToUpdate = "TestXMLPath";
                    ldapServer = "ldapServerTest";
                    user = "usernameTest";
                    pwd = "passwordTest";
                    break;
            }

            string[] properties = ConfigurationManager.AppSettings[propertiesToUpdate].Split(';');
            string xmlPath = ConfigurationManager.AppSettings[xmlToUpdate];
            string server = ConfigurationManager.AppSettings[ldapServer];
            string username = ConfigurationManager.AppSettings[user];
            string password = ConfigurationManager.AppSettings[pwd];

            if (properties.Length > 0 && pageSize > 0 && File.Exists(xmlPath))
            {
                LdapConnection ldapConnection = new LdapConnection(server, username, password);
                ldapConnection.setAndExecuteSearch(filter, pageSize);
                ldapConnection.searchResultsToXml(xmlPath, properties);
            }
        }
    }
}
