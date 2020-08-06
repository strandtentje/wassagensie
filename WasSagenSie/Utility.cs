using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class Utility
{
    public static T DeepClone<T>(this T obj)
    {
        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }


    public static void RemoveByIdentity(this List<Size> list, Size item)
    {
        list.RemoveAll((Size obj) => obj.Identity == item.Identity);
    }
}