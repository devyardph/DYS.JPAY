
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;


namespace DYS.JPay.Shared.Shared.Extensions
{
	public static class GenericExtensions
	{
		public static IEnumerable<T> ToItems<T>(this IEnumerable<T> sequence)
		{
			return sequence ?? Enumerable.Empty<T>();
		}

		public static bool HasAny<T>(this IEnumerable<T>? sequence)
		{
			return sequence != null && sequence.Any() ? true : false;
		}

        public static bool IsNull<T>(this T value)
		{
			return value is null;
		}


        public static IEnumerable<T> SetValue<T>(this IEnumerable<T> items, Action<T>
      updateMethod)
        {
            foreach (T item in items)
            {
                updateMethod(item);
            }
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ignoreProperties"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static IDictionary<string, IEnumerable<object>> ConvertObjectToDictionary(object o, params string[] ignoreProperties)
        {
            if (o != null)
            {
                if (o is IDictionary)
                    throw new InvalidOperationException($"The input object is already of type {typeof(IDictionary)}");

                var props = TypeDescriptor.GetProperties(o);
                var d = new Dictionary<string, IEnumerable<object>>();
                foreach (var prop in props.Cast<PropertyDescriptor>().Where(x => !ignoreProperties.Contains(x.Name)))
                {
                    var val = prop.GetValue(o);
                    if (val != null)
                    {
                        var type = val.GetType();
                        if (type == typeof(System.String[]))
                        {
                            var data = val as string[];
                            var test = Array.ConvertAll(data, item => item.ToString());
                            d.Add(prop.Name, new List<object>() { string.Join(",", test) });
                        }
                        else
                        {
                            d.Add(prop.Name, new List<object>() { val });
                        }
                    }

                }
                return d;
            }
            return default;
        }


    }
}
