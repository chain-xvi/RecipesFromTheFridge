using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace RecipesFromTheFridge.Services
{
    class InternetConnectionAvailabilityCheckService
    {
        /// <summary>
        /// Checks internet connection availability.
        /// </summary>
        /// <returns>
        /// true, if there is internet connection.
        /// false, if nope...
        /// </returns>
        public static bool IsInternetAvailable()
        {
            ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
            bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
            return internet;
        }
    }
}
