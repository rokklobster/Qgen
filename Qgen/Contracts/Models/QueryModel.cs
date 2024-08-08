namespace Qgen.Contracts.Models;


/// <summary>
/// object specifying what data to query
/// </summary>
public interface QueryModel
{
    /// <summary>
    /// items to skip
    /// </summary>
    public uint Skip { get; }

    /// <summary>
    /// amount of items to take
    /// </summary>
    public uint Take { get; }

    /// <summary>
    /// string to search for in fields available for searching
    /// </summary>
    public string? SearchQuery { get; }

    /// <summary>
    /// fields to include for operations (todo: and for output)
    /// </summary>
    public string[]? Include { get; }

    /// <summary>
    /// sort definitions in order of application
    /// </summary>
    public Ordering[]? OrderBy { get; }

    /// <summary>
    /// definitions of fields to use for grouping in order of application
    /// </summary>
    public string[]? GroupBy { get; }

    /// <summary>
    /// filter definitions to apply to query. Complex expression are allowed using compositions
    /// </summary>
    public FilterComposition? Filters { get; }
}
