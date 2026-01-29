# Unity Game Demo

[![](https://markdown-videos-api.jorgenkh.no/youtube/6MAQlIkuKoM)](https://youtu.be/6MAQlIkuKoM)
<br>
[Demo Youtube Video](https://youtu.be/6MAQlIkuKoM)
<br><br>
[Windows Build](https://github.com/knightgames-tr/PlayableAdsDev/blob/main/windows_build.zip)

### Developed using Unity 3D and the C# programming language.

**Additional Assets Used:** DOTween, Joystick Pack

#### Development Notes:

* Manager classes were implemented using the Singleton pattern.
* A parent class named StandPoint was defined for standing points, with ActionPoint and PaymentPoint classes extending from it.
* A LineController class was created to handle NPC queueing. NPCs are positioned in line using a dynamic offset value.
* When the NPC at the front of the queue completes its task, it notifies the LevelManager, allowing the next NPC to proceed.
* Stair movement was implemented by moving each step object to the next point within a looping sequence.
* While implementing the plank painting feature, the pixel corresponding to the touched point is first identified. Neighboring pixels are calculated using the brush size as a radius and then painted on the texture. A check is performed to determine whether the pixel was previously white, and the painting percentage is calculated based on this condition.
* Player controls were implemented using the Character Controller.
* Trail and money spending effects were created using the Particle System.
* Camera transitions were handled using Cinemachine.
* For various animations, chained DOTween tween features were utilized.
* An Avatar Mask was used to allow holding and walking animations to play simultaneously, and an Animation Layer was created.
* Level-specific, more specialized, hard-coded actions were defined within the LevelManager using Coroutines.
