using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PokeGuideGenerator
{
    public class CsvWriter
    {

        public void WriteToCsv(List<EncounterInfo>[] encounters, string path)
        {
            var csv = new StringBuilder($"id;name;location;version;conditions;method;chance;minLvl;maxLvl;trigger;evolution_method;baby;{Environment.NewLine}");
            foreach (var encounterInfos in encounters)
            {
                var bestEncounters = GetBestEncountersPerVersion(encounterInfos).ToList();
                foreach (var encounter in bestEncounters.GroupBy(x => new { x.Location, x.Name, x.PokemonId, x.Details?.Chance, MethodName = x.Details?.Method.Name }))
                {
                    if (encounter.Count() > 1)
                    {
                        var multipleVersions = string.Join('/', encounter.Select(x => PokemonUtil.LongVersionToShort(x.Version)));
                        var firstEncounter = encounter.First();
                        var multipleVersionEncounter = new EncounterInfo(encounter.Key.PokemonId, encounter.Key.Name, encounter.Key.Location, multipleVersions, firstEncounter.Details, firstEncounter.EvolutionTrigger, firstEncounter.EvolutionMethod, firstEncounter.IsBaby);
                        csv.AppendLine(multipleVersionEncounter.ToString());
                    }
                    else
                    {
                        csv.AppendLine(encounter.First().ToStringWithShortVersion());
                    }
                }
            }

            Console.WriteLine($"Saving encounters to '{path}'...");
            File.WriteAllText(path, csv.ToString());
        }

        private static IEnumerable<EncounterInfo> GetBestEncountersPerVersion(IEnumerable<EncounterInfo> encounters)
        {
            return encounters
                .OrderByDescending(x => x.Details?.Chance)
                .ThenByDescending(x => x.Details?.MaxLevel)
                .GroupBy(x => x.Version)
                .Select(x => x.FirstOrDefault());
        }
    }
}
