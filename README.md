# Pokemon Catching Guide

![Banner](banner.jpg)

Generate a CSV file of the best Pokemon locations in specific generation. Used to help Catch Em All in every single generation.

Using .NET 5.0 and consuming data from [PokeAPI](https://pokeapi.co) by using the [PokeAPI .NET wrapper](https://github.com/mtrdp642/PokeApiNet).

## Installation

You will need to have [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0) installed to run the console application.

## Usage

Generate a CSV file for your generation by downloading the executable or building from the source.

You may use the following arguments:

```shell
#  -g, --generation    Required. Set Pokemon generation (min 1, max 8).
#  -s, --side-games    Optional. Include side games (like XD/Colloseum).
#  --help              Display this help screen.
#  --version           Display version information.
```

### Executable

```shell
cd executable-dir
.\PokeGuideGenerator.exe -g 3 -s
```

### Build from source

```shell
cd pokemon-catch-guide\PokeGuideGenerator
dotnet restore
dotnet build
dotnet run -- -g 3 -s
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## License
[Apache License 2.0](http://www.apache.org/licenses/)