# C# Snake

A Windows desktop game built with C# and Windows Forms. Double-click **SnakeGame.exe** to play. Requires the Windows .NET Framework 4.x runtime.

## Controls

- Arrow keys or WASD: steer
- Space or P: pause / resume
- R: restart
- Esc: quit

Eat pink food to earn 10 points and grow. Avoid walls and your own body. The game speeds up as your score rises. Switching to another window pauses the game; press Space to resume. Best score lasts for the current session.

## Build from source

Run `powershell -ExecutionPolicy Bypass -File .\build.ps1` from this folder. The script uses the C# compiler included with Windows .NET Framework; no packages or .NET SDK are needed. Game source is in `Program.cs`.
