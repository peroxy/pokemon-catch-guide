using PokeApiNet;
using System.Linq;

namespace PokeGuideGenerator
{
    internal partial class Program
    {
        public record EncounterInfo(int PokemonId, string Name, string Location, string Version, Encounter Details)
        {
            public override string ToString()
            {
                if (Details == null)
                {
                    return $"{PokemonId};{Name};{Location};{Version};;;;;;";
                }
                return $"{PokemonId};{Name};{Location};{Version};{string.Join(',', Details?.ConditionValues.Select(x => x.Name))};{Details?.Method.Name};{Details?.Chance}%;{Details?.MinLevel};{Details?.MaxLevel};";
            }

            public string ToStringWithShortVersion()
            {
                if (Details == null)
                {
                    return $"{PokemonId};{Name};{Location};{PokemonUtil.LongVersionToShort(Version)};;;;;;";
                }
                return $"{PokemonId};{Name};{Location};{PokemonUtil.LongVersionToShort(Version)};{string.Join(',', Details?.ConditionValues.Select(x => x.Name))};{Details?.Method.Name};{Details?.Chance}%;{Details?.MinLevel};{Details?.MaxLevel};";
            }
        }
    }
}
