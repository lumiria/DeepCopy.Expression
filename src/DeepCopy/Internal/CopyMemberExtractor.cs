﻿using System;
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

        public static IEnumerable<(MemberInfo, CopyPolicy)> Extract<T>() =>
            Extract(typeof(T));

        public static IEnumerable<(MemberInfo, CopyPolicy)> Extract(Type type)
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

        private static CopyPolicy Seal(Type type, CopyMemberAttribute attribute)
        {
            if (TypeUtils.IsValueType(type))
            {
                return CopyPolicy.Assign;
            }
            if (type.IsArray)
            {
                if (attribute?.CopyPolicy == CopyPolicy.DeepCopy)
                    return CopyPolicy.DeepCopy;
                if (attribute?.CopyPolicy == CopyPolicy.ShallowCopy)
                    return CopyPolicy.ShallowCopy;
                if (attribute?.CopyPolicy == CopyPolicy.Assign)
                    return CopyPolicy.Assign;
                    
                return (TypeUtils.IsValueType(type.GetElementType())
                        ? CopyPolicy.ShallowCopy
                        : CopyPolicy.DeepCopy);
            }

            if (attribute?.CopyPolicy == CopyPolicy.ShallowCopy)
                return CopyPolicy.ShallowCopy;
            if (attribute?.CopyPolicy == CopyPolicy.Assign)
                return CopyPolicy.Assign;
            return CopyPolicy.DeepCopy;
        }
    }
}