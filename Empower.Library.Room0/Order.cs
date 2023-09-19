﻿namespace Skyline.DataMiner.Empower.Library.Room0
{
    using Nito.AsyncEx.Synchronous;

    using Skyline.DataMiner.CICD.Tools.WinEncryptedKeys.Lib;

    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///  Allows dispatching of an order to be sent over HTTPS to DataMiner.
    /// </summary>
    internal class Order : IOrder
    {
        private const string endpoint = "https://solutions.skyline.be/api/custom/operations/order";
        private static readonly string keyName = "SLC_EXTERNAL_DISPATCHER_KEY";

        private readonly SecureString apiKey;
        private readonly string suffix;
        private readonly string name;

        /// <summary>
        /// Creates an instance of <see cref="Order"/>.
        /// </summary>
        /// <param name="orderValue">Value of the order.</param>
        public Order(string orderValue)
        {
            // Key was setup using the dotnet tool Skyline.DataMiner.CICD.Tools.WinEncryptedKeys
            apiKey = Keys.RetrieveKey(keyName);
            name = orderValue;
            suffix = " from room 'this is a nothing'";
        }

        /// <summary>
        /// Dispatches this job and waits on result.
        /// REQUIRED - A user secret holding the GUID to log to DataMiner is required.
        /// Run the commands on every server you want to provide access: 
        /// dotnet tool install --global Skyline.DataMiner.CICD.Tools.WinEncryptedKeys
        /// WinEncryptedKeys --name SLC_EXTERNAL_DISPATCHER_KEY --value MYGUID
        /// </summary>
        /// <param name="user">The user dispatching the job.</param>
        /// <returns>True if the dispatch was successful, False if logging failed.</returns>
        public bool Dispatch(string user)
        {
            Task<bool> task = DispatchAsync(user);
            return task.WaitAndUnwrapException();
        }

        /// <summary>
        /// Dispatches this job without waiting on a result.
        /// Run the commands on every server you want to provide access: 
        /// dotnet tool install --global Skyline.DataMiner.CICD.Tools.WinEncryptedKeys
        /// WinEncryptedKeys --name SLC_EXTERNAL_DISPATCHER_KEY --value MYGUID
        /// </summary>
        /// <param name="user">The user dispatching the job.</param>
        /// <returns>True if the dispatch was successful, False if logging failed.</returns>
        public async Task<bool> DispatchAsync(string user)
        {
            var url = endpoint;
            string data = "User " + user + " placed order " + name + suffix;

            using (var client = new HttpClient())
            {
                var content = new StringContent(data, Encoding.UTF8, "text/plain");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new NetworkCredential("", apiKey).Password);
                var response = await client.PostAsync(url, content).ConfigureAwait(false);

                return response.IsSuccessStatusCode;
            }
        }
    }
}
