using System.Collections.Generic;

namespace Hardware.Info.Windows
{
    internal interface IWmiQueryProvider
    {
        IEnumerable<IWmiPropertySource> Query(string scope, string query);

        IEnumerable<IWmiPropertySource> QueryRelated(string scope, IWmiPropertySource wmiPropertySource, string relatedClass);
    }
}
