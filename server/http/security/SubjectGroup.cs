﻿// Copyright (c) 2013 Richard Long & HexBeerium
//
// Released under the MIT license ( http://opensource.org/licenses/MIT )
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using jsonbroker.library.common.http;
using jsonbroker.library.common.log;
using jsonbroker.library.common.http.headers;
using jsonbroker.library.common.http.headers.request;
using jsonbroker.library.server.http;
using jsonbroker.library.common.security;

namespace jsonbroker.library.server.http.security
{
    public class SubjectGroup
    {
        private static Log log = Log.getLog(typeof(SubjectGroup));

        ///////////////////////////////////////////////////////////////////////
        Dictionary<String, Subject> _subjectDictionary;

        protected Dictionary<String, Subject> subjectDictionary
        {
            get { return _subjectDictionary; }
            set { _subjectDictionary = value; }
        }

        ///////////////////////////////////////////////////////////////////////
        public SubjectGroup()
        {
            _subjectDictionary = new Dictionary<String, Subject>();

        }

        ///////////////////////////////////////////////////////////////////////


        public Dictionary<String, Subject>.ValueCollection subjects()
        {
            Dictionary<String, Subject>.ValueCollection answer = _subjectDictionary.Values;
            return answer;
        }

        public int count()
        {
            return _subjectDictionary.Count;
        }

        public bool contains(String username)
        {
            if (_subjectDictionary.ContainsKey(username))
            {
                return true;
            }
            return false;
        }

        // will throw an exception for a bad 'username'
        public Subject getSubject(String username)
        {
            
            if (!_subjectDictionary.ContainsKey(username))
            {
                log.errorFormat("!_subjectDictionary.ContainsKey(username); username = '{0}'", username);
                throw HttpErrorHelper.unauthorized401FromOriginator(this);
            }

            Subject answer = _subjectDictionary[username];
            return answer;
        }


        public void addSubject(Subject subject)
        {
            String username = subject.Username;

            _subjectDictionary[username] = subject;
        }

        public void removeSubject(String username)
        {
            _subjectDictionary.Remove(username);
        }


    }
}
