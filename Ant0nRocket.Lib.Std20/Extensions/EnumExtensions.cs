using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Ant0nRocket.Lib.Std20.Reflection;
using System.ComponentModel;

namespace Ant0nRocket.Lib.Std20.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returnes an IEnumerable[T] of all posible values of specified enum.
        /// </summary>
        public static IEnumerable<T> GetPosibleValues<T>(this T enumTypeValue) where T : struct
        {
            var enumType = enumTypeValue.GetType();
            if (!enumType.IsEnum)
                throw new ArgumentException($"{nameof(enumTypeValue)} must be Enum");

            return Enum.GetValues(enumType).Cast<T>();
        }

        public static Dictionary<T, string> GetDescriptions<T>(this T enumTypeValue) where T : struct
        {
            var enumType = enumTypeValue.GetType();
            if (!enumType.IsEnum)
                throw new ArgumentException($"{nameof(enumTypeValue)} must be Enum");

            var result = new Dictionary<T, string>();

            var posibleValues = enumTypeValue.GetPosibleValues();
            foreach (var value in posibleValues)
            {
                var m = value.GetType()
                    .GetMembers()
                    .Where(m => m.MemberType == System.Reflection.MemberTypes.Field && m.Name != "value__");

                foreach (var member in m)
                {
                    var descriptionAttr = AttributeUtils.GetAttribute<DescriptionAttribute>(member.GetType());
                    var description = descriptionAttr?.Description ?? member.Name;
                    result.Add(value, description);
                }
            }

            return result;
        }
    }
}
