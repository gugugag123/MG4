# MG4
## Devlog
The `FloatAnimation` script makes the food bob up and down. When the scene starts, `Start()` sets `_timer` to `_switchSpeed`. This gives the food a full interval before it changes direction. The bool `_up` starts as `false`, so the food moves down first. In `Update()`, which runs every frame, the script checks `_up` to decide which way to move. If it is true, the food moves up; otherwise, it moves down. It uses `transform.Translate()` to move along the Y axis, with `_moveSpeed * Time.deltaTime` setting the distance each frame. Using `Time.deltaTime` keeps the movement speed consistent even when the frame rate changes.

At the same time, `Update()` subtracts `Time.deltaTime` from `_timer`. Once the timer reaches zero or goes below it, the script resets it to `_switchSpeed` and switches `_up` with `_up = !_up`. So if the food was moving down, it now moves up, and the same thing happens again when the timer runs out. That repeated switch is what creates the bobbing motion. One thing to keep straight is that `_switchSpeed` controls how long the food moves in each direction, while `_moveSpeed` controls how fast it moves. In my scene, I used a movement speed of 0.3 and a 1-second interval. This gives the food a small floating motion while keeping it close enough to its starting position to collect.
## Controls
- W / Up: swim forward; S / Down: swim backward.
- A / D or Left / Right: turn; Q: swim up; E: swim down.
- Touch a yellow sphere to open its addition, subtraction, or multiplication question. Type an integer answer and press Enter. Subtraction can have negative answers.
- Correct answers give one point and remove that food. Incorrect answers move the fish backward so you can try again. Touched food stops bobbing.

## Implementation
The Main scene includes 50 separate trigger spheres: 17 Add, 17 Subtract, and 16 Multiply. A seeded random layout spreads 12 over vehicle roofs, 28 over building roofs, and 10 around street areas. Roof heights come from the actual model bounds, with clearance for the sphere and its floating animation. The layout stays the same on each playthrough so it is easy to test and revisit. The fish has a collider and a Rigidbody with gravity disabled. Each question-checking method returns whether the typed integer equals the operation's result. Only food triggers start questions, and another food cannot replace an unanswered question.

## Open-Source Assets
- [Quirky animal 3D models](https://assetstore.unity.com/packages/3d/characters/animals/quirky-series-free-animals-pack-178235)
- [City 3D models](https://assetstore.unity.com/packages/3d/environments/simplepoly-city-low-poly-assets-58899)
- [Bite sound effect](https://freesound.org/people/Jofae/sounds/353067/)

