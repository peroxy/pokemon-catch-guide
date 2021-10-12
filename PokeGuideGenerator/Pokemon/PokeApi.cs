using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PokeApiNet;
using RestSharp;
using ShellProgressBar;
using static PokeGuideGenerator.Program;

namespace PokeGuideGenerator.Pokemon
{
    public class PokeApi
    {
        private const string ApiEndpoint = "https://pokeapi.co/api/v2";
        private readonly IRestClient _restClient = new RestClient(ApiEndpoint);
        private readonly PokeApiClient _pokeApiClient = new();

        public async Task<List<EncounterInfo>[]> GetEncounters(Options options)
        {
            var requests = new List<Task<List<EncounterInfo>>>();

            int minPokedexNumber = 1;
            int maxPokedexNumber = PokemonUtil.GetMaxPokedexNumber(options.Generation);
            if (options.FromDexNumber.HasValue)
            {
                minPokedexNumber = options.FromDexNumber.Value;
            }
            if (options.ToDexNumber.HasValue)
            {
                maxPokedexNumber = options.ToDexNumber.Value;
            }
            
            int totalTicks = maxPokedexNumber - minPokedexNumber;
            var progressBarOptions = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                ForegroundColorDone = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593'
            };
            
            Console.WriteLine($"Getting all Pokemon encounters, please wait.{Environment.NewLine}");
            using var pbar = new ProgressBar(totalTicks, "Fetching Pokemon encounters...", progressBarOptions);
            var versions = PokemonUtil.GetVersions(options.Generation, options.IncludeSideGames);
            for (int i = minPokedexNumber; i <= maxPokedexNumber; i++)
            {
                requests.Add(GetEncounterInfo(i, versions, pbar, options.Generation));
            }
            return await Task.WhenAll(requests);
        }

        private async Task<List<EncounterInfo>> GetEncounterInfo(int pokemonId, string[] versions, ProgressBar progressBar, PokemonGeneration generation)
        {
            var pokemon = await _pokeApiClient.GetResourceAsync<PokeApiNet.Pokemon>(pokemonId);

            Debug.WriteLine($"GET encounters for {pokemonId}-{pokemon.Name}");

            var species = await _pokeApiClient.GetResourceAsync<PokemonSpecies>(pokemon.Species.Name);

            var encountersResponse = await _restClient.ExecuteGetAsync(new RestRequest($"/pokemon/{pokemonId}/encounters"));
            var locationAreaEncounters = JsonConvert.DeserializeObject<List<LocationAreaEncounter>>(encountersResponse.Content);

            var evolutionChainResponse = await _restClient.ExecuteGetAsync(new RestRequest($"/evolution-chain/{GetIdFromUrl(species.EvolutionChain.Url)}"));
            var evolutionChain = JsonConvert.DeserializeObject<EvolutionChain>(evolutionChainResponse.Content);
            var evolutionInfo = GetEvolutionInfoFromEvolutionChain(pokemon.Name, evolutionChain);

            var encounters = new List<EncounterInfo>();

            foreach (var locationAreaEncounter in locationAreaEncounters)
            {
                foreach (var version in locationAreaEncounter.VersionDetails)
                {
                    if (versions.Contains(version.Version.Name))
                    {
                        foreach (var details in version.EncounterDetails)
                        {
                            encounters.Add(new EncounterInfo(pokemonId, pokemon.Name, locationAreaEncounter.LocationArea.Name, version.Version.Name, details, evolutionInfo.Trigger, evolutionInfo.Method, evolutionInfo.IsBaby, generation));
                        }
                    }
                }
            }
            
            progressBar.Tick();
            
            if (encounters.Count == 0)
            {
                return new List<EncounterInfo> { new EncounterInfo(pokemonId, pokemon.Name, "no-location", "none", null, evolutionInfo.Trigger, evolutionInfo.Method, evolutionInfo.IsBaby, generation) };
            }
            return encounters;
        }


        private (string Trigger, string Method, bool IsBaby) GetEvolutionInfoFromEvolutionChain(string pokemonName, EvolutionChain evolutionChain)
        {
            string species = PokemonUtil.PokemonNameToSpeciesName(pokemonName);

            if (evolutionChain.Chain.Species.Name == species)
            {
                return (null, null, evolutionChain.Chain.IsBaby);
            }

            var chain = GetPokemonInEvolutionChain(species, evolutionChain.Chain);
            return ChainToEvolutionInfo(chain);
        }

        private static (string Trigger, string Method, bool IsBaby) ChainToEvolutionInfo(ChainLink chain)
        {
            var evoDetails = chain.EvolutionDetails.FirstOrDefault();
            string trigger = evoDetails?.Trigger?.Name;
            bool isBaby = chain.IsBaby;

            if (evoDetails == null)
            {
                return (trigger, null, isBaby);
            }

            var method = new StringBuilder();
            if (evoDetails.Gender.HasValue)
            {
                method.Append($"gender:{evoDetails.Gender}/");
            }
            if (evoDetails.HeldItem != null)
            {
                method.Append($"held_item:{evoDetails.HeldItem.Name}/");
            }
            if (evoDetails.Item != null)
            {
                method.Append($"item:{evoDetails.Item.Name}/");
            }
            if (evoDetails.KnownMove != null)
            {
                method.Append($"known_move:{evoDetails.KnownMove.Name}/");
            }
            if (evoDetails.KnownMoveType != null)
            {
                method.Append($"known_move_type:{evoDetails.KnownMoveType.Name}/");
            }
            if (evoDetails.Location != null)
            {
                method.Append($"location:{evoDetails.Location.Name}/");
            }
            if (evoDetails.MinAffection.HasValue)
            {
                method.Append($"min_affection:{evoDetails.MinAffection}/");
            }
            if (evoDetails.MinBeauty.HasValue)
            {
                method.Append($"min_beauty:{evoDetails.MinBeauty}/");
            }
            if (evoDetails.MinHappiness.HasValue)
            {
                method.Append($"min_happiness:{evoDetails.MinHappiness}/");
            }
            if (evoDetails.MinLevel.HasValue)
            {
                method.Append($"min_level:{evoDetails.MinLevel}/");
            }
            if (evoDetails.NeedsOverworldRain)
            {
                method.Append($"need_rain/");
            }
            if (evoDetails.PartySpecies != null)
            {
                method.Append($"party_species:{evoDetails.PartySpecies.Name}/");
            }
            if (evoDetails.PartyType != null)
            {
                method.Append($"party_type:{evoDetails.PartyType.Name}/");
            }
            if (evoDetails.RelativePhysicalStats.HasValue)
            {
                method.Append($"stats:{evoDetails.RelativePhysicalStats}/");
            }
            if (!string.IsNullOrWhiteSpace(evoDetails.TimeOfDay))
            {
                method.Append($"time:{evoDetails.TimeOfDay}/");
            }
            if (evoDetails.TradeSpecies != null)
            {
                method.Append($"trade_species:{evoDetails.TradeSpecies.Name}/");
            }
            if (evoDetails.TurnUpsideDown)
            {
                method.Append($"turn_upside_down/");
            }

            if (method.Length == 0)
            {
                return (trigger, null, isBaby);
            }

            method = method.Remove(method.Length - 1, 1); //remove trailing /
            return (trigger, method.ToString(), isBaby);
        }

        private ChainLink GetPokemonInEvolutionChain(string speciesName, ChainLink chain)
        {
            if (chain == null)
            {
                return null;
            }
            else if (chain.Species.Name == speciesName)
            {
                return chain;
            }
            else if (chain.EvolvesTo.Count == 0)
            {
                return null;
            }

            foreach (var evolveTo in chain.EvolvesTo)
            {
                if (evolveTo.Species.Name == speciesName)
                {
                    return evolveTo;
                }
                else
                {
                    var possibleChain = GetPokemonInEvolutionChain(speciesName, evolveTo);
                    if (possibleChain != null)
                    {
                        return possibleChain;
                    }
                }
            }

            return null;
        }

        private static int GetIdFromUrl(string url)
        {
            var textId = url.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();
            return int.Parse(textId);
        }
    }
}
