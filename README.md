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
- `VI` - X, Y, Omega Ruby, Alpha Sapphire.

Generations 7 (`VII` - Sun, Moon, Ultra Sun, Ultra Moon, Let's Go Pikachu, Let's Go Eevee) and 8 (`VIII` - Sword, Shield) lack encounter data that is provided by [PokeAPI](https://pokeapi.co)(as of 2021-10-11). However, those encounters should be generated correctly when the data does get updated eventually.

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
.\PokeGuideGenerator.exe --generation 3 --side-games --from 1 --to 151 --output ~\pokemon.csv
```

### Build from source

```shell
cd pokemon-catch-guide\PokeGuideGenerator
dotnet restore
dotnet build
dotnet run -- --generation 3 --side-games --from 1 --to 151 --output ~\pokemon.csv
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

Example of the first 20 Pokemon from generation `III` CSV:
```csv
id;name;location;version;conditions;method;chance;minLvl;maxLvl;trigger;evolution_method;baby;
1;bulbasaur;pallet-town-area;FR/LG;;gift;100%;5;5;;;no;
2;ivysaur;no-location;;;;;;;level-up;min_level:16;no;
3;venusaur;no-location;;;;;;;level-up;min_level:32;no;
4;charmander;pallet-town-area;FR/LG;;gift;100%;5;5;;;no;
5;charmeleon;no-location;;;;;;;level-up;min_level:16;no;
6;charizard;no-location;;;;;;;level-up;min_level:36;no;
7;squirtle;pallet-town-area;FR/LG;;gift;100%;5;5;;;no;
8;wartortle;no-location;;;;;;;level-up;min_level:16;no;
9;blastoise;no-location;;;;;;;level-up;min_level:36;no;
10;caterpie;kanto-route-25-area;FR/LG;;walk;20%;8;8;;;no;
11;metapod;pattern-bush-area;FR/LG;;walk;5%;9;9;level-up;min_level:7;no;
12;butterfree;no-location;;;;;;;level-up;min_level:10;no;
13;weedle;kanto-route-25-area;FR/LG;;walk;20%;8;8;;;no;
14;kakuna;pattern-bush-area;FR/LG;;walk;20%;9;9;level-up;min_level:7;no;
15;beedrill;no-location;;;;;;;level-up;min_level:10;no;
16;pidgey;five-isle-meadow-area;FR/LG;;walk;20%;44;44;;;no;
17;pidgeotto;berry-forest-area;FR/LG;;walk;20%;37;37;level-up;min_level:18;no;
18;pidgeot;no-location;;;;;;;level-up;min_level:36;no;
19;rattata;kanto-route-9-area;FR/LG;;walk;20%;16;16;;;no;
20;raticate;pokemon-mansion-b1f;FR/LG;;walk;20%;34;34;level-up;min_level:20;no;
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[Apache License 2.0](http://www.apache.org/licenses/)
