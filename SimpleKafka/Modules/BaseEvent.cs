using System;

namespace SimpleKafka.Modules;

public abstract class BaseEvent
{
    public Guid SessionId { get; set; }
}