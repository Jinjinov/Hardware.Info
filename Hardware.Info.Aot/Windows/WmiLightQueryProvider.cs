using System;
using System.Collections.Generic;
using WmiLight;

namespace Hardware.Info.Windows
{
    internal class WmiLightQueryProvider : IWmiQueryProvider
    {
        private readonly Dictionary<string, WmiConnection> _wmiConnectionDict = new Dictionary<string, WmiConnection>();

        private readonly TimeSpan? _enumerationOptionsTimeout;

        public WmiLightQueryProvider(TimeSpan? enumerationOptionsTimeout = null)
        {
            _enumerationOptionsTimeout = enumerationOptionsTimeout;
        }

        public IEnumerable<IWmiPropertySource> Query(string scope, string query)
        {
            if (!_wmiConnectionDict.TryGetValue(scope, out WmiConnection? wmiConnection))
            {
                wmiConnection = new WmiConnection(scope);
                _wmiConnectionDict[scope] = wmiConnection;
            }

            if (_enumerationOptionsTimeout.HasValue)
            {
                foreach (WmiObject mo in wmiConnection.CreateQuery(query, _enumerationOptionsTimeout.Value))
                    yield return new WmiLightObjectAdapter(mo);
            }
            else
            {
                foreach (WmiObject mo in wmiConnection.CreateQuery(query))
                    yield return new WmiLightObjectAdapter(mo);
            }
        }

        public IEnumerable<IWmiPropertySource> QueryRelated(string scope, IWmiPropertySource wmiPropertySource, string relatedClass)
        {
            if (!_wmiConnectionDict.TryGetValue(scope, out WmiConnection? wmiConnection))
            {
                wmiConnection = new WmiConnection(scope);
                _wmiConnectionDict[scope] = wmiConnection;
            }

            WmiLightObjectAdapter? wmiLightObjectAdapter = wmiPropertySource as WmiLightObjectAdapter;

            if (wmiLightObjectAdapter is null)
                yield break;

            WmiObject wmiObject = wmiLightObjectAdapter.GetWmiObject();

            if (_enumerationOptionsTimeout.HasValue)
            {
                foreach (WmiObject mo in wmiConnection.CreateQueryForRelated(wmiObject, relatedClass, _enumerationOptionsTimeout.Value))
                    yield return new WmiLightObjectAdapter(mo);
            }
            else
            {
                foreach (WmiObject mo in wmiConnection.CreateQueryForRelated(wmiObject, relatedClass))
                    yield return new WmiLightObjectAdapter(mo);
            }
        }
    }
}
