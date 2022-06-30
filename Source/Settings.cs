using Common.Settings.Types;

namespace TWKVideoTools.Settings;

public class OutputPath : Setting<string, OutputPath>
{
    public override string? DefaultValue => "";
}

public class KeyPath : Setting<string, KeyPath>
{
    public override string? DefaultValue => "";
}

public class Prefix : Setting<string, Prefix>
{
    public override string? DefaultValue => "Output";
}