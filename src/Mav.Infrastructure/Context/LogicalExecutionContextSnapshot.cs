namespace Mav.Infrastructure.Context;

[Serializable]
public class LogicalExecutionContextSnapshot
{
    public KeyValuePair<string, object>[] Context { get; set; } = [];

    public string[] Stack { get; set; } = [];
}
