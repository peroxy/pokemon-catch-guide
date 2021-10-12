using System;
using CommandLine;
using System.Threading.Tasks;
using PokeGuideGenerator.CSV;
using PokeGuideGenerator.Pokemon;

namespace PokeGuideGenerator
{
    public class Program
    {
        public class Options
        {
            [Option('g', "generation", Required = true, HelpText = "Set Pokemon generation (1-6). Generations 7 and 8 currently lack encounter data in PokeApi (as of 2021-10-11), however, they should be supported just fine by this generator once the data is there.")]
            public PokemonGeneration Generation { get; set; }

            [Option('s', "side-games", Required = false, HelpText = "Include side games (like XD/Colloseum).")]
            public bool IncludeSideGames { get; set; }
            
            [Option('a', "all-encounters", Required = false, HelpText = "Include every single encounter from each game. By default only the best (highest chance) encounter from each game is included.")]
            public bool IncludeAllEncounters { get; set; }

            [Option('f', "from", Required = false, HelpText = "Start generating from this pokedex number (including this one).")]
            public int? FromDexNumber { get; set; }

            [Option('t', "to", Required = false, HelpText = "Generate until this pokedex number (including this one).")]
            public int? ToDexNumber { get; set; }

            [Option('o', "output", Required = false, HelpText = @"Output CSV file path, e.g. ~\dir\pokemon.csv. Default is encounters.csv in current directory.", Default = "encounters.csv")]
            public string OutputPath { get; set; }
        }

        private static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                   .WithParsedAsync(async options =>
                   {
                       var api = new PokeApi();
                       var csvWriter = new CsvWriter(options);
                       var encounters = await api.GetEncounters(options);
                       csvWriter.WriteToCsv(encounters, options.OutputPath);
                   });
        }


    }
}
