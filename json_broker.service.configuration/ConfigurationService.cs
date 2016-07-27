// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.Http.json_broker;
using dotnet.lib.Http.json_broker.server;
using dotnet.lib.CoreAnnex.log;
using System.IO;
using System.Security.Permissions;
using System.Security;
using dotnet.lib.CoreAnnex.json.input;
using dotnet.lib.CoreAnnex.json.handlers;
using dotnet.lib.CoreAnnex.json;


namespace dotnet.lib.Http.json_broker.service.configuration
{
    public class ConfigurationService : DescribedService
    {
        private static readonly Log log = Log.getLog(typeof(ConfigurationService));

        public static readonly String SERVICE_NAME = "jsonbroker.ConfigurationService";
        public static readonly ServiceDescription SERVICE_DESCRIPTION = new ServiceDescription(SERVICE_NAME);


        ////////////////////////////////////////////////////////////////////////////
        //
        private readonly Dictionary<String, JsonObject> _bundles;

        ////////////////////////////////////////////////////////////////////////////
        //
        String _rootFolder;

        ////////////////////////////////////////////////////////////////////////////
        //
        public ConfigurationService(String rootFolder)
        {

            _rootFolder = rootFolder;
            log.info(_rootFolder, "_rootFolder");

            // vvv http://stackoverflow.com/questions/5482230/c-sharp-equivalent-of-javas-mkdirs
            Directory.CreateDirectory(_rootFolder);
            // ^^^ http://stackoverflow.com/questions/5482230/c-sharp-equivalent-of-javas-mkdirs

            _bundles = new Dictionary<String, JsonObject>();
        }

        private static bool canRead(String path)
        {
            FileIOPermission readPermission = new FileIOPermission(FileIOPermissionAccess.Read, path);
            if (SecurityManager.IsGranted(readPermission))
            {
                return true;
            }
            return false;
        }

        public bool exists(String path)
        {
            return System.IO.File.Exists(path);
        }

        private String toFilePath( String bundleName ) {

            String filename = bundleName + ".json";
            String answer = Path.Combine(_rootFolder, filename);
            log.debug(answer, "answer");
            return answer;

        }


        public JsonObject getBundle(String bundleName)
        {

            if (_bundles.ContainsKey(bundleName))
            {
                return _bundles[bundleName];
            }

            String filePath = toFilePath(bundleName);

            if (!exists(filePath) || !canRead(filePath))
            {
                return null;
            }

            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            JsonObject answer;
            try {
                answer = JsonObject.Build(fileStream, (int)fileStream.Length);
            }
            finally
            { 
                fileStream.Close();
            }

            _bundles[bundleName] = answer;

            return answer;
        }

        private void saveBundle(String bundleName, JsonObject bundle)
        {
            // String bundleText = bundle.ToString();

            String filePath = toFilePath(bundleName);

            StreamWriter streamWriter = new StreamWriter(filePath);
            try
            {
                streamWriter.Write(bundle.ToString());
            }
            finally
            {
                streamWriter.Close();
            }            
        }

        public void setBundle(String bundleName, JsonObject bundle)
        {
            _bundles[bundleName] = bundle;
        }

        public void saveAllBundles()
        {
            log.enteredMethod();

            foreach (KeyValuePair<String, JsonObject> kvp in _bundles)
            {
                saveBundle(kvp.Key, kvp.Value);
            }
        }


        public BrokerMessage process(BrokerMessage request)
        {
            String methodName = request.getMethodName();

            if ("save_bundles".Equals(methodName))
            {
                this.saveAllBundles();

                return BrokerMessage.buildResponse(request);
            }

            if ("saveBundles".Equals(methodName))
            {
                this.saveAllBundles();

                return BrokerMessage.buildResponse(request);
            }

            if ("getBundle".Equals(methodName))
            {
                JsonObject associativeParamaters = request.GetAssociativeParamaters();
                String bundleName = associativeParamaters.GetString("bundle_name");

                JsonObject bundleValue = getBundle(bundleName);

                BrokerMessage answer = BrokerMessage.buildResponse(request);
                associativeParamaters = answer.GetAssociativeParamaters();
                associativeParamaters.put("bundle_name", bundleName);
                associativeParamaters.put("bundle_value", bundleValue);
                return answer;
            }

            throw ServiceHelper.methodNotFound(this, request);
        }


        public ServiceDescription getServiceDescription()
        {
            return SERVICE_DESCRIPTION;
        }


    }
}
