using System;

namespace Vox.Framework.Autofac;

public class InjectableServiceAttribute : Attribute
{
    public bool IsSingletone { get; set; }
}
