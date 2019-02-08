using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class CopyMemberExtractor
    {
        private static BindingFlags bindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static IEnumerable<(MemberInfo, InnerCopyPolicy)> Extract<T>() =>
            Extract(typeof(T));

        public static IEnumerable<(MemberInfo, InnerCopyPolicy)> Extract(Type type)
        {
            IEnumerable<(MemberInfo, Type, CopyMemberAttribute)> targets = null;

            if (type.GetCustomAttribute(typeof(CloneableAttribute)) == null)
            {
                targets = GetFields(type);
            }
            else
            {
                targets = GetFieldsWithAttribute(type)
                    .Concat(GetPropertiesWithAttribute(type))
                    .Where(t => t.Item3 != null);
            }

            return targets.Select(t =>
                (t.Item1, Seal(t.Item2, t.Item3)));
        }

        private static IEnumerable<(MemberInfo, Type, CopyMemberAttribute)> GetFields(Type type) =>
            TypeUtils.GetFields(type, bindingFlags)
                .Select(x => ((MemberInfo)x, x.FieldType, (CopyMemberAttribute)null));

        private static IEnumerable<(MemberInfo, Type, CopyMemberAttribute)> GetFieldsWithAttribute(Type type) =>
            TypeUtils.GetFields(type, bindingFlags)
                .Select(x => ((MemberInfo)x, x.FieldType, GetCopyMemberAttribute(x)));

        private static IEnumerable<(MemberInfo, Type, CopyMemberAttribute)> GetPropertiesWithAttribute(Type type) =>
            type.GetProperties(bindingFlags)
                .Select(x => ((MemberInfo)x, x.PropertyType, GetCopyMemberAttribute(x)));

        private static CopyMemberAttribute GetCopyMemberAttribute(MemberInfo memberInfo) =>
            (CopyMemberAttribute)memberInfo.GetCustomAttribute(typeof(CopyMemberAttribute));

        private static InnerCopyPolicy Seal(Type type, CopyMemberAttribute attribute)
        {
            if (TypeUtils.IsValueType(type))
            {
                return InnerCopyPolicy.Assign;
            }
            if (type.IsArray)
            {
                if (attribute?.CopyPolicy == CopyPolicy.DeepCopy)
                    return InnerCopyPolicy.DeepCopy;
                if (attribute?.CopyPolicy == CopyPolicy.ShallowCopy)
                    return InnerCopyPolicy.ShallowCopy;
                if (attribute?.CopyPolicy == CopyPolicy.Assign)
                    return InnerCopyPolicy.Assign;
                    
                return (TypeUtils.IsValueType(type.GetElementType())
                        ? InnerCopyPolicy.ShallowCopy
                        : InnerCopyPolicy.DeepCopy);
            }

            if (attribute?.CopyPolicy == CopyPolicy.ShallowCopy)
                return InnerCopyPolicy.MemberwiseClone;
            if (attribute?.CopyPolicy == CopyPolicy.Assign)
                return InnerCopyPolicy.Assign;
            return InnerCopyPolicy.DeepCopy;
        }
    }
}