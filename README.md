# Pokemon Catching Guide

![Banner](banner.jpg)

Generate a CSV file of the best Pokemon locations in specific generation. Used to help catch em' all in every generation.

Using .NET 5.0 and consuming data from [PokeAPI](https://pokeapi.co) by using the [PokeAPI .NET wrapper](https://github.com/mtrdp642/PokeApiNet).

## Limitations

Currently games from generations 1 to 6 are supported:
- `I` - Red, Blue, Yellow
- `II` - Gold, Silver, Crystal
- `III` - FireRed, LeafGreen, Ruby, Sapphire, Emerald, Colloseum, XD Gale Of Darkness
- `IV` - Diamond, Pearl, Platinum, HeartGold, SoulSilver
- `V` - Black, White, Black 2, White 2,
- `VI` - X, Y (Omega Ruby and Alpha Sapphire are missing a lot of data).

Generations 7 (`VII` - Sun, Moon, Ultra Sun, Ultra Moon, Let's Go Pikachu, Let's Go Eevee) and 8 (`VIII` - Sword, Shield) lack encounter data that is provided by [PokeAPI](https://pokeapi.co) (as of 2021-10-11). However, those encounters should be generated correctly when the data does get updated eventually.

## Installation

You will need to have [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) installed to build and run the console application from source.

If you download the executable you will not need anything else as it comes prepackaged and ready to go (Windows only at the moment).

## Usage

Generate a CSV file for your generation by downloading the executable or building from the source. It will generate a CSV file with the best encounters for each game in the current directory.

You may use the following arguments:

```shell
.\PokeGuideGenerator.exe --help

# -g, --generation    Required. Set Pokemon generation (1-6). Generations 7 and 8 currently lack encounter data in
#                     PokeApi (as of 2021-10-11), however, they should be supported just fine by this generator once the
#                     data is there.
# 
# -s, --side-games    Optional. Include side games (like XD/Colloseum).
#
# -a, --all-encounters    Include every single encounter from each game. By default only the best (highest chance) encounter from
#                          each game is included.
#
# 
# -f, --from          Optional. Start generating from this pokedex number (including this one).
# 
# -t, --to            Optional. Generate until this pokedex number (including this one).
# 
# -o, --output        Optional (Default: encounters.csv). Output CSV file path, e.g. ~\dir\pokemon.csv. Default is encounters.csv in current directory.
# 
# --help              Display this help screen.
# 
# --version           Display version information.
```

### Executable

```shell
cd executable-dir
.\PokeGuideGenerator.exe --generation 3 --side-games --from 1 --to 151 --output ~\pokemon.csv --all-encounters
```

### Build from source

```shell
cd pokemon-catch-guide\PokeGuideGenerator
dotnet restore
dotnet build
dotnet run -- --generation 3 --side-games --from 1 --to 151 --output ~\pokemon.csv --all-encounters
```

## CSV

The generated CSV file contains headers:
  - `id` (Pokemon National Pokedex number),
  - `name` (Pokemon name),
  - `location` (encounter location in the game),
  - `version` (game version),
  - `conditions` (condition to trigger the encounter),
  - `method` (encounter method),
  - `chance` (chance of encounter in percentage),
  - `minLvl` (minimum possible level of the Pokemon for this encounter),
  - `maxLvl` (maximum possible level of the Pokemon for this encounter),
  - `trigger` (type of evolution),
  - `evolution_method` (how to evolve the Pokemon),
  - `baby` (is this Pokemon a baby).

Example of 20 Pokemon from generation `III` CSV:
```csv
id;name;location;version;conditions;method;chance;minLvl;maxLvl;trigger;evolution_method;baby;
232;donphan;no-location;;;;;;;level-up;min_level:25;no;
233;porygon2;no-location;;;;;;;trade;held_item:up-grade;no;
234;stantler;hoenn-altering-cave-h;E;;walk;20%;24;24;;;no;
234;stantler;kanto-altering-cave-h;FR/LG;;walk;20%;24;24;;;no;
235;smeargle;artisan-cave-area;E;;walk;20%;41;41;;;no;
235;smeargle;kanto-altering-cave-i;FR/LG;;walk;20%;24;24;;;no;
236;tyrogue;no-location;;;;;;;;;yes;
237;hitmontop;no-location;;;;;;;level-up;min_level:20/stats:0;no;
238;smoochum;no-location;;;;;;;;;yes;
239;elekid;no-location;;;;;;;;;yes;
240;magby;no-location;;;;;;;;;yes;
241;miltank;hoenn-safari-zone-expansion-north;E;;walk;4%;37;37;;;no;
242;blissey;no-location;;;;;;;level-up;min_happiness:220;no;
243;raikou;roaming-kanto-area;FR/LG;story-progress-beat-elite-four-round-two,starter-squirtle;only-one;100%;50;50;;;no;
244;entei;roaming-kanto-area;FR/LG;story-progress-beat-elite-four-round-two,starter-bulbasaur;only-one;100%;50;50;;;no;
245;suicune;roaming-kanto-area;FR/LG;story-progress-beat-elite-four-round-two,starter-charmander;only-one;100%;50;50;;;no;
246;larvitar;sevault-canyon-area;FR/LG;;walk;4%;15;15;;;no;
247;pupitar;no-location;;;;;;;level-up;min_level:30;no;
248;tyranitar;no-location;;;;;;;level-up;min_level:55;no;
249;lugia;navel-rock-area;E/FR/LG;;only-one;100%;70;70;;;no;
250;ho-oh;navel-rock-area;E/FR/LG;;only-one;100%;70;70;;;no;
251;celebi;no-location;;;;;;;;;no;
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[Apache License 2.0](http://www.apache.org/licenses/)
