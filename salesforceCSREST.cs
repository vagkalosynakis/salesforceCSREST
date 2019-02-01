using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Salesforce.Common;
using Salesforce.Force;

namespace SalesforceAPI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            doPost();
        }

        string authToken = "";
        string loginUrl = "https://login.salesforce.com";
        string clientId = "[client_id from Salesforce]";
        string clientSecret = "[client_secret from Salesforce]";
        string username = "[username]";
        string password = "[password]";
        string securityToken = "[securityToken]";

		//Get access token from Salesforce OAuth
        public async void doPost()
        {
            var dictionaryForUrl = new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", clientId},
                {"client_secret", clientSecret},
                {"username", username},
                {"password", password + securityToken}
            };

            HttpClient http = new HttpClient();
            HttpContent content = new FormUrlEncodedContent(dictionaryForUrl);
            HttpResponseMessage res = await http.PostAsync("https://login.salesforce.com/services/oauth2/token", content);
            string message = await res.Content.ReadAsStringAsync();

            JObject jo = JObject.Parse(message);
            authToken = (string)jo["access_token"];
        }

		//GET using above token to get all objects from Salesforce
        private async void get_Click(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://[instance].salesforce.com/services/apexrest/objects/";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + authToken);
                HttpResponseMessage responseMessage = await client.SendAsync(request);

                string response = await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
		
		//GET using above token to get one(1) object from Salesforce
        private async void get1_Click(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://[instance].salesforce.com/services/apexrest/objects/[field value]";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Add("authorization", "Bearer " + authToken);
                HttpResponseMessage responseMessage = await client.SendAsync(request);

                string response = await responseMessage.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
		
		//POST using above token to create a new object in Salesforce
        private async void post_Click(object sender, EventArgs e)
        {
            try
            {
                var auth = new AuthenticationClient();
                await auth.UsernamePasswordAsync(clientId, clientSecret, username, password + securityToken);
                var client = new ForceClient(auth.InstanceUrl, auth.AccessToken, auth.ApiVersion);
                var object = new object() {
					[field] = [value]
					};
                var id = await client.CreateAsync([Object], object);
                textBox1.Text = id.ToString();
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

		//Object to pass as POST body
        public class object
        {
            public string Name { get; set; }
        }
    }
}
