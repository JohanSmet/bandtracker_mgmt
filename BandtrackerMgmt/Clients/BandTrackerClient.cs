using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RestSharp;

namespace BandtrackerMgmt
{
    class BandTrackerClient
    {
        // constants
        const string BASE_URL = "https://bandtracker-justcode.rhcloud.com/api";

        // login
        public bool LoginTokenIsValid()
        {
            return !String.IsNullOrEmpty(m_token);
        }

        public bool LoginTokenRestore()
        {
            if (Properties.Settings.Default.BandTrackerTokenTime < DateTime.Now.AddDays(-1))
                return false;

            m_token = Properties.Settings.Default.BandTrackerToken;
            return LoginTokenIsValid();
        }

        public void LoginTokenSave()
        {
            Properties.Settings.Default.BandTrackerToken     = m_token;
            Properties.Settings.Default.BandTrackerTokenTime = DateTime.Now;
            Properties.Settings.Default.Save();
        }

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
            {
                m_token = f_response.token;
                LoginTokenSave();
            }

            return f_response.success;
        }

        // bands
        public async Task<List<Band>> BandList(int p_count, int p_skip, string p_name_pattern, bool p_no_bio, bool p_no_discogs, CancellationToken cancelToken)
        {
            // configure request
            var request = new RestRequest("/bands/list", Method.GET);
            request.AddParameter("count", p_count);
            request.AddParameter("skip",  p_skip);

            if (!String.IsNullOrEmpty(p_name_pattern))
                request.AddParameter("name", p_name_pattern);

            if (p_no_bio)
                request.AddParameter("nobio", 1);

            if (p_no_discogs)
                request.AddParameter("nodiscogs", 1);

            // execute request
            return await Execute<List<Band>>(request);
        }

        public async Task<List<Band>> BandList(int p_count, int p_skip, string p_name_pattern, bool p_no_bio, bool p_no_discogs)
        {
            return await BandList(p_count, p_skip, p_name_pattern, p_no_bio, p_no_discogs, CancellationToken.None);
        }

        public async Task<int> BandUpdateStatus(List<string> p_mbids, Band.Status p_status)
        {
            // configure request
            var request = new RestRequest("/bands/update-status", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new 
            { 
                status = p_status,
                bands = p_mbids 
            });

            // execute request
            var f_response = await Execute<TaskResponse>(request);

            return f_response.done;
        }

        // tasks
        public async Task<List<ServerTask>> TaskList(string p_status, CancellationToken cancelToken)
        {
            // configure request
            var request = new RestRequest("/task", Method.GET);
            request.AddParameter("status", p_status);

            // execute request
            return await Execute<List<ServerTask>>(request);
        }

        public async Task<List<ServerTask>> TaskList(string p_status)
        {
            return await TaskList(p_status, CancellationToken.None);
        }

        public async Task<int> TaskCreateMusicBrainzUrl(List<string> p_mbids)
        {
            // configure request
            var request = new RestRequest("/task/mb-urls", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(new { bands = p_mbids });

            // execute request
            var f_response = await Execute<TaskResponse>(request);

            return f_response.done;
        }

        // helper functions
        private async Task<T> Execute<T>(RestRequest request, CancellationToken cancelToken) where T : new()
        {
            // configure client
            var client = new RestClient();
            client.BaseUrl = new System.Uri(BASE_URL);

            if (!String.IsNullOrEmpty(m_token))
                request.AddParameter("x-access-token", m_token, ParameterType.HttpHeader);

            // execute request
            var response = await client.ExecuteTaskAsync<T>(request, cancelToken);

            if (response.ErrorException != null)
            {
                return default(T);
            }

            return response.Data;
        }

        private async Task<T> Execute<T>(RestRequest request) where T : new()
        {
            return await Execute<T>(request, CancellationToken.None);
        }

        // nested classes
        private class LoginResponse
        {
            public bool   success   { get; set; }
            public string token     { get; set; }
            public string error     { get; set; }
        }

        private class TaskResponse
        {
            public int    done      { get; set; }
        }

        // variables
        private string m_token = "";

        // singleton
        private static BandTrackerClient m_instance = new BandTrackerClient();
        public  static BandTrackerClient Instance { get {return m_instance; } }
    }
}
