using System;
using System.Linq;

namespace Ant0nRocket.Lib.Extensions
{
    /// <summary>
    /// Extensions of a type 'object'.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returnes a JSON (string) value of an <paramref name="obj"/>.
        /// If <paramref name="pretty"/> is true then formatting will be applyed.
        /// </summary>
        public static string AsJson(this object obj, bool pretty = false)
        {
            return Ant0nRocketLibConfig.GetJsonSerializer().Serialize(obj, pretty);
        }

        /// <summary>
        /// Returnes a value (as object) of specified <paramref name="propertyName"/>.<br />
        /// Usefull when you can't handle it directly with compiler
        /// (during deserialization, for example).
        /// </summary>
        /// <exception cref="ArgumentNullException">If <paramref name="obj"/> is null</exception>
        /// <exception cref="ArgumentException">If <paramref name="propertyName"/> is null or empty</exception>
        /// <exception cref="MissingMemberException">If <paramref name="propertyName"/> not found</exception>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException($"{propertyName} is empty or null");

            var objType = obj.GetType();
            var propertyInfo = objType.GetProperties().SingleOrDefault(p => p.Name == propertyName) ??
                throw new MissingMemberException($"Property '{propertyName}' not found in '{objType.Name}'");

            return propertyInfo.GetValue(obj);
        }

        /// <summary>
        /// Sets a value of a <paramref name="propName"/> of <paramref name="obj"/> using  reflection.
        /// </summary>
        public static void SetPropertyValue(this object obj, string propName, object value)
        {
            var property = obj.GetType().GetProperties().SingleOrDefault(p => p.Name == propName) ??
                throw new InvalidOperationException($"there should be a property with name '{propName}'");

            property.SetValue(obj, value);
        }
    }
}
