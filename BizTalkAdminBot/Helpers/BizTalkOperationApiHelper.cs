using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using BizTalkAdminBot.Models;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net.Http.Headers;

namespace BizTalkAdminBot.Helpers
{
    /// <summary>
    /// Class to Wrap to give out the call to the Logic App which 
    /// calls the on prem Operation/management service provided by BizTalk 2016.
    /// </summary>
    public class BizTalkOperationApiHelper
    {
        private string _operation;

        private string _receiveport;

        private string _receivelocation;

        private string _sendport;

        public BizTalkOperationApiHelper(string operation)
        {
            _operation = operation;
            _receiveport = string.Empty;
            _receivelocation = string.Empty;
            _sendport = string.Empty;

        }

        /// <summary>
        /// Constructor To help the Start/stop of Send port
        /// </summary>
        /// <param name="operation">operation Name</param>
        /// <param name="sendPort">Send Port Name</param>
        /// <param name="status">Status for Send Port</param>
        public BizTalkOperationApiHelper(string operation, string sendPort)
        {
            _operation = operation;
            _receivelocation = string.Empty;
            _receiveport = string.Empty;
            _sendport = sendPort;

        }
        
        

        /// <summary>
        /// Get the List of Applications in the BizTalk environment
        /// </summary>
        /// <returns>List of Application Object</returns>
        public async Task<List<Application>> GetApplicationsAsync()
        {
            
            List<Application> applications = new List<Application>();

            string applicationsJson = await GetOnPremDataAsync();
            if (!string.IsNullOrEmpty(applicationsJson))
            {
                applications = JsonConvert.DeserializeObject<List<Application>>(applicationsJson);
            }
            
            return applications;
        }

        /// <summary>
        /// Get the Orchestrations in BizTalk environment
        /// </summary>
        public async Task<List<Orchestration>> GetOrchestrationsAsync()
        {
            List<Orchestration> orchestrations = new List<Orchestration>();

            string orchestrationJson = await GetOnPremDataAsync();

            if (!string.IsNullOrEmpty(orchestrationJson))
            {
                orchestrations = JsonConvert.DeserializeObject<List<Orchestration>>(orchestrationJson);
            }
            return orchestrations;
        }

        /// <summary>
        /// Get the Hosts in the BizTalk Environment
        /// </summary>
        /// <returns></returns>
        public async Task<List<Host>> GetHostsAsync()
        {
            List<Host> hosts = new List<Host>();

            string hostsJson = await GetOnPremDataAsync();

            if (!string.IsNullOrEmpty(hostsJson))
            {
                hosts = JsonConvert.DeserializeObject<List<Host>>(hostsJson);
            }
            return hosts;
        }

        /// <summary>
        /// Get the Send Ports in BizTalk Environment
        /// </summary>
        /// <returns></returns>
        public async Task<List<SendPort>> GetSendPortsAsync()
        {
            List<SendPort> sendPorts = new List<SendPort>();

            string sendporstJson = await GetOnPremDataAsync();

            if (!string.IsNullOrEmpty(sendporstJson))
            {
                sendPorts = JsonConvert.DeserializeObject<List<SendPort>>(sendporstJson);
            }
            return sendPorts;
        }

        public async Task<bool> ChangeSendportStateAsync()
        {
            bool status = false;
            string responseJson = await GetOnPremDataAsync();
            if(responseJson.Contains(Constants.LASuccessResponse))
            {
                status = true;    
            }
            
            return status;

        }

        /// <summary>
        /// Get the Instances in the BizTalk Environment
        /// </summary>
        /// <returns>List of Instance Object</returns>
        public async Task<List<Instance>> GetInstancesAsync()
        {
            List<Instance> instances = new List<Instance>();
            string instancesJson = await GetOnPremDataAsync();
            if(!string.IsNullOrEmpty(instancesJson))
            {
                instances = JsonConvert.DeserializeObject<List<Instance>>(instancesJson);
            }

            return instances;
        }


        /// <summary>
        /// Get the Data from On Premises BizTalk Environment using Logic App
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetOnPremDataAsync()
        {
            string response = string.Empty;
            HttpClient client = new HttpClient();

            string request = ComposeRequest();

            var content = new StringContent(request, Encoding.UTF8, "application/json");
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await client.PostAsync(Constants.LogicAppUri, content);

            if (result.IsSuccessStatusCode)
            {
                response = await result.Content.ReadAsStringAsync();
            }

            return response;
        }

        /// <summary>
        /// Compose the request sent to the Logic App
        /// </summary>
        /// <returns>String for the request</returns>
        private string ComposeRequest()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            builder.Append(string.Format(Constants.PostRequest, _operation, _receiveport, _receivelocation, _sendport));
            builder.Append("}");

            return builder.ToString();

        }
    }
}
