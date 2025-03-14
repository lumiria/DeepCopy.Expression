﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DeepCopy.Internal.Utilities;

namespace DeepCopy.Internal
{
    internal static class CopyMemberExtractor
    {
        private static readonly BindingFlags bindingFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        public static IEnumerable<(MemberInfo, CopyPolicy)> Extract<T>(params string[] ignoreFields) =>
            Extract(typeof(T), ignoreFields);

        public static IEnumerable<(MemberInfo, CopyPolicy)> Extract(Type type, params string[] ignoreFields)
        {
            if (type.GetCustomAttribute(typeof(CloneableAttribute)) == null)
            {
                return GetFields(type)
                    .Where(field => !ignoreFields.Contains(field.Name))
                    .Select(field => ((MemberInfo)field, Seal(field.FieldType, CopyMemberAttribute.Default)));
            }
            else
            {
                return GetFieldsWithAttribute(type)
                    .Concat(GetPropertiesWithAttribute(type))
                    .Where(t => t.Item3 != null)
                    .Select(t => (t.Item1, Seal(t.Item2, t.Item3)));
            }

        }

        private static IEnumerable<FieldInfo> GetFields(Type type) =>
            TypeUtils.GetFields(type, bindingFlags);

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
            if (TypeUtils.IsAssignableType(type) || TypeUtils.IsDelegate(type))
            {
                return CopyPolicy.Assign;
            }

            return attribute.CopyPolicy != CopyPolicy.Default
                ? attribute.CopyPolicy
                : type.IsArray && TypeUtils.IsAssignableType(type.GetElementType())
                    ? CopyPolicy.ShallowCopy
                    : CopyPolicy.DeepCopy;
        }
    }
}