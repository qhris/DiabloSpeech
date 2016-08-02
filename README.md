# DiabloSpeech

[![Build status](https://img.shields.io/appveyor/ci/qhris/DiabloSpeech/develop.svg)](https://ci.appveyor.com/project/qhris/diablospeech/branch/develop)
[![Coverage Status](https://img.shields.io/coveralls/qhris/DiabloSpeech/develop.svg)](https://coveralls.io/github/qhris/DiabloSpeech?branch=develop)

A twitch chat bot for Diablo 2 streamers.

## Features

- Ability for users to request item information while running [DiabloInterface](https://github.com/Zutatensuppe/DiabloInterface/).
- Request leaderboard information directly from [speedrun.com](http://www.speedrun.com/d2lod).
- Add and remove custom commands.

### Commands

- `!wr [class]`: Show [speedrun.com](http://www.speedrun.com/d2lod) leaderboards for class.
- `!item [location]`: Query DiabloInterface for equipped item stats.

### Custom Commands

Ability to add/remove custom text commands.

- `!cmd add test this is some text to be added.` Adds the command `test`.
- `!cmd remove test` Removes the command `test` with its subcommands.
- `!cmd add test !two this is test number two.` Adds the subcommand `test two`.
- `!cmd remove test two` Removes the subcommand `test two`. Base command `test` is still kept.

Commands are automatically saved when edited in `commands.json`.

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
