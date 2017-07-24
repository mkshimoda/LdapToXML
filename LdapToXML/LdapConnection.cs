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
        public SearchResultCollection searchResults;
        public List<SearchResult> searchResultList;

        public LdapConnection()
        {
            // TODO make constructor take an object to set these credentials
            directoryEntry = new DirectoryEntry("LDAP://ldap.forumsys.com/dc=example,dc=com", "uid=tesla,dc=example,dc=com", "password", AuthenticationTypes.None);
        }

        public void setSearch()
        {
            // TODO add filter to search if specified
            directorySearch = new DirectorySearcher(directoryEntry);
        }

        public void executeSearch()
        {
            // TODO figure out how to get more than 1000 results
            // searchResults = directorySearch.FindAll(); 
            searchResultList = mockSearchResults();
        }

        public void searchResultsToXml()
        {

            string[] properties = { "DisplayName", "Description", "Department", "Office", "OfficePhone", "EmailAddress", "Photo" };

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
            doc.Save(Path.GetFullPath(@"C:\Users\mshimoda\Source\Repos\BWS-Intranet\CMS\xml\MockXml.xml"));
        }

        public List<SearchResult> SearchResultsToList(SearchResultCollection results)
        {
            // TODO replace when using actual data, grab results and map to List<SearchResult>
            return mockSearchResults();
        }

        private List<SearchResult> mockSearchResults()
        {
            List<SearchResult> results = new List<SearchResult>();

            for (int i = 0; i < 10; i++)
            {
                var user = new
                {
                    DisplayName = "user" + i,
                    Description = "description" + i,
                    Department = "department" + i,
                    Office = "office" + i,
                    OfficePhone = "808-555-555" + i,
                    EmailAddress = "usertest" + i + "@sample.com",
                    Photo = "randomstring"
                };
                results.Add(SearchResultFactory.Construct(user));
            }

            return results;
        }

        public String getProperty(string property, SearchResult result)
        {
            if (result.Properties[property].Count > 0)
            {
                return result.Properties[property][0].ToString();
            }
            else
            {
                return null;
            }

        }

    }
}
