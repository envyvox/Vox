using System;

namespace Vox.Data.Util;

public interface ICreatedEntity
{
    DateTimeOffset CreatedAt { get; set; }
}