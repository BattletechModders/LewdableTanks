#nullable enable
using HBS.Logging;
using NullableLogging;

namespace LewdableTanks;

internal static class Log
{
    private const string Name = nameof(LewdableTanks);
    internal static readonly NullableLogger Main = NullableLogger.GetLogger(Name, LogLevel.Debug);
}
