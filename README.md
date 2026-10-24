# MG4
## Devlog
In `FloatAnimation`, `Start()` sets `_timer` to `_switchSpeed`, so the first movement interval begins with the full amount of time. `_up` is a private bool that starts as `false`, which means the food moves down first. Unity calls `Update()` every frame. It checks `_up` and uses `transform.Translate()` to move the food along the Y axis: positive Y when `_up` is true and negative Y otherwise. The distance is `_moveSpeed * Time.deltaTime`, so the movement is based on seconds rather than the number of frames.

`Update()` also subtracts `Time.deltaTime` from `_timer` each frame. When `_timer` reaches zero or less, the script resets it to `_switchSpeed` and flips `_up` using `_up = !_up`. That makes the food travel in the opposite direction for the next interval. Repeating this process creates the up-and-down animation. `_switchSpeed` controls how many seconds pass before the direction changes; it does not control the actual movement speed. For this scene I set each food object's movement speed to 0.3 and switching interval to 1 second, so it stays close to its starting height and remains easy to reach.

## Controls
- W / Up: swim forward; S / Down: swim backward.
- A / D or Left / Right: turn; Q: swim up; E: swim down.
- Touch a yellow sphere to open its addition, subtraction, or multiplication question. Type an integer answer and press Enter. Subtraction can have negative answers.
- Correct answers give one point and remove that food. Incorrect answers move the fish backward so you can try again. Touched food stops bobbing.

## Implementation
The Main scene includes three separate trigger spheres, tagged Add, Subtract, and Multiply. The fish has a collider and a Rigidbody with gravity disabled. Each question-checking method returns whether the typed integer equals the operation's result. Only food triggers start questions, and another food cannot replace an unanswered question.

## Open-Source Assets
- [Quirky animal 3D models](https://assetstore.unity.com/packages/3d/characters/animals/quirky-series-free-animals-pack-178235)
- [City 3D models](https://assetstore.unity.com/packages/3d/environments/simplepoly-city-low-poly-assets-58899)
- [Bite sound effect](https://freesound.org/people/Jofae/sounds/353067/)
