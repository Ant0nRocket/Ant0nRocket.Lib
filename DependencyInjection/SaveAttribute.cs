using System;
using System.Text.RegularExpressions;

namespace Ant0nRocket.Lib.Std20.DependencyInjection
{
    public class SaveAttribute : Attribute
    {
        private string directoryName = string.Empty;

        /// <summary>
        /// When set this value all special chars (like "!", "@", "&", etc.)
        /// will be replaced with nothing. Only discards allowed ("_").
        /// </summary>
        public string DirectoryName
        {
            get => directoryName;
            set => directoryName = Regex.Replace(value, @"[^\w\.]", string.Empty);
        }

        private string fileName = string.Empty;

        /// <summary>
        /// When set this value all special chars (like "!", "@", "&", etc.)
        /// will be replaced with nothing. Only discards allowed ("_").
        /// </summary>
        public string FileName
        {
            get => fileName;
            set => fileName = Regex.Replace(value, @"[^\w\.]", string.Empty);
        }

        public SerializerType SerializerType { get; set; } = SerializerType.Json;


        public SaveAttribute(string fileName, SerializerType serializerType = SerializerType.Json)
        {
            FileName = fileName;
            SerializerType = serializerType;
        }

        public SaveAttribute(string directoryName, string fileName, SerializerType serializerType = SerializerType.Json)
        {
            DirectoryName = directoryName;
            FileName = fileName;
            SerializerType = serializerType;
        }
    }
}
