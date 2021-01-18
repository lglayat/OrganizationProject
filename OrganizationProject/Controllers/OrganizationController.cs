using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OrganizationProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrganizationController : ControllerBase
    {
        static HttpClient client = new HttpClient();

        [HttpGet]
        public async Task<List<Organization>> RunAsync()
        {
            //set up client
            client.BaseAddress = new Uri("https://5f0ddbee704cdf0016eaea16.mockapi.io/organizations");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Get the List of Organizations
            List<Organization> orgs = await GetOrgsAsync(client.BaseAddress);

            //Fill the organizations with their respective users
            List<Organization> orgsWithUsers = await PopulateUsers(client.BaseAddress, orgs);

            //populate phone count,blacklist count etc for each user
            List<Organization> finalOrgs = await PopulatePhones(client.BaseAddress, orgsWithUsers);

            return finalOrgs;
        }

        public async Task<List<Organization>> GetOrgsAsync(System.Uri path)
        {
            List<Organization> orgs = null;

            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                orgs = await response.Content.ReadAsAsync<List<Organization>>();
            }
            return orgs;
        }

        public async Task<List<Organization>> PopulateUsers(System.Uri path, List<Organization> orgs)
        {
            for (int i = 0; i < orgs.Count; i++)
            {
                List<User> users = await GetUsers(path + "/" + orgs[i].Id + "/users/");
                orgs[i].Users = users;
            }
            return orgs;
        }

        public async Task<List<Organization>> PopulatePhones(System.Uri path, List<Organization> orgs)
        {
            for (int i = 0; i < orgs.Count; i++)
            {
                int blackListTotal = 0, totalCount = 0;

                for (int j = 0; j < orgs[i].Users.Count; j++)
                {
                    List<Phone> phones = await GetPhones(path + "/" + orgs[i].Id + "/users/" + orgs[i].Users[j].Id + "/phones");

                    //Set phone metadata
                    orgs[i].Users[j].PhoneCount = phones.Count;
                    totalCount += phones.Count;

                    //increment blackListTotal and totalCount
                    for (int k = 0; k < phones.Count; k++)
                    {
                        orgs[i].Users[j].PhoneCount++;
                        if (phones[k].Blacklist == true) blackListTotal++;
                    }
                }
                orgs[i].BlackListTotal = blackListTotal;
                orgs[i].TotalCount = totalCount;
            }
            return orgs;
        }

        public async Task<List<User>> GetUsers(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);

            //This block handles me getting throttled by the API
            if ((int)response.StatusCode == 429)
            {
                int attempt = 1, limit = 10;
                Random r = new Random();

                while ((int)response.StatusCode != 200)
                {
                    //If limit hit, throw exception
                    if (attempt > limit) throw new ArgumentException("Server is possibly down. Refused last 10 attempts");

                    //calculate and execute sleep with exponential backoff w/ jitter.
                    int sleep = r.Next(0, (int)Math.Min(1000 * Math.Pow(2, attempt), 32000));
                    System.Threading.Thread.Sleep(sleep);
                    attempt++;

                    //Try again
                    response = await client.GetAsync(path);
                    if ((int)response.StatusCode == 200)
                    {
                        return await response.Content.ReadAsAsync<List<User>>();
                    }
                }
                return null;
            }
            else
            {
                return await response.Content.ReadAsAsync<List<User>>();
            }
        }

        public async Task<List<Phone>> GetPhones(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);

            //This block handles me getting throttled by the API
            if ((int)response.StatusCode == 429)
            {
                int attempt = 1, limit = 10;
                Random r = new Random();

                while ((int)response.StatusCode != 200)
                {
                    //If limit hit, throw exception
                    if (attempt > limit) throw new ArgumentException("Server is possibly down. Refused last 10 attempts");

                    //calculate and execute sleep with exponential backoff w/ jitter.
                    int sleep = r.Next(0, (int)Math.Min(1000 * Math.Pow(2, attempt), 32000));
                    System.Threading.Thread.Sleep(sleep);
                    attempt++;

                    //Try again
                    response = await client.GetAsync(path);

                    if ((int)response.StatusCode == 200)
                    {
                        return await response.Content.ReadAsAsync<List<Phone>>();
                    }
                }
                return null;
            }
            else
            {
                return await response.Content.ReadAsAsync<List<Phone>>();
            }
        }

    }
}
