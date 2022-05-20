using System;

namespace Vox.Data.Util;

public interface IUpdatedEntity
{
    DateTimeOffset UpdatedAt { get; set; }
}