﻿using System;
using System.IO;

namespace Ant0nRocket.Lib.Std20.Logging
{
    [Obsolete]
    public class BasicLogWritterEventArgs
    {
        public string LogMessage { get; set; }

        public string LogFileName { get; set; }

        public Stream LogFileStream { get; set; }
    }
}
