// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;

namespace dotnet.lib.Http.authentication
{
    public interface ServerSecurityConfiguration
    {
        ////////////////////////////////////////////////////////////////////////////
        // ServerSecurityConfiguration::create

        void addClient(Subject client);

        ////////////////////////////////////////////////////////////////////////////
        // ServerSecurityConfiguration::read 
        String getRealm();

        bool hasClient(String clientUsername);

        // can return null
        Subject getClient(String clientUsername);

        Subject[] getClients();

        ////////////////////////////////////////////////////////////////////////////
        // ServerSecurityConfiguration::delete 
        void removeClient(String clientUsername);

    }
}
