# GOG Galaxy(R) Integration Demo

This project was created to demonstrate the basic use of GOG Galaxy SDK in a Unity Engine game. Achievements, statistics, leaderboards, friends, matchmaking and networking were all implemented using GOG Galaxy SDK.

## Getting started

Follow these instructions to get our demo up and running on your machine in no time.

### Prerequisites

* Unity Editor (2017.3.1f1 or newer) can be downloaded from [Unity](https://unity3d.com/) website.
* GOG Galaxy(R) SDK Unity Package can be downloaded from [GOG Developer Portal](https://devportal.gog.com/panel/components/sdk). Please note that you need to have access to GOG Developer Portal to download GOG Galaxy(R) SDK.

### Installing

1. Install Unity Editor (2017.3.1f1 or newer) on your machine.
2. Clone our Unity Project from git to your hard drive.
```
git clone https://github.com/gogcom/galaxy-csharp-demo-game
```
3. Download GOG Galaxy(R) SDK Unity Package from [GOG Developer Portal](https://devportal.gog.com/panel/components/sdk).
4. Open our Unity Project with Unity Editor.
5. Import GOG Galaxy(R) SDK Unity Package via the Unity Editor. 
```
Assets -> Import Package -> Custom Package
```
6. GOG Galaxy(R) SDK Unity Package will run RedistInstall.cs script that will place all the neccessary files in their required location.
7. When building a standalone player RedistCopy.cs script will place GOG dlls to the appropriate locations.

## Built with

* [Unity3D](https://unity3d.com/)
* [GOG Galaxy(R) SDK](https://devportal.gog.com/panel/components/sdk)

## Authors

* **Jakub Baranowski** - Idea, design, code, documentation.
* **Małgorzata Płachetka** - Design, code, documentation.
* **Mateusz Siłaczewski** - GOG Galaxy SDK Integration QA.
* **Grzegorz Dominiak** - Gameplay QA.

## License

Copyright (C) 2018 GOG sp. z o.o. Code licensed under MIT License - see the [LICENSE.md](LICENSE.md) file for details.
