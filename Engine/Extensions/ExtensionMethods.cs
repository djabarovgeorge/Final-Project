using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Engine.Extensions
{
    public static class ExtensionMethods
    {
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }

        public static bool ContainsContent(this string str, string containsIn)
        {
            return str.Contains(containsIn, System.StringComparison.OrdinalIgnoreCase);
        }

        public static bool CompareContent(this string str, string containsIn)
        {
            return str.Trim() == containsIn.Trim();
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return true;
            }

            var collection = enumerable as ICollection<T>;
            if (collection != null)
            {
                return collection.Count < 1;
            }
            return !enumerable.Any();
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue value)
        {
            if (map.ContainsKey(key))
            {
                map[key] = value;
            }
            else
            {
                map.Add(key, value);
            }
        }

    }
}
