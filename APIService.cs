using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace football_project
{
    public class APIService
    {
        private const string apiKey = "e4577f884amsh708a218eb6d03a4p1542f9jsn5f6c8ce32a1f";
        private const string apiHost = "sofascore.p.rapidapi.com";

        private static readonly HttpClient client = new HttpClient(); //readonly means cant be reassigned after creation

        public APIService()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("x-rapidapi-key", apiKey);
            client.DefaultRequestHeaders.Add("x-rapidapi-host", apiHost);

            if (!client.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        public async Task<List<Team>> SearchTeamsAsync(string searchText) //testq
        {
            List<Team> result = new List<Team>();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                return result;
            }


            //error handlling - wont crash app if anything goes wronmg 
            try
            {
                string safe = Uri.EscapeDataString(searchText.Trim());  //escape data string converts special characters (spaces bewcome %20)
                string url = $"https://{apiHost}/search?q={safe}&type=all&page=0";

                HttpResponseMessage response = await client.GetAsync(url); //await pauses this method until the API responds, but lets the rest of the app keep running

                if (!response.IsSuccessStatusCode)
                {
                    return result;
                }

                string json = await response.Content.ReadAsStringAsync(); //Reads the response body as a plain string of JSON
                dynamic data = JsonConvert.DeserializeObject(json);  //dynamic- reading it loosely- not assigning it to specific class

                //if json is empty or doesnt contain a results fied, return early
                if (data == null || data.results == null)
                {
                    return result;
                }


                //Loops through every result
                //The search API returns a mix of players, teams, and tournaments
                //so we check type == "team" to only keep the team results.
                for (int i = 0; i < data.results.Count; i++)
                {
                    var item = data.results[i];

                    //Null checks before reading each field
                    if (item != null && item.type != null && item.type.ToString() == "team")
                    {

                        //Pulls out the id, name and gender from the JSON and creates a Team object with them, adding it to the result list
                        if (item.entity != null && item.entity.id != null && item.entity.name != null)
                        {
                            int id = (int)item.entity.id;
                            string name = (string)item.entity.name;
                            string gender = "";

                            if (item.entity.gender != null)
                            {
                                gender = (string)item.entity.gender;
                            }

                            result.Add(new Team(id, name, gender));
                        }
                    }
                }
            }

            //Three catch blocks —
            // network errors, JSON parsing errors, and anything else unexpected
            // All return the empty list rather than crashing
            catch (HttpRequestException)
            {
                return result;
            }
            catch (JsonException)
            {
                return result;
            }
            catch (Exception)
            {
                return result;
            }

            return result;
        }

        // Returns both the squad (List<Player>) and the Manager together
        // using a tuple so we only need one API call
        public async Task<(List<Player> Players, Manager Manager)> GetTeamSquadAsync(int teamId, string teamName)
        {
            List<Player> players = new List<Player>();
            Manager manager = null;

            try
            {
                //Builds the squad endpoint URL using the team ID found in the previous search call, then fires the request
                string url = $"https://{apiHost}/teams/get-squad?teamId={teamId}";

                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return (players, manager);
                }

                string json = await response.Content.ReadAsStringAsync();
                //deserialize into our specific SquadResponse class
                //Newtonsoft uses the [JsonProperty] attributes on SquadResponse to map the JSON keys to the right properties automatically.
                SquadResponse data = JsonConvert.DeserializeObject<SquadResponse>(json);

                if (data == null)
                {
                    return (players, manager);
                }

                // Parse manager if present
                if (data.Manager != null)
                {
                    string nationality = data.Manager.Country?.Name ?? "Unknown";
                    manager = new Manager(data.Manager.ID, data.Manager.Name, nationality, teamName);
                }

                // Parse players
                if (data.Players != null)
                {
                    for (int i = 0; i < data.Players.Count; i++)
                    {
                        SquadPlayerWrapper item = data.Players[i];

                        if (item != null && item.Player != null)
                        {
                            string position = "";

                            if (item.Player.Position != null)
                            {
                                string raw = item.Player.Position;

                                if (raw == "G") position = "GK";
                                else if (raw == "D") position = "DEF";
                                else if (raw == "M") position = "MID";
                                else if (raw == "F") position = "FWD";
                                else position = raw;
                            }

                            players.Add(new Player(
                                item.Player.ID,
                                item.Player.Name,
                                position,
                                teamName,
                                teamId
                            ));
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
                return (players, manager);
            }
            catch (JsonException)
            {
                return (players, manager);
            }
            catch (Exception)
            {
                return (players, manager);
            }

            return (players, manager);
        }
    }
}