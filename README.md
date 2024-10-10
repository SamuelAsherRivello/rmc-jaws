[![npm package](https://img.shields.io/npm/v/com.rmc.rmc-jaws)](https://www.npmjs.com/package/com.rmc.rmc-jaws)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

<img width = "600" src="https://raw.githubusercontent.com/SamuelAsherRivello/rmc-jaws/refs/heads/main/RMC%20Jaws/Documentation/ReadMe/Art/Sprites/ProjectBanner.png" />

# RMC J.A.W.S. Library for Unity (Just Amazon Web Services) 

- [How To Use](#how-to-use)
- [Install](#install)
  - [Via NPM](#via-npm)
  - [Or Via Git URL](#or-via-git-url)
- [Optional](#optional)
  - [Tests](#tests)
  - [Samples](#samples)
- [Configuration](#configuration)

<!-- toc -->

## How to use

While finishing an , I refined the API and generalized it for educational and development use-cases.

RMC JAWS demonstrates a solid philosophy to API design and development approach. It is light and accesses key features of Amazon Web Server (AWS) Gaming integration.

It is a great starting point to working with Unity and AWS. Its design serves as a real-world codebase to expand as you need more AWS services. Currently RMC Jaws offers only a few AWS services.

See [SamuelAsherRivello.com/packages/rmc-jaws/](https://www.samuelasherrivello.com/packages/rmc-jaws/) for more info!

Enjoy!

## Install

You can either install [Via NPM](#via-npm) or [Via Git URL](#or-via-git-url). The result will be the same.

### Via NPM

You can either use the Unity Package Manager Window (UPM) or directly edit the manifest file. The result will be the same.

**UPM**

To use the [Package Manager Window](https://docs.unity3d.com/Manual/upm-ui.html), first add a [Scoped Registry](https://docs.unity3d.com/2023.1/Documentation/Manual/upm-scoped.html), then click on the interface menu ( `Status Bar → (+) Icon → Add Package By Name ...` ).

**Manifest File**

Or to edit the `Packages/manifest.json` directly with your favorite text editor, add a scoped registry then the following line(s) to dependencies block:

```json
{
  "scopedRegistries": [
    {
      "name": "npmjs",
      "url": "https://registry.npmjs.org/",
      "scopes": ["com.rmc"]
    }
  ],
  "dependencies": {
    "com.rmc.rmc-jaws": "0.0.1"
  }
}
```

Package should now appear in package manager.

### Or Via Git URL

You can either use the Unity Package Manager (UPM) Window or directly edit the manifest file. The result will be the same.

**UPM**

To use the [Package Manager Window](https://docs.unity3d.com/Manual/upm-ui.html) click on the interface menu ( `Status Bar → (+) Icon → Add Package From Git Url ...` ).

**Manifest File**

Or to edit the `Packages/manifest.json` directly with your favorite text editor, add following line(s) to the dependencies block:

```json
{
  "dependencies": {
    "com.rmc.rmc-jaws": "https://github.com/SamuelAsherRivello/rmc-jaws.git"
  }
}
```

## Optional

### Tests

The package can optionally be set as _testable_.
In practice this means that tests in the package will be visible in the [Unity Test Runner](https://docs.unity3d.com/2017.4/Documentation/Manual/testing-editortestsrunner.html).

Open `Packages/manifest.json` with your favorite text editor. Add following line **after** the dependencies block:

```json
{
  "dependencies": {},
  "testables": ["com.rmc.rmc-jaws"]
}
```

### Samples

Some packages include optional samples with clear use cases. To import and run the samples:

1. Open Unity
1. Complete the package installation (See above)
1. Open the [Package Manager Window](https://docs.unity3d.com/Manual/upm-ui.html)
1. Select this package
1. Select samples
1. Import

## Configuration

- `Unity Target` - [Standalone MAC/PC](https://support.unity.com/hc/en-us/articles/206336795-What-platforms-are-supported-by-Unity-)
- `Unity Version` - Any [Unity Editor](https://unity.com/download) 2022.x or higher
- `Unity Rendering` - Any [Unity Render Pipeline](https://docs.unity3d.com/Manual/universal-render-pipeline.html)
- `Unity Aspect Ratio` - Any [Unity Game View](https://docs.unity3d.com/Manual/GameView.html)

<BR>
<BR>

## Credits

**Created By**

- Samuel Asher Rivello
- Over 25 years XP with game development (2024)
- Over 11 years XP with Unity (2024)

**Contact**

- Twitter - <a href="https://twitter.com/srivello/">@srivello</a>
- Git - <a href="https://github.com/SamuelAsherRivello/">Github.com/SamuelAsherRivello</a>
- Resume & Portfolio - <a href="http://www.SamuelAsherRivello.com">SamuelAsherRivello.com</a>
- LinkedIn - <a href="https://Linkedin.com/in/SamuelAsherRivello">Linkedin.com/in/SamuelAsherRivello</a> <--- Say Hello! :)

**License**

Provided as-is under <a href="./LICENSE">MIT License</a> | Copyright ™ & © 2006 - 2024 Rivello Multimedia Consulting, LLC