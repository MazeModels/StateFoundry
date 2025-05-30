# StateFoundry
Implementation of statecharts, a formalism for modeling stateful logic that extends traditional finite state machines.

## Additional resources
- [What are Statecharts?](https://statecharts.dev/)
- [Why state machines and unit testing matter in game architecture](https://chickensoft.games/blog/game-architecture/#-why-write-unit-tests-at-all)
- [XState Introduction to State Machines and Statecharts](https://xstate.js.org/docs/guides/introduction-to-state-machines-and-statecharts/#states)

---

## Installation via OpenUPM (Unity Package Manager)
To make installing the package easier, you can use [OpenUPM](https://openupm.com/) an alternative registry for Unity’s Package Manager.

### Installation Steps:
1. Add the OpenUPM registry to Unity:
   - Open Unity and go to **Window > Package Manager**.
   - Click the gear icon in the top right corner and select **Advanced Project Settings**.
   - Under **Scoped Registries**, click **+** to add a new registry.
   - Enter the following details:
     - **Name:** OpenUPM
     - **URL:** https://package.openupm.com
     - **Scopes:** `com.mazemodels.statefoundry`

2. Install the package:
   - Return to the **Package Manager** window.
   - Click **Add package by name** (or similar, depending on your Unity version).
   - Enter the package name: `com.mazemodels.statefoundry`
   - Click **Add** to install the package into your project.

---

## Unstable API Notice

This package is currently in an **early prototyping phase** and is considered **unstable**. The API may change significantly without prior notice, which can lead to breaking changes and require adjustments in your projects.

We recommend using the package with caution during this phase. The API is expected to stabilize with the release of version **v1.0.0**, at which point backward compatibility will be better maintained.

Thank you for your understanding and feedback as we continue to improve the package.

---

## Author and Licensing

© 2025 Stefano Pittalis — All rights reserved.
_This project is originally created and maintained by Stefano Pittalis._

### License
This repository is licensed under the **MIT License**.  
You are free to use, modify, and distribute this code under the terms of the [MIT License](LICENSE.md).

### Usage in Company Projects
This package can be safely used within your company or personal projects without restrictions beyond the MIT License.  
However, **please note that the ownership and copyright remain with the author**, and this project is not affiliated with any company unless explicitly stated.

### Future Commercial Licensing
In the future, this package may be offered under a **commercial license** on platforms like the Unity Asset Store, which may include additional features, support, or assets not available in this open source version.  
Using this repository does **not** grant rights to commercial versions or redistributed copies without proper licensing.

### Contact
For questions, commercial licensing inquiries, or contributions, please contact: stefano.pittalis@hotmail.it

---

*Thank you for using this package!*
