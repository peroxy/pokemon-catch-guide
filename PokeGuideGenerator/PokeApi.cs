using Newtonsoft.Json;
using PokeApiNet;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokeGuideGenerator
{
    //TODO: method to evolve? when to evolve?
    //we need to lookup the pokemon species first from the existing evolution triggers
    //-> /pokemon-species/{id}/
    //-> then /evolution-chain/{id}
    //then a method that checks which properties are SET and put that into the evolveMethod string for csv
    public class PokeApi
    {
        private const string apiEndpoint = "https://pokeapi.co/api/v2";
        private readonly IRestClient _restClient = new RestClient(apiEndpoint);
        private readonly PokeApiClient _pokeApiClient = new PokeApiClient();

        public async Task<List<EncounterInfo>[]> GetEncountersForGeneration(PokemonGeneration generation, bool includeSideGames)
        {
            var evolutionTriggers = await GetEvolutionTriggers();

            var requests = new List<Task<List<EncounterInfo>>>();
            int maxPokedexNumber = PokemonUtil.GetMaxPokedexNumber(generation);
            var versions = PokemonUtil.GetVersions(generation, includeSideGames);
            for (int i = 1; i <= maxPokedexNumber; i++)
            {
                requests.Add(GetEncounterInfo(i, PokemonUtil.PokedexNumberToName(i), versions, evolutionTriggers));
            }

            Console.WriteLine("Getting all encounters, please wait...");
            return await Task.WhenAll(requests);
        }

        private async Task<List<EvolutionTrigger>> GetEvolutionTriggers()
        {
            var response = await _restClient.ExecuteGetAsync(new RestRequest("/evolution-trigger"));
            var triggers = JsonConvert.DeserializeObject<EvoTriggers>(response.Content);

            var requests = new List<Task<IRestResponse>>();
            foreach (var trigger in triggers.Results)
            {
                requests.Add(_restClient.ExecuteGetAsync(new RestRequest($"/evolution-trigger/{trigger.Name}")));
            }

            var responses = await Task.WhenAll(requests);
            var evolutionTriggers = new List<EvolutionTrigger>();
            foreach (var resp in responses)
            {
                evolutionTriggers.Add(JsonConvert.DeserializeObject<EvolutionTrigger>(resp.Content));
            }

            return evolutionTriggers;
        }

        private async Task<List<EncounterInfo>> GetEncounterInfo(int pokemonId, string pokemonName, string[] versions, List<EvolutionTrigger> evolutionTriggers)
        {
            Debug.WriteLine($"GET encounters for {pokemonId}-{pokemonName}");

            var pokemon = await _pokeApiClient.GetResourceAsync<Pokemon>(pokemonId);
            var species = await _pokeApiClient.GetResourceAsync<PokemonSpecies>(pokemon.Species.Name);

            var encountersResponse = await _restClient.ExecuteGetAsync(new RestRequest($"/pokemon/{pokemonId}/encounters"));
            var locationAreaEncounters = JsonConvert.DeserializeObject<List<LocationAreaEncounter>>(encountersResponse.Content);

            var evolutionChainResponse = await _restClient.ExecuteGetAsync(new RestRequest($"/evolution-chain/{GetIdFromUrl(species.EvolutionChain.Url)}"));
            var evolutionChain = JsonConvert.DeserializeObject<EvolutionChain>(evolutionChainResponse.Content);

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
