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
            string[] properties;
            string xmlPath;
            string toUpdate;

            string filter = ConfigurationManager.AppSettings["filter"];
            int pageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pageSize"]);

            if(args.Length > 0)
            {
                toUpdate = args[0].ToLower();
            }
            else
            {
                toUpdate = String.Empty;
            }
            string propertiesToUpdate;
            string xmlToUpdate;

            switch(toUpdate)
            {
                case "bws":
                    propertiesToUpdate = "BWSEmployeeProperties";
                    xmlToUpdate = "BWSEmployeeXMLPath";
                    break;
                case "city":
                    propertiesToUpdate = "CityEmployeeProperties";
                    xmlToUpdate = "CityEmployeeXMLPath";
                    break;
                default:
                    propertiesToUpdate = "TestProperties";
                    xmlToUpdate = "TestXMLPath";
                    break;
            }

            properties = ConfigurationManager.AppSettings[propertiesToUpdate].Split(';');
            xmlPath = ConfigurationManager.AppSettings[xmlToUpdate];

            if(properties.Length > 0 && pageSize > 0 && File.Exists(xmlPath))
            {
                LdapConnection ldapConnection = new LdapConnection();
                ldapConnection.setAndExecuteSearch(filter, pageSize);
                ldapConnection.searchResultsToXml(xmlPath, properties);
            }
        }
    }
}
