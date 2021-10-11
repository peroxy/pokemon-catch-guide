using PokeApiNet;
using System.Linq;

namespace PokeGuideGenerator
{
    public record EncounterInfo(int PokemonId, string Name, string Location, string Version, Encounter Details, string Evolution)
    {
        public override string ToString()
        {
            if (Details == null)
            {
                return $"{PokemonId};{Name};{Location};{Version};;;;;;{Evolution};";
            }
            return $"{PokemonId};{Name};{Location};{Version};{string.Join(',', Details?.ConditionValues.Select(x => x.Name))};{Details?.Method.Name};{Details?.Chance}%;{Details?.MinLevel};{Details?.MaxLevel};{Evolution};";
        }

        public string ToStringWithShortVersion()
        {
            if (Details == null)
            {
                return $"{PokemonId};{Name};{Location};{PokemonUtil.LongVersionToShort(Version)};;;;;;{Evolution};";
            }
            return $"{PokemonId};{Name};{Location};{PokemonUtil.LongVersionToShort(Version)};{string.Join(',', Details?.ConditionValues.Select(x => x.Name))};{Details?.Method.Name};{Details?.Chance}%;{Details?.MinLevel};{Details?.MaxLevel};{Evolution};";
        }

    }
}
