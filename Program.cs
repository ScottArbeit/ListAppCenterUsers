//using AppCenter.AccountManagement.Client.Models;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ListAppCenterUsers
{
    class Program
    {
        private static readonly StringBuilder output = new StringBuilder();

        static async Task Main(string[] args)
        {
            // Cast parameters to lower-case, but leave parameter values as they are.
            List<string> arguments = new List<string>(args.Select(arg => arg.StartsWith("--") ? arg.ToLowerInvariant() : arg));

            AddOutput("ListAppCenterUsers - Microsoft Visual Studio App Center");
            AddOutput(string.Empty);

            if (arguments.Count == 0 || arguments.Contains(Parameters.Help1) || arguments.Contains(Parameters.Help2))
            {
                ShowHelp();
                return;
            }

            // Get the provided parameter values.
            string apiToken = GetParameterValue(arguments, Parameters.ApiToken);
            string organization = GetParameterValue(arguments, Parameters.Organization);
            string outputFile = GetParameterValue(arguments, Parameters.OutputFile);

            // Check that we have a valid set of parameters.
            bool isError =
                IsError(string.IsNullOrEmpty(apiToken),
                    $"Error: {Parameters.ApiToken} is required to connect to your App Center account.") ||

                IsError(string.IsNullOrEmpty(organization),
                    $"Error: {Parameters.Organization} is required to connect to your App Center account.");

            if (!isError)
            {
                using HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("https://api.appcenter.ms/v0.1/") };
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("X-API-Token", apiToken);

                // First, get the organization-level users.
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"orgs/{Uri.EscapeDataString(organization)}/users");
                HttpResponseMessage response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    WriteHeader("Organization users:");

                    List<OrganizationUserResponse> users = await JsonSerializer.DeserializeAsync<List<OrganizationUserResponse>>(await response.Content.ReadAsStreamAsync());
                    users.Sort(new UserResponseComparer());

                    foreach (var user in users)
                    {
                        AddOutput($"{user.Role, -13}{user.Email, -40}{user.Name, -40}{user.DisplayName}");
                    }

                    AddOutput(string.Empty);
                }

                // Next, get the list of teams, and list the users in those teams
                request = new HttpRequestMessage(HttpMethod.Get, $"orgs/{Uri.EscapeDataString(organization)}/teams");
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    List<TeamResponse> teams = await JsonSerializer.DeserializeAsync<List<TeamResponse>>(await response.Content.ReadAsStreamAsync());

                    foreach (var team in teams)
                    {
                        WriteHeader($"Team {team.Name} users:");

                        request = new HttpRequestMessage(HttpMethod.Get, $"orgs/{Uri.EscapeDataString(organization)}/teams/{team.Name}/users");
                        response = await httpClient.SendAsync(request);

                        List<TeamUserResponse> teamUsers = await JsonSerializer.DeserializeAsync<List<TeamUserResponse>>(await response.Content.ReadAsStreamAsync());
                        teamUsers.Sort(new UserResponseComparer());
                        foreach (var user in teamUsers)
                        {
                            AddOutput($"{user.Role,-13}{user.Email,-40}{user.Name,-40}{user.DisplayName}");
                        }

                        AddOutput(string.Empty);
                    }
                }

                // Next, get the shared distribution groups
                request = new HttpRequestMessage(HttpMethod.Get, $"orgs/{Uri.EscapeDataString(organization)}/distribution_groups");
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    List<DistributionGroupResponse> distributionGroups = await JsonSerializer.DeserializeAsync<List<DistributionGroupResponse>>(await response.Content.ReadAsStreamAsync());

                    foreach (var distributionGroup in distributionGroups)
                    {
                        WriteHeader($"Shared distribution group {distributionGroup.Name} members:");

                        request = new HttpRequestMessage(HttpMethod.Get, $"orgs/{Uri.EscapeDataString(organization)}/distribution_groups/{distributionGroup.Name}/members");
                        response = await httpClient.SendAsync(request);

                        List<DistributionGroupMembersResponse> distributionGroupMemebers = await JsonSerializer.DeserializeAsync<List<DistributionGroupMembersResponse>>(await response.Content.ReadAsStreamAsync());
                        foreach (var member in distributionGroupMemebers)
                        {
                            AddOutput($"{"tester",-13}{member.Email,-40}{member.Name,-40}{member.DisplayName}");
                        }

                        AddOutput(string.Empty);
                    }
                }

                // Next, get the list of apps, and list the collaborators and the members of each distribution group
                request = new HttpRequestMessage(HttpMethod.Get, $"orgs/{Uri.EscapeDataString(organization)}/apps");
                response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var apps = await JsonSerializer.DeserializeAsync<AppResponse[]>(await response.Content.ReadAsStreamAsync());

                    foreach (var app in apps)
                    {
                        WriteHeader($"App {app.DisplayName} collaborators:");

                        request = new HttpRequestMessage(HttpMethod.Get, $"apps/{app.Owner.Name}/{app.Name}/testers");
                        response = await httpClient.SendAsync(request);

                        List<TeamUserResponse> teamUsers = await JsonSerializer.DeserializeAsync<List<TeamUserResponse>>(await response.Content.ReadAsStreamAsync());
                        teamUsers.Sort(new UserResponseComparer());
                        foreach (var user in teamUsers)
                        {
                            AddOutput($"{(user.Role ?? "tester"),-13}{user.Email,-40}{user.Name,-40}{user.DisplayName}");
                        }

                        AddOutput(string.Empty);

                        request = new HttpRequestMessage(HttpMethod.Get, $"apps/{app.Owner.Name}/{app.Name}/distribution_groups");
                        response = await httpClient.SendAsync(request);

                        List<DistributionGroupResponse> distributionGroups = await JsonSerializer.DeserializeAsync<List<DistributionGroupResponse>>(await response.Content.ReadAsStreamAsync());
                        foreach (var distributionGroup in distributionGroups.Where(dg => dg.Name != "Collaborators"))
                        {
                            request = new HttpRequestMessage(HttpMethod.Get, $"apps/{app.Owner.Name}/{app.Name}/distribution_groups/{distributionGroup.Name}/members");
                            response = await httpClient.SendAsync(request);

                            WriteHeader($"App {app.DisplayName} - Distribution group {distributionGroup.DisplayName} members:");

                            List<DistributionGroupMembersResponse> distributionGroupMembers = await JsonSerializer.DeserializeAsync<List<DistributionGroupMembersResponse>>(await response.Content.ReadAsStreamAsync());
                            foreach (var distributionGroupMember in distributionGroupMembers)
                            {
                                AddOutput($"{"tester",-13}{distributionGroupMember.Email,-40}{distributionGroupMember.Name,-40}{distributionGroupMember.DisplayName}");
                            }

                            AddOutput(string.Empty);
                        }
                    }
                }
            }

            AddOutput($"{Environment.NewLine}Done.");

            if (!string.IsNullOrEmpty(outputFile))
            {
                await File.WriteAllTextAsync(outputFile, output.ToString());
            }
        }

        private static void WriteHeader(string header)
        {
            AddOutput(header);
            AddOutput(string.Empty);
            AddOutput($"{"Role",-13}{"Email",-40}{"Name",-40}{"Display Name"}");
            AddOutput($"{new String('-', 12),-13}{new String('-', 39),-40}{new String('-', 39),-40}{new String('-', 25)}");
        }

        private static string GetParameterValue(List<string> arguments, string parameter, string defaultValue = "")
        {
            string returnValue = defaultValue;

            int parameterIndex = arguments.IndexOf(parameter);
            if ((parameterIndex >= 0) && (parameterIndex < arguments.Count - 1))
            {
                string parameterValue = arguments[parameterIndex + 1];
                if (!parameterValue.StartsWith("--"))
                {
                    returnValue = parameterValue;
                }
            }

            return returnValue;
        }

        private static bool IsError(bool errorCondition, string errorMessage)
        {
            if (errorCondition)
            {
                AddOutput(errorMessage);
            }

            return errorCondition;
        }

        private static void AddOutput(string outputValue)
        {
            Console.WriteLine(outputValue);
            output.AppendLine(outputValue);
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Examples:");
            Console.WriteLine("---------");
            Console.WriteLine("List all users associated with an App Center organization:");
            Console.WriteLine($"ListAppCenterUsers {Parameters.ApiToken} myApiToken {Parameters.Organization} My-Organization-Name");
            Console.WriteLine();
            Console.WriteLine("Show help:");
            Console.WriteLine($"ListAppCenterUsers {Parameters.Help1} or ListAppCenterUsers {Parameters.Help2}");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine("-----------");
            Console.WriteLine($"  {Parameters.ApiToken}: (required) The API token for your organization");
            Console.WriteLine($"  {Parameters.Organization}: (required) The name of your organization (i.e. the slug used in AppCenter URL's: https://appcenter.ms/orgs/{{**this part**}})");
            Console.WriteLine($"  {Parameters.OutputFile}: Optional filename to write output to; default is to write output to the console");
            Console.WriteLine($"  {Parameters.Help1} | {Parameters.Help2}: Show this help message");
            Console.WriteLine();
        }
    }

    public class UserResponseComparer : IComparer<UserResponse>
    {
        public int Compare(UserResponse user1, UserResponse user2)
        {
            if (user1?.Role == null && user2?.Role == null)
            {
                return user1.Email.CompareTo(user2.Email); ;
            }
            else if (user1.Role.CompareTo(user2.Role) != 0)
            {
                return user1.Role.CompareTo(user2.Role);
            }
            else
            {
                return user1.Email.CompareTo(user2.Email);
            }
        }
    }
}
