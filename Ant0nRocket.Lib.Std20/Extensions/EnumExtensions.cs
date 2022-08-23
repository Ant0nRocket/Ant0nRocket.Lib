using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        /// <summary>
        /// Returnes dictionary that contains enum posible values as key and
        /// descriptions that was set by <see cref="DescriptionAttribute"/> as value.<br />
        /// If description wasn't set then value name will be used as value.
        /// </summary>
        public static Dictionary<T, string> GetValueDescriptionsDict<T>(this T enumTypeValue) where T : struct
        {
            var result = new Dictionary<T, string>();

            var enumType = enumTypeValue.GetType();
            var members = enumType.GetMembers();
            var values = GetPosibleValues(enumTypeValue);

            foreach (var value in values)
            {
                var name = $"{value}";
                var member = members
                    .Where(m => m.Name == name).First() ?? 
                    throw new InvalidOperationException($"Member '{name}' not found in '{enumType.Name}'");

                var descriptionAttribute = (DescriptionAttribute)member
                    .GetCustomAttributes(false)
                    .Where(o => o.GetType() == typeof(DescriptionAttribute))
                    .FirstOrDefault();

                var description = descriptionAttribute?.Description ?? name;

                result.Add(value, description);
            }

            return result;
        }
    }
}
