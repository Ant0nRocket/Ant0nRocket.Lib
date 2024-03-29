﻿using Ant0nRocket.Lib.Serialization;
using System;
using System.IO;
using System.Linq;

namespace Ant0nRocket.Lib
{
    /// <summary>
    /// Configuration of a <see cref="Lib"/>.
    /// </summary>
    public class Ant0nRocketLibConfig
    {
        #region IsPortableMode

        private static bool? isPortableMode = null;


        /// <summary>
        /// Set this flag to true if you want that library "think" that
        /// the application is in portable mode (for example, function
        /// <see cref="IO.FileSystemUtils.TryReadFromFile{T}"/> depends on it).
        /// </summary>
        public static bool IsPortableMode
        {
            get => isPortableMode ?? GetIsPortableModeFlagFromEnvironment();
            set => isPortableMode = value;
        }

        /// <summary>
        /// Automatically checks existance of the following conditions:<br />
        /// 1) Is ".portable" file exists near main executable?<br />
        /// 2) Is there a command line flag "--portable"?<br />
        /// 3) Are the application started from folder that contains "/Debug/" in name?<br />
        /// If any of those conditions are true - portable mode will be set to true.
        /// </summary>
        private static bool GetIsPortableModeFlagFromEnvironment()
        {
            var appBaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            var portableFilePath = Path.Combine(appBaseDirectoryPath, ".portable");

            var specialFilePortableFlag = File.Exists(portableFilePath);
            var commandLinePortableFlag = Environment.GetCommandLineArgs().Contains("--portable");

            return specialFilePortableFlag || commandLinePortableFlag;
        }

        #endregion

        #region Json serializer

        private static IJsonSerializer? _jsonSerializer;

        /// <summary>
        /// Returnes registred with <see cref="RegisterJsonSerializer(IJsonSerializer)"/> JSON serializer.
        /// </summary>
        /// <exception cref="ApplicationException">When no serializer were registred</exception>
        public static IJsonSerializer GetJsonSerializer()
        {
            if (_jsonSerializer == null)
                throw new ApplicationException($"Call '{nameof(RegisterJsonSerializer)}' first, current serializer is null");
            return _jsonSerializer;
        }

        /// <summary>
        /// Simply registers <paramref name="jsonSerializer"/> as a new library-wide JSON serializer.
        /// </summary>
        public static void RegisterJsonSerializer(IJsonSerializer jsonSerializer) =>
            _jsonSerializer = jsonSerializer;

        #endregion
    }
}
