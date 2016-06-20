// This file is a part of DiabloSpeech and is under the MIT license.
// See the LICENSE file at the root of the project for more information.
using DiabloSpeech.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiabloSpeech.Business.Speedrun
{
    public class SpeedrunClient
    {
        static readonly TimeSpan CacheTimeout = TimeSpan.FromMinutes(10);
        const string BaseUrl = "http://speedrun.com/api/v1";
        const string GameId = "yd4opx1e";
        const string CategoryAny = "9kvmg8kg";
        const string CategoryAnyHC = "zd3yyrr2";
        const string CategoryHell = "824w1mk5";
        const string CategoryHellHC = "9kvxxl32";
        static readonly IReadOnlyDictionary<string, string> CategoryNameMapping = new Dictionary<string, string>()
        {
            ["9kvmg8kg"] = "Any%",
            ["zd3yyrr2"] = "Any% HC",
            ["824w1mk5"] = "Any% Hell",
            ["9kvxxl32"] = "Any% Hell HC",
        };

        IEnumerable<string> CategoryUrls =>
            from category in new[] { CategoryAny, CategoryAnyHC, CategoryHell, CategoryHellHC }
            select $"{BaseUrl}/leaderboards/{GameId}/category/{category}";

        static object cacheLock = new object();
        static Dictionary<string, List<GameRecord>> cachedRecords = null;
        static DateTime cacheTime;

        async Task<JObject> RequestJsonAsync(string url)
        {
            var content = new MemoryStream();
            var request = (HttpWebRequest)WebRequest.Create(url);

            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    await stream.CopyToAsync(content);
                }
            }

            string jsonString = Encoding.UTF8.GetString(content.ToArray());
            return JObject.Parse(jsonString);
        }

        public async Task<Dictionary<string, List<GameRecord>>> QueryLeaderboardAsync()
        {
            lock (cacheLock)
            {
                // Check cache to rate limit requests.
                if (cachedRecords != null && DateTime.Now < cacheTime)
                {
                    return cachedRecords;
                }
            }

            // Request game variables, containing the class IDs and names.
            Task<JObject> variablesTask = RequestJsonAsync($"{BaseUrl}/games/{GameId}/variables");

            // Request current leaderboards from the different categories.
            var leaderboardRequests = from categoryUrl in CategoryUrls
                                      select RequestJsonAsync(categoryUrl);

            // Wait for the data to be availbable.
            IEnumerable<JObject> leaderboards = await Task.WhenAll(leaderboardRequests);
            JObject variables = await variablesTask;

            // Get the class variable type.
            JToken classToken = (from variable in variables["data"]
                                where (string)variable["name"] == "Class"
                                select variable).FirstOrDefault();
            string classVariableId = (string)classToken?["id"] ?? "<INVALID>";

            // Get best run by class and category.
            var runByClassCategory = from leaderboard in leaderboards
                                     let category = leaderboard["data"]?["category"].ToObject<string>()
                                     from runData in leaderboard["data"]?["runs"]
                                     let run = runData["run"].ToObject<LeaderboardRun>()
                                     orderby run.Times.Realtime_t
                                     let classId = run.Values.ValueOrDefault(classVariableId)
                                     where classId != null
                                     group run by new
                                     {
                                         category,
                                         classId
                                     } into runGroup
                                     select runGroup.First();

            // Acquire runner names from best runs.
            var runnerTasks = from run in runByClassCategory
                              let runnerUrl = run.Players.FirstOrDefault().Uri
                              where !string.IsNullOrEmpty(runnerUrl)
                              group runnerUrl by runnerUrl into runnerGroup
                              select RequestJsonAsync(runnerGroup.First());
            var runnersData = await Task.WhenAll(runnerTasks);

            // Construct a runnerId->runnerName mapping.
            var runners = (from runner in runnersData
                           let id = runner["data"]?["id"]?.ToObject<string>()
                           let name = runner["data"]?["names"]?["international"]?.ToObject<string>()
                           where id != null && name != null
                           group name by id).ToDictionary(x => x.Key, x => x.First());

            // Build a list of records for each category.
            var classRecords = new Dictionary<string, List<GameRecord>>();
            foreach (var run in runByClassCategory)
            {
                string categoryName = CategoryNameMapping.ValueOrDefault(run.Category);

                // Extract and lookup the character class Id to get the class name.
                string classId = run.Values?.ValueOrDefault(classVariableId);
                string className = (string)classToken?["values"]?["values"]?[classId]?["label"] ?? "<INVALID>";
                className = className.ToLowerInvariant();

                // Finds name of the runner.
                string runnerId = run.Players?.FirstOrDefault().Id;
                string runnerName = runners.ValueOrDefault(runnerId);

                var time = TimeSpan.FromSeconds(run.Times.Realtime_t);

                // Create or get the record list for the current character class.
                List<GameRecord> records;
                if (!classRecords.TryGetValue(className, out records))
                {
                    records = new List<GameRecord>();
                    classRecords[className] = records;
                }

                // Adds the record to the class leaderboard.
                records.Add(new GameRecord()
                {
                    User = runnerName ?? "Unknown",
                    Category = categoryName ?? run.Category,
                    Time = time
                });
            }

            lock (cacheLock)
            {
                // Store in cache.
                cachedRecords = classRecords;
                cacheTime = DateTime.Now.Add(CacheTimeout);
            }

            return classRecords;
        }
    }
}
