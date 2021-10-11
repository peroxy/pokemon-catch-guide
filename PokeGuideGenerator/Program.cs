using CommandLine;
using System.Threading.Tasks;

namespace PokeGuideGenerator
{
    public class Program
    {
        public class Options
        {
            [Option('g', "generation", Required = true, HelpText = "Set Pokemon generation (1-8).")]
            public PokemonGeneration Generation { get; set; }

            [Option('s', "side-games", Required = false, HelpText = "Include side games (like XD/Colloseum).")]
            public bool IncludeSideGames { get; set; }

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
                       var csvWriter = new CsvWriter();
                       var encounters = await api.GetEncounters(options);
                       csvWriter.WriteToCsv(encounters, options.OutputPath);
                   });
        }


    }
}
