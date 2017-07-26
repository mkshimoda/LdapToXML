using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.Xml.Linq;
using System.IO;

namespace LdapToXML
{
    public class LdapConnection
    {
        public DirectoryEntry directoryEntry;
        public DirectorySearcher directorySearch;
       // public SearchResultCollection searchResults;
        public IEnumerable<SearchResult> searchResultList;

        public LdapConnection(string server, string user, string pass)
        {
            // TODO make constructor take an object to set these credentials
            directoryEntry = new DirectoryEntry("LDAP://" + server, user, pass, AuthenticationTypes.None);
        }

        public void setAndExecuteSearch(string filter, int pageSize)
        {
            directorySearch = new DirectorySearcher(directoryEntry, filter);
            using(directorySearch)
            {
                directorySearch.PageSize = pageSize;
                searchResultList = SafeFindAll(directorySearch);
            }
        }


        public void searchResultsToXml(string filePath, string[] properties)
        {
            XElement allResults = new XElement("Objs");
            var counter = 0;

            foreach (SearchResult result in searchResultList)
            {

                XElement user = new XElement("Obj", new XAttribute("RefId", counter.ToString()),
                    new XElement("TNRef", new XAttribute("RefId", "0")));
                XElement userProperties = new XElement("MS");

                foreach (string prop in properties)
                {
                    var value = getProperty(prop, result);
                    if (value != null)
                    {
                        if (prop == "Photo")
                        {
                            userProperties.Add(new XElement("BA", value, new XAttribute("N", prop)));
                        }
                        else
                        {
                            userProperties.Add(new XElement("S", value, new XAttribute("N", prop)));
                        }
                    }
                    else
                    {
                        userProperties.Add(new XElement("Nil", new XAttribute("N", prop)));
                    }
                }
                user.Add(userProperties);
                allResults.Add(user);
                counter++;
            }
            XDocument doc = new XDocument(allResults);
            doc.Save(Path.GetFullPath(filePath));
        }
  

        // From https://stackoverflow.com/questions/90652/can-i-get-more-than-1000-records-from-a-directorysearcher-in-asp-net 
        public IEnumerable<SearchResult> SafeFindAll(DirectorySearcher search)
        {
            using (SearchResultCollection results = search.FindAll())
            {
                foreach( SearchResult result in results)
                {
                    yield return result;
                }
            }
        }

        public string getProperty(string property, SearchResult result)
        {
            var count = result.Properties[property].Count;
            if (count > 0)
            {
                string value = String.Empty;
                foreach (String val in result.Properties[property])
                {
                    value += val;
                    count--;
                    if(count > 0)
                    {
                        value += ", ";
                    }
                }
                return value;
           
            }
            else
            {
                return null;
            }

        }

    }
}
