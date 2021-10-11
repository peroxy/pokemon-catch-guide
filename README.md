# Pokemon Catching Guide

![Banner](banner.jpg)

Generate a CSV file of the best Pokemon locations in specific generation. Used to help catch em' all in every generation.

Using .NET 5.0 and consuming data from [PokeAPI](https://pokeapi.co) by using the [PokeAPI .NET wrapper](https://github.com/mtrdp642/PokeApiNet).

## Limitations

Currently only generations 1-6 (I - R/B/Y, II - G/S/C, III - FR/LG/R/S/E/COL/XD, IV - D/P/Pt/HG/SS, V - B/W/B2/W2 and VI - X/Y/OR/AS) are supported. Generations 7 (VII - S/M/US/UM/LGP/LGE) and 8 (VIII - SW/SH) lack encounter data that is provided by [PokeAPI](https://pokeapi.co)(as of 2021-10-11). However, those encounters should be generated correctly when the data does get updated eventually.

## Installation

You will need to have [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) installed to build and run the console application from source.

If you download the executable you will not need anything else as it comes prepackaged and ready to go (Windows only at the moment).

## Usage

Generate a CSV file for your generation by downloading the executable or building from the source. It will generate a CSV file named `encounters.csv` in the current directory.

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

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[Apache License 2.0](http://www.apache.org/licenses/)