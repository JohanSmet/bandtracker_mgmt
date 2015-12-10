using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RestSharp;

namespace BandtrackerMgmt
{
    class BandTrackerClient
    {
        // constants
        const string BASE_URL = "https://bandtracker-justcode.rhcloud.com/api";

        // login
        public async Task<bool> LoginAsync(string p_username, string p_password)
        {
            // configure request
            var request = new RestRequest("/auth/login", Method.POST);
            request.RequestFormat = DataFormat.Json;

            request.AddBody(new { 
                name = p_username, 
                passwd = p_password 
            });

            // execute request
            var f_response = await Execute<LoginResponse>(request);

            if (f_response.success)
                m_token = f_response.token;

            return f_response.success;
        }

    
        // helper functions
        private async Task<T> Execute<T>(RestRequest request) where T : new()
        {
            // configure client
            var client = new RestClient();
            client.BaseUrl = new System.Uri(BASE_URL);

            if (!String.IsNullOrEmpty(m_token))
                request.AddParameter("x-auth_token", m_token, ParameterType.HttpHeader);

            // execute request
            var response = await client.ExecuteTaskAsync<T>(request);

            if (response.ErrorException != null)
            {
                return default(T);
            }

            return response.Data;
        }

        private class LoginResponse
        {
            public bool   success   { get; set; }
            public string token     { get; set; }
            public string error     { get; set; }
        }

        // variables
        private string m_token = "";

        // singleton
        private static BandTrackerClient m_instance = new BandTrackerClient();
        public  static BandTrackerClient Instance { get {return m_instance; } }
    }
}
