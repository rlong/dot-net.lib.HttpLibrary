// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using jsonbroker.library.server.broker;
using jsonbroker.library.common.http;
using jsonbroker.library.common.log;
using jsonbroker.library.common.broker;
using jsonbroker.library.common.auxiliary;
using jsonbroker.library.common.defaults;

namespace jsonbroker.library.server.http.reqest_handler
{
    public class ServicesRequestHandler : RequestHandler
    {
        private static Log log = Log.getLog(typeof(ServicesRequestHandler));

        private static int _MAXIMUM_REQUEST_ENTITY_LENGTH;


        static ServicesRequestHandler()
        {
            Defaults defaults = DefaultsHelper.GetDefaults("jsonbroker.ServicesRequestHandler");
            _MAXIMUM_REQUEST_ENTITY_LENGTH = defaults.GetInt("MAXIMUM_REQUEST_ENTITY_LENGTH", 32 * 1024);
        }


        private ServicesRegistery _servicesRegistery;


        public ServicesRequestHandler()
        {
            _servicesRegistery = new ServicesRegistery();
        }

        public ServicesRequestHandler(ServicesRegistery servicesRegistery)
        {
            _servicesRegistery = servicesRegistery;
        }



        public void AddService(DescribedService service)
        {
            log.infoFormat("adding service '{0}'", service.getServiceDescription().getServiceName());
            _servicesRegistery.addService(service);
        }


        private static Data GetData(Entity entity)
        {
            if (entity is DataEntity)
            {
                DataEntity dataEntity = (DataEntity)entity;
                return dataEntity.Data;
            }
            Data answer = new Data(entity.getContent(), (int)entity.getContentLength());
            return answer;
        }

        private static BrokerMessage process(ServicesRegistery servicesRegistery, BrokerMessage request)
        {
            try
            {
                return servicesRegistery.process(request);
            }
            catch (Exception e)
            {
                log.error(e);
                return BrokerMessage.buildFault(request, e);
            }
        }



        internal static HttpResponse processPostRequest(ServicesRegistery servicesRegistery, HttpRequest request)
        {

            if (HttpMethod.POST != request.Method)
            {
                log.errorFormat("unsupported method; request.Method = '{0}'", request.Method);
                throw HttpErrorHelper.badRequest400FromOriginator(typeof(ServicesRequestHandler));
            }


            Entity entity = request.Entity;
            if (_MAXIMUM_REQUEST_ENTITY_LENGTH < entity.getContentLength())
            {
                log.errorFormat("_MAXIMUM_REQUEST_ENTITY_LENGTH < entity.getContentLength(); _MAXIMUM_REQUEST_ENTITY_LENGTH = {0}; entity.getContentLength() = {1}", _MAXIMUM_REQUEST_ENTITY_LENGTH, entity.getContentLength());
                throw HttpErrorHelper.requestEntityTooLarge413FromOriginator(typeof(ServicesRequestHandler));
            }

            Data data = GetData(entity);

            BrokerMessage call = Serializer.deserialize(data);
            BrokerMessage response = process(servicesRegistery, call);

            HttpResponse answer;
            {
                if (BrokerMessageType.ONEWAY == call.getMessageType())
                {
                    answer = new HttpResponse(HttpStatus.NO_CONTENT_204);
                }
                else
                {
                    Data responseData = Serializer.Serialize(response);
                    Entity responseBody = new DataEntity(responseData);
                    answer = new HttpResponse(HttpStatus.OK_200, responseBody);
                }
            }

            return answer;


        }


        public HttpResponse processRequest(HttpRequest request)
        {
            return processPostRequest(_servicesRegistery, request);
        }


        public String getProcessorUri()
        {
            return "/services";
        }

    }
}
