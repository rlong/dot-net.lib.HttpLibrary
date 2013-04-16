// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.common.log;
using jsonbroker.library.common.json;
using jsonbroker.library.common.http.headers.request;
using jsonbroker.library.service.configuration;
using jsonbroker.library.common.auxiliary;

namespace jsonbroker.library.common.security
{
    public class SecurityConfiguration : ClientSecurityConfiguration, ServerSecurityConfiguration
    {
        private static Log log = Log.getLog(typeof(SecurityConfiguration));




        public static readonly SecurityConfiguration TEST = buildTestConfiguration();


        //////////////////////////////////////////////////////
	    private String _identifier;

        ////////////////////////////////////////////////////////////////////////////
        //
        ConfigurationService _configurationService;

        ////////////////////////////////////////////////////////////////////////////
        //
        Dictionary<String, Subject> _clients;


        ////////////////////////////////////////////////////////////////////////////
        //
        Dictionary<String, Subject> _servers;



        ////////////////////////////////////////////////////////////////////////////
        //
        private SecurityConfiguration(String identifier, ConfigurationService configurationService)
        {
            _identifier = identifier;
            _configurationService = configurationService;

            _clients = new Dictionary<string, Subject>();
            _servers = new Dictionary<string, Subject>();

        }

        private SecurityConfiguration(JsonObject value, ConfigurationService configurationService)
        {

            _identifier = value.GetString("identifier");
            _configurationService = configurationService;

            _clients = new Dictionary<string, Subject>();
            _servers = new Dictionary<string, Subject>();


            { // subjects
                JsonArray registeredSubjects = value.GetJsonArray("subjects");

                for (int i = 0, count = registeredSubjects.Count(); i < count; i++)
                {
                    JsonObject subjectData = registeredSubjects.GetJsonObject(i);

                    String subjectIdentifier = subjectData.GetString("identifier");
                    String subjectPassword = subjectData.GetString("password");
                    String subjectLabel = subjectData.GetString("label");

                    this.addSubject(subjectIdentifier, subjectPassword, subjectLabel);
                }
            }

        }

        public static SecurityConfiguration buildTestConfiguration()
        {
            SecurityConfiguration answer = new SecurityConfiguration(Subject.TEST_REALM, null);
            answer.addClient(Subject.TEST);

            return answer;
        }


        public static SecurityConfiguration build(SecurityAdapter identifierProvider, ConfigurationService configurationService)
        {
            JsonObject bundleData = configurationService.getBundle(SimpleSecurityAdapter.BUNDLE_NAME);

            SecurityConfiguration answer = null;

            if (null != bundleData)
            {
                if (bundleData.Contains("identifier"))
                {
                    answer = new SecurityConfiguration(bundleData, configurationService);
                    return answer;
                }
            }

            String identifer = identifierProvider.getIdentifier();
            log.debug(identifer, "identifer");

            answer = new SecurityConfiguration(identifer, configurationService);
            answer.save(); // ensure we persist the newly created 'identifer'

            return answer;
        }

        private void save()
        {

            if (null == _configurationService)
            {
                log.debug("null == _configurationService");
                return;
            }

            JsonObject bundleData = this.toJsonObject();
            _configurationService.setBundle(SimpleSecurityAdapter.BUNDLE_NAME, bundleData);
            _configurationService.saveAllBundles();

        }

        private void addSubject(String subjectIdentifier, String subjectPassword, String subjectLabel)
        {

            Subject client = new Subject(subjectIdentifier, _identifier, subjectPassword, subjectLabel);
            _clients[subjectIdentifier] = client;

            Subject server = new Subject(_identifier, subjectIdentifier, subjectPassword, subjectLabel);
            _servers[subjectIdentifier] = server;

            this.save();

        }

        private void removeSubject(String subjectIdentifier)
        {

            
            _clients.Remove(subjectIdentifier);
            _servers.Remove(subjectIdentifier);

            this.save();
        }


        public JsonObject toJsonObject()
        {
            JsonObject answer = new JsonObject();
            answer.put("identifier", _identifier);

            JsonArray subjects = new JsonArray();

            foreach (KeyValuePair<String, Subject> kvp in _clients)
            {
                Subject client = kvp.Value;
                JsonObject subjectData = new JsonObject();
                subjectData.put("identifier", client.Username);
                subjectData.put("password", client.Password);
                subjectData.put("label", client.Label);

                subjects.Add(subjectData);
            }

            answer.put("subjects", subjects);

            return answer;


        }

        ////////////////////////////////////////////////////////////////////////////
        // ClientSecurityConfiguration

        ////////////////////////////////////////////////////////////////////////////
        // ServerSecurityConfiguration::create

        public void addServer(Subject server)
        {
            String subjectIdentifier = server.Realm;
            String subjectPassword = server.Password;
            String subjectLabel = server.Label;

            this.addSubject(subjectIdentifier, subjectPassword, subjectLabel);
        }

        ////////////////////////////////////////////////////////////////////////////
        // ClientSecurityConfiguration::read

        public String getUsername()
        {
            return _identifier;
        }

        // can return null
        public Subject getServer(String realm)
        {
            return _servers[realm];
        }

        
        ////////////////////////////////////////////////////////////////////////////	
        // ServerSecurityConfiguration


        ////////////////////////////////////////////////////////////////////////////	
        // ServerSecurityConfiguration::create

        public void addClient(Subject client)
        {
            String subjectIdentifier = client.Username;
            String subjectPassword = client.Password;
            String subjectLabel = client.Label;

            this.addSubject(subjectIdentifier, subjectPassword, subjectLabel);

        }

        ////////////////////////////////////////////////////////////////////////////
        // ServerSecurityConfiguration::read 

        public String getRealm()
        {
            return _identifier;
        }

        // can return null
        public Subject getClient(String clientUsername)
        {
            if (_clients.ContainsKey(clientUsername))
            {
                return _clients[clientUsername];
            }
            return null;            
        }

        public bool hasClient(String clientUsername)
        {
            if (_clients.ContainsKey(clientUsername))
            {
                return true;
            }
            return false;
        }

        public Subject[] getClients()
        {

            // vvv http://stackoverflow.com/questions/197059/convert-dictionary-values-into-array

            Subject[] answer = new Subject[_clients.Count];
            _clients.Values.CopyTo(answer, 0);

            // ^^^ http://stackoverflow.com/questions/197059/convert-dictionary-values-into-array

            return answer;
        }

        ////////////////////////////////////////////////////////////////////////////
        // ServerSecurityConfiguration::delete 
        public void removeClient(String clientUsername)
        {
            this.removeSubject(clientUsername);
        }

    }

}
