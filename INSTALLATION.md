# Installation Guide

## Required Software

- Unity 6
- Unity ML-Agents package
- Python 3.10 recommended
- VS Code
- Git
- Docker optional

## Unity Setup

1. Open Unity Hub.
2. Add the `UnityProject` folder.
3. Open the project using Unity 6.
4. In Package Manager, install:
   - ML Agents
   - Barracuda or Sentis, depending on the Unity ML-Agents version
   - TextMeshPro
5. Add an ML-Agents `Behavior Parameters` component to the robot agent.
6. Set behaviour name to `RoboticPaintingAgent`.
7. Set action space to continuous with 4 actions.
8. Set observation space according to `RoboticArmAgent.cs`.

## Python Setup

```bash
cd ExpressiveRoboticPainting
python -m venv .venv
source .venv/bin/activate
pip install -r Python/requirements.txt
```

## Demonstration Recording Setup

In Unity, add:

- `RoboticArmAgent`
- `Behavior Parameters`
- `Demonstration Recorder`
- `TrajectoryLogger`
- `ManualRobotController`
- `DemonstrationStyleManager`

Record at least 10 demonstrations for each style. A stronger experimental setup should use 20 demonstrations per style.
