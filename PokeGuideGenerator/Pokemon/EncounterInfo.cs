﻿using System.Linq;
using PokeApiNet;
using PokeGuideGenerator.CSV;

namespace PokeGuideGenerator.Pokemon
{
    public record EncounterInfo(int PokemonId, string Name, string Location, string Version, Encounter Details, string EvolutionTrigger, string EvolutionMethod, bool IsBaby,
        PokemonGeneration Generation)
    {
        public override string ToString()
        {
            if (Details == null)
            {
                return $"{PokemonId};{Name};{Location};{Version};;;;;;{EvolutionTrigger};{EvolutionMethod};{IsBaby.ToHumanFormat()};{(int)Generation};";
            }

            return
                $"{PokemonId};{Name};{Location};{Version};{string.Join(',', Details?.ConditionValues.Select(x => x.Name))};{Details?.Method.Name};{Details?.Chance}%;{Details?.MinLevel};{Details?.MaxLevel};{EvolutionTrigger};{EvolutionMethod};{IsBaby.ToHumanFormat()};{(int)Generation};";
        }

        public string ToStringWithShortVersion()
        {
            if (Details == null)
            {
                return
                    $"{PokemonId};{Name};{Location};{PokemonUtil.LongVersionToShort(Version)};;;;;;{EvolutionTrigger};{EvolutionMethod};{IsBaby.ToHumanFormat()};{(int)Generation};";
            }

            return
                $"{PokemonId};{Name};{Location};{PokemonUtil.LongVersionToShort(Version)};{string.Join(',', Details?.ConditionValues.Select(x => x.Name))};{Details?.Method.Name};{Details?.Chance}%;{Details?.MinLevel};{Details?.MaxLevel};{EvolutionTrigger};{EvolutionMethod};{IsBaby.ToHumanFormat()};{(int)Generation};";
        }
    }
}