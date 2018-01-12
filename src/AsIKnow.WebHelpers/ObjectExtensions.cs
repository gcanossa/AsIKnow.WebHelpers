using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AsIKnow.WebHelpers
{
    public static class ObjectExtensions
    {
        public static IEnumerable<string> ToPropertyNameCollection<T>(this Expression<Func<T, object>> ext)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            NewExpression nexpr = ext.Body as NewExpression;

            Type baseType = typeof(T);

            if (nexpr != null)
            {
                foreach (PropertyInfo pinfo in nexpr.Members)
                {
                    PropertyInfo tmp = baseType.GetProperty(pinfo.Name);

                    if (tmp != null)
                        yield return pinfo.Name;
                }
            }
            else
            {
                MemberInfo info = (ext.Body as MemberExpression ?? ((UnaryExpression)ext.Body).Operand as MemberExpression).Member;
                
                PropertyInfo tmp = baseType.GetProperty(info.Name);

                if (tmp != null)
                    yield return info.Name;
            }
        }

        public static Dictionary<string, object> ExludeProperties<T>(this T ext, Expression<Func<T,object>> selector)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string item in typeof(T).GetRuntimeProperties().Select(p=>p.Name).Except(selector.ToPropertyNameCollection()))
            {
                result.Add(item, ext.GetPropValue(item));
            }

            return result;
        }
        public static Dictionary<string, object> SelectProperties<T>(this T ext, Expression<Func<T, object>> selector)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (string item in selector.ToPropertyNameCollection())
            {
                result.Add(item, ext.GetPropValue(item));
            }

            return result;
        }
        public static bool HasPropery(this object ext, string name, Type baseType = null, Type propertyBaseType = null)
        {
            if (ext == null)
                throw new ArgumentNullException(nameof(ext));
            name = name ?? throw new ArgumentNullException(nameof(name));
            baseType = baseType ?? ext.GetType();
            propertyBaseType = propertyBaseType ?? typeof(object);

            PropertyInfo pinfo = baseType.GetProperty(name);
            return pinfo != null && propertyBaseType.IsAssignableFrom(pinfo.PropertyType);
        }
        public static bool HasPropery<T, P>(this T ext, string name)
        {
            return ext.HasPropery(name, typeof(T), typeof(P));
        }
        public static object GetPropValue<T>(this T ext, string name)
        {
            return ext.GetPropValue<T, object>(name);
        }
        public static P GetPropValue<T, P>(this T ext, string name)
        {
            if (!ext.HasPropery<T, P>(name))
                throw new ArgumentException("Property not found.", nameof(name));

            return (P)typeof(T).GetProperty(name).GetValue(ext);
        }
        public static void SetPropValue<T, P>(this T ext, string name, P value)
        {
            if (!ext.HasPropery<T, P>(name))
                throw new ArgumentException("Property not found.", nameof(name));

            typeof(T).GetProperty(name).SetValue(ext, value);
        }
    }
}
