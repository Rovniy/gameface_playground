using System.Collections.Generic;
using cohtml.Net;

[CoherentType]
public sealed class HudBindingDemoModel
{
    [CoherentProperty("counter")]
    public int Counter = 1;

    [CoherentProperty("events")]
    public List<HudEventLogItem> Events = new();
}

[CoherentType]
public sealed class HudEventLogItem
{
    [CoherentProperty("id")]
    public int Id;

    [CoherentProperty("text")]
    public string Text;

    [CoherentProperty("damage")]
    public int Damage;

    [CoherentProperty("time")]
    public string Time;

    public HudEventLogItem(int id, string text, int damage, string time)
    {
        Id = id;
        Text = text;
        Damage = damage;
        Time = time;
    }
}

[CoherentType]
public sealed class HudUsername
{
    [CoherentProperty("username")]
    public string Username { get; set; } = "None";
}