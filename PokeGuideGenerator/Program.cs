using CommandLine;
using Newtonsoft.Json;
using PokeApiNet;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeGuideGenerator
{
    internal partial class Program
    {
        //TODO: method to evolve? when to evolve?
        //we need to lookup the pokemon species first from the existing evolution triggers
        //-> /pokemon-species/{id}/
        //-> then /evolution-chain/{id}
        //then a method that checks which properties are SET and put that into the evolveMethod string for csv

        public class Options
        {
            [Option('g', "generation", Required = true, HelpText = "Set Pokemon generation (min 1, max 8).")]
            public PokemonGeneration Generation { get; set; }

            [Option('s', "side-games", Required = false, HelpText = "Include side games (like XD/Colloseum).")]
            public bool IncludeSideGames { get; set; }
        }

        private const string apiEndpoint = "https://pokeapi.co/api/v2";
        private static async Task Main(string[] args)
        {
            var client = new RestClient(apiEndpoint);
            await Parser.Default.ParseArguments<Options>(args)
                   .WithParsedAsync(o => GenerateForGeneration(o, client));
        }

        private static async Task GenerateForGeneration(Options options, RestClient client)
        {
            var evolutionTriggers = await GetEvolutionTriggers(client);
            
            var requests = new List<Task<List<EncounterInfo>>>();
            int maxPokedexNumber = PokemonUtil.GetMaxPokedexNumber(options.Generation);
            var versions = PokemonUtil.GetVersions(options.Generation, options.IncludeSideGames);
            for (int i = 1; i <= maxPokedexNumber; i++)
            {
                requests.Add(GetEncounterInfo(client, i, PokemonUtil.PokedexNumberToName(i), versions, evolutionTriggers));
            }

            var csv = new StringBuilder($"id;name;location;version;conditions;method;chance;minLvl;maxLvl;{Environment.NewLine}");
            Console.WriteLine("Getting all encounters, please wait...");
            List<EncounterInfo>[] encounters = await Task.WhenAll(requests);
            foreach (var encounterInfos in encounters)
            {
                var bestEncounters = GetBestEncountersPerVersion(encounterInfos).ToList();
                foreach (var encounter in bestEncounters.GroupBy(x => new { x.Location, x.Name, x.PokemonId, x.Details?.Chance, MethodName = x.Details?.Method.Name }))
                {
                    if (encounter.Count() > 1)
                    {
                        var multipleVersions = string.Join('/', encounter.Select(x => PokemonUtil.LongVersionToShort(x.Version)));
                        var multipleVersionEncounter = new EncounterInfo(encounter.Key.PokemonId, encounter.Key.Name, encounter.Key.Location, multipleVersions, encounter.First().Details, encounter.First().Evolution);
                        csv.AppendLine(multipleVersionEncounter.ToString());
                    }
                    else
                    {
                        csv.AppendLine(encounter.First().ToStringWithShortVersion());
                    }
                }
            }

            Console.WriteLine("Saving encounters to 'encounters.csv'...");
            File.WriteAllText("encounters.csv", csv.ToString());

            Console.WriteLine("Finished!");
        }

        private static async Task<List<EvolutionTrigger>> GetEvolutionTriggers(RestClient client)
        {
            var response = await client.ExecuteGetAsync(new RestRequest("/evolution-trigger"));
            var triggers = JsonConvert.DeserializeObject<EvoTriggers>(response.Content);

            var requests = new List<Task<IRestResponse>>();
            foreach (var trigger in triggers.Results)
            {
                requests.Add(client.ExecuteGetAsync(new RestRequest($"/evolution-trigger/{trigger.Name}")));
            }

            var responses = await Task.WhenAll(requests);
            var evolutionTriggers = new List<EvolutionTrigger>();
            foreach (var resp in responses)
            {
                evolutionTriggers.Add(JsonConvert.DeserializeObject<EvolutionTrigger>(resp.Content));
            }
            
            return evolutionTriggers;
        }

        private static IEnumerable<EncounterInfo> GetBestEncountersPerVersion(IEnumerable<EncounterInfo> encounters)
        {
            return encounters
                .OrderByDescending(x => x.Details?.Chance)
                .ThenByDescending(x => x.Details?.MaxLevel)
                .GroupBy(x => x.Version)
                .Select(x => x.FirstOrDefault());
        }

        private static async Task<List<EncounterInfo>> GetEncounterInfo(IRestClient client, int pokemonId, string pokemonName, string[] versions, List<EvolutionTrigger> evolutionTriggers)
        {
            Debug.WriteLine($"GET encounters for {pokemonId}-{pokemonName}");
            var response = await client.ExecuteGetAsync(new RestRequest($"/pokemon/{pokemonId}/encounters"));
            var locationAreaEncounters = JsonConvert.DeserializeObject<List<LocationAreaEncounter>>(response.Content);
            var encounters = new List<EncounterInfo>();

            var evolutionTrigger = evolutionTriggers.FirstOrDefault(x => x.PokemonSpecies.Any(y => GetIdFromUrl(y.Url) == pokemonId))?.Name;

            foreach (var locationAreaEncounter in locationAreaEncounters)
            {
                foreach (var version in locationAreaEncounter.VersionDetails)
                {
                    if (versions.Contains(version.Version.Name))
                    {
                        foreach (var details in version.EncounterDetails)
                        {
                            encounters.Add(new EncounterInfo(pokemonId, pokemonName, locationAreaEncounter.LocationArea.Name, version.Version.Name, details, evolutionTrigger));
                        }
                    }
                }
            }

            if (encounters.Count == 0)
            {
                return new List<EncounterInfo> { new EncounterInfo(pokemonId, pokemonName, "no-location", "none", null, evolutionTrigger) };
            }
            return encounters;
        }

        private static int GetIdFromUrl(string url)
        {
            var textId = url.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
            return int.Parse(textId);
        }

        

    }
}
