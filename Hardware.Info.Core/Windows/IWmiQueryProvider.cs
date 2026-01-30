using System;
using System.Collections.Generic;

namespace Hardware.Info.Windows
{
    internal interface IWmiQueryProvider : IDisposable
    {
        IEnumerable<IWmiPropertySource> Query(string scope, string query);

        IEnumerable<IWmiPropertySource> QueryRelated(string scope, IWmiPropertySource wmiPropertySource, string relatedClass);
    }
}
