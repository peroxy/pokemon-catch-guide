using CommandLine;
using System.Threading.Tasks;

namespace PokeGuideGenerator
{
    public class Program
    {
        public class Options
        {
            [Option('g', "generation", Required = true, HelpText = "Set Pokemon generation (min 1, max 8).")]
            public PokemonGeneration Generation { get; set; }

            [Option('s', "side-games", Required = false, HelpText = "Include side games (like XD/Colloseum).")]
            public bool IncludeSideGames { get; set; }
        }

        private static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                   .WithParsedAsync(async options =>
                   {
                       var api = new PokeApi();
                       var csvWriter = new CsvWriter();
                       var encounters = await api.GetEncountersForGeneration(options.Generation, options.IncludeSideGames);
                       csvWriter.WriteToCsv(encounters, "encounters.csv");
                   });
        }


    }
}
