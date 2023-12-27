﻿using System.Diagnostics;
using System.Reflection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace FluentUI.Demo.Shared.Components;

public static class PropertyInfoExtensions
{
    public static IEnumerable<PropertyChildren> GetPropertyChildren(this Type type)
    {
        return type.GetSubProperties()
                   .Select(i => new PropertyChildren(i, 0))
                   .ToArray();
    }

    public static IEnumerable<PropertyInfo> GetSubProperties(this PropertyInfo property)
    {
        return property.PropertyType.GetSubProperties();
    }

    public static IEnumerable<PropertyInfo> GetSubProperties(this Type type)
    {
        var items = type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .Where(m => m.MemberType == MemberTypes.Property)
                        .OrderBy(m => m.Name)
                        .Select(i => (PropertyInfo)i)
                        .ToArray();

        return items;
    }

    /// <summary>
    /// Return True if the specified property type is a Simple Type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSimpleType(this PropertyInfo property)
    {
        return property.PropertyType.IsSimpleType();
    }

    /// <summary>
    /// Return True if the specified type is a Simple Type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSimpleType(this Type type)
    {
        return type.IsPrimitive ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type.IsNullable() &&
                   (
                       Nullable.GetUnderlyingType(type)?.IsPrimitive == true ||
                       Nullable.GetUnderlyingType(type) == typeof(string) ||
                       Nullable.GetUnderlyingType(type) == typeof(decimal)
                   );
    }
}

[DebuggerDisplay("{Item.Name}")]
public class PropertyChildren
{
    public PropertyChildren(PropertyInfo item, int level)
    {
        Id = Identifier.NewId();
        Level = level;
        Item = item;
        Children = item.IsSimpleType()
                 ? null 
                 : item.GetSubProperties()
                       .Select(i => new PropertyChildren(i, level + 1))
                       .ToArray();
    }

    public string Id { get; }

    public int Level { get; }

    public string Summary => GetSummary();

    public PropertyInfo Item { get; }

    public IEnumerable<PropertyChildren>? Children { get; }

    private string GetSummary()
    {
        var property = Item;
        var ns = property.ReflectedType?.Namespace ?? string.Empty;
        var prefix = property.ReflectedType?.FullName?.Substring(ns.Length + 1).Replace("+", ".");
        var commentKey = $"{prefix}.{property.Name}";
        return CodeComments.GetSummary(commentKey);
    }
}