# DiabloSpeech

[![Build status](https://ci.appveyor.com/api/projects/status/dy2orsw74sjtes33/branch/develop?svg=true)](https://ci.appveyor.com/project/qhris/diablospeech/branch/develop)
[![Coverage Status](https://coveralls.io/repos/github/qhris/DiabloSpeech/badge.svg?branch=develop)](https://coveralls.io/github/qhris/DiabloSpeech?branch=develop)

A twitch chat bot for Diablo 2 streamers.

## Features

- Ability for users to request item information while running [DiabloInterface](https://github.com/Zutatensuppe/DiabloInterface/).
- Request leaderboard information directly from [speedrun.com](http://www.speedrun.com/d2lod).

### Commands

- `!wr [class]`: Show [speedrun.com](http://www.speedrun.com/d2lod) leaderboards for class.
- `!item [location]`: Query DiabloInterface for equipped item stats.
- `!breakpoints [type] [class]`: Show breakpoints for class `fcr/fhr`.

## Building

First check the project out locally. We use submodules so don't forget to initialize those.
```bash
$ git clone --recursive https://github.com/qhris/DiabloSpeech.git
```
or
```bash
$ git clone https://github.com/qhris/DiabloSpeech.git
$ cd ./DiabloSpeech
$ git submodule update --init
```

Next we can run and test the program on the different platforms.

### Windows
```powershell
$ ./build.ps1
```

### Linux / OSX / MSys
```sh
$ ./build.sh
```

Only Windows is currently officially supported, however the tests should still run.

## Contributing

We gladly accept any pull requests that are submitted.

Please make use of [editorconfig](http://editorconfig.org/) for your editor of choice.

## License

Copyright © 2016 DiabloSpeech. Provided as-is under the MIT license. See [LICENSE](./LICENSE) for more information.
