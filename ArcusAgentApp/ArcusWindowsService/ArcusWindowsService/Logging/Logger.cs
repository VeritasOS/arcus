/******************************************************************************
 * VERITAS:    Copyright (c) 2017 Veritas Technologies LLC.
 * This software is licensed as described in the file LICENSE which is part of this repository    
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Logging
{
    public sealed class Logger
    {
        private static volatile ILog _instance;
        private static object syncRoot = new Object();

        public static ILog Instance
        {
            get
            {
                if (_instance == null) 
                {
                    lock (syncRoot) 
                    {
                        if (_instance == null)
                        {
                            _instance = new Log();

                            _instance.Name = @"BE_ARCUS_LOG";
                            _instance.Description = @"BE ARCUS Logger";
                            _instance.Owner = @"BE ARCUS";
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
