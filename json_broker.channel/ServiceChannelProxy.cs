// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using dotnet.lib.Http.json_broker;
using dotnet.lib.CoreAnnex.log;
using dotnet.lib.CoreAnnex.json.output;
using dotnet.lib.CoreAnnex.exception;
using dotnet.lib.CoreAnnex.json;
using dotnet.lib.Http.json_broker.server;

namespace dotnet.lib.Http.json_broker.channel
{
    public class ServiceChannelProxy : Service 
    {


        private static readonly byte[] NEWLINE = { (byte)'\n' };

        
        private static Log log = Log.getLog( typeof( ServiceChannelProxy ) );


        Channel _channel;


        public ServiceChannelProxy(Channel channel)
        {
            _channel = channel;
        }

        private void dispachRequest(BrokerMessage request)
        {

            JsonArray requestArray = request.ToJsonArray();
            byte[] endpointHeader = JsonArrayHelper.ToBytes(requestArray);

            // channel header ...
            {
                String channelHeader = "[\"jsonbroker.JsonbrokerEndpoint\",1,0,null," + endpointHeader.Length + "]\n";
                log.debug(channelHeader, "channelHeader");
                _channel.Write(channelHeader);
            }

            // endpoint header ...
            {
                _channel.Write(endpointHeader);
                _channel.Write(NEWLINE);
            }
        }

        private BrokerMessage readResponse()
        {

            String channelHeader = _channel.ReadLine();
            log.debug(channelHeader, "channelHeader");

            if (null == channelHeader)
            {
                String errorMessage = "null == channelHeader; channel looks like it is closed";

                throw new BaseException(this, errorMessage);
            }

            String endpointHeader = _channel.ReadLine();
            log.debug(endpointHeader, "endpointHeader");

            JsonArray brokerMessageArray = JsonArrayHelper.FromString(endpointHeader);
            return new BrokerMessage(brokerMessageArray);

        }

        public BrokerMessage process(BrokerMessage request)
        {

            dispachRequest(request);
            return readResponse();
        }

    }
}
