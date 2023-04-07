using System;

namespace Vox.Data.Util;

public interface IUniqueIdentifiedEntity
{
    Guid Id { get; set; }
}