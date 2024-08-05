﻿namespace Qgen.Declarations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class EnableFeaturesAttribute : Attribute
{
    public bool EnableFiltering { get; set; }
    public bool EnableSorting { get; set; }
    public bool EnableSearching { get; set; }
    public bool EnableGrouping { get; set; }

    /// <summary>
    /// if set to non-null, all queries will be enriched
    /// with ordering on this field with specified direction. 
    /// only one property should have this set to non-null.
    /// </summary>
    public DefaultSorting DefaultSortingDirection { get; set; }

    public CustomMethod? FilterReader {get;set;}
    public CustomMethod? SortReader {get;set;}
    public CustomMethod? SearchReader {get;set;}
    public CustomMethod? GroupReader {get;set;}
}