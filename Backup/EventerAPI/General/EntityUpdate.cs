using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace EventerAPI.General
{
    public static class EntityUpdate
    {
        public static T EntityUpdateNotNull<T>(T source, T destination)
        {

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                prop.SetValue(destination, prop.GetValue(source) ?? prop.GetValue(destination));
            }

            return destination;
        }
    }
}