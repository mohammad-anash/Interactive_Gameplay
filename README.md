# Interactive Mansion Gameplay

## Overview

I created an interactive mansion gameplay system to improve my game development skills. I enjoy building different kinds of things because I believe that whenever I try to create something, I learn new things and face new challenges. These challenges help me improve my skills and become a better developer.

In this README, I'll share the problems I faced while developing this project, how I solved them, and the new things I learned along the way.

## About the Project

This project is an interactive 3D mansion containing 14 different types of interactable objects, such as drawers, bookshelves, lights, a telephone, candles, notes, a gramophone, a random-color cube, and more.

I created an `InteractionManager` that interacts with all interactable objects without needing to know their specific implementations. Each object implements its own behaviour through an interface.

I used an interface to keep the system flexible and easier to extend. If I want to add another interactable object in the future, I only need to create its script and implement the required behaviour. I don't need to modify the existing interaction system.

## Concepts Used in This Project

- 3D Geometry
- Materials
- UI
- Lighting
- Raycasting
- Layers
- Interfaces
- Events
- Mathf Methods
- Object-Oriented Programming

## New Things I Learned While Making This Project

### 1. `Mathf.PerlinNoise()`

**Explanation:**

`Mathf.PerlinNoise()` is a built-in Unity method that takes two `float` arguments and returns a smooth, noise-based value.

Unlike ordinary random values, which can change unpredictably from one value to another, Perlin noise produces values that change gradually. For example, the values might change from `0.10` to `0.15`, then `0.20`, and then `0.28`.

Ordinary random values, on the other hand, might change from `0.5` to `0.9`, then `0.2`, and then `0.7`.

**How I used it:**

I used Perlin noise in my `CandleController` script to produce smoothly changing values over time. I used `Time.time` as one of its arguments so the noise value would change as time passed.

You can check the `CandleController` script to see how I implemented it.

**Important note:** Perlin noise values are generally between `0` and `1`. If you need a particular range for your use case, you can clamp or otherwise adjust the result as needed.

### 2. `Transform.TransformPoint()`

**Explanation:**

`TransformPoint()` converts a position from an object's local space into world space. It takes a `Vector3` representing the local position and returns a `Vector3` representing the corresponding world position.

**Example:**

Suppose an object has the following world position:

```csharp
new Vector3(10f, 0f, 5f);
```

We want to find the world position of a point located two units forward in its local space:

```csharp
Vector3 position = transform.TransformPoint(
    new Vector3(0f, 0f, 2f)
);
```

If the object has no rotation and its scale is `(1, 1, 1)`, the resulting world position will be:

```csharp
new Vector3(10f, 0f, 7f);
```

`TransformPoint()` automatically accounts for the object's position, rotation, and scale. Therefore, if the object rotates, the resulting world position changes accordingly.

**Where it is useful:**

- Finding a position relative to an object.
- Positioning visual effects.
- Placing objects at specific points relative to a character or weapon.
- Calculating spawn positions for projectiles.

### 3. `Physics.CheckBox()`

**Explanation:**

`Physics.CheckBox()` checks whether any colliders overlap an invisible, box-shaped area in the 3D world.

It returns a `bool` value:

- `true` if at least one collider overlaps the box.
- `false` if no collider overlaps the box.

**Arguments:**

The method accepts two required arguments and three optional arguments.

| Argument                  | Type                      | Purpose                                          |
| ------------------------- | ------------------------- | ------------------------------------------------ |
| `center`                  | `Vector3`                 | The center position of the box.                  |
| `halfExtents`             | `Vector3`                 | Half the box's size along the X, Y, and Z axes.  |
| `orientation`             | `Quaternion`              | The rotation of the box.                         |
| `layerMask`               | `LayerMask` or `int`      | Specifies which layers Unity should check.       |
| `queryTriggerInteraction` | `QueryTriggerInteraction` | Controls whether trigger colliders are included. |

**Example:**

```csharp
bool isBlocked = Physics.CheckBox(
    transform.position,
    new Vector3(2f, 1f, 3f),
    transform.rotation
);
```

In this example, the box is centered at the object's position, uses the object's rotation, and has half-extents of `(2, 1, 3)`. Therefore, its full size is `4 × 2 × 6` units.

If a collider overlaps the box, `isBlocked` becomes `true`; otherwise, it becomes `false`.

**Important:** `halfExtents` represents half the box's size, not its full size. Also, `Physics.CheckBox()` checks for any collider unless you filter the results using a layer mask or other appropriate settings.

### 4. `transform.lossyScale`

**Explanation:**

`transform.lossyScale` returns the approximate scale of a GameObject in world space, including the effects of its parent objects' scaling. Its type is `Vector3`.

It tells you how large the object appears in world space, rather than just its local scale.

**Example:**

Parent Object Scale:

```csharp
new Vector3(2f, 2f, 2f);
```

Child Object Local Scale:

```csharp
new Vector3(3f, 1f, 1f);
```

**Result:**

```csharp
new Vector3(6f, 2f, 2f);
```

- `localScale` tells you the scale relative to the parent.
- `lossyScale` tells you the approximate scale in world space.

### 5. `Mathf.Approximately()`

**Explanation:**

`Mathf.Approximately()` is used to compare two floating-point numbers and check whether they are close enough to be considered equal. It returns `true` if they are sufficiently close; otherwise, it returns `false`.

But why not use the `==` operator? Because computers store floating-point numbers in binary, small precision differences can occur during calculations. Using `==` may therefore produce unexpected results when comparing floating-point values.

`Mathf.Approximately()` can be a better choice when a small difference is acceptable.

**Arguments:**

It takes two arguments, and both are of type `float`.

### 6. `GetComponentInParent()`

**Explanation:**

`GetComponentInParent()` searches for a component of the specified type on the current GameObject first. If the component is not found, it searches the parent objects.

If it finds the component, it returns that component. If it cannot find the component in the searched hierarchy, it returns `null`.

**Argument:**

- The type of component you want to find.

## Problems I Faced While Making This Project

### Problem 1: Interaction Manager Could Interact with Objects Through Walls

**Explanation:**

While testing my gameplay, I was interacting with objects while walking on the stairs. Suddenly, I noticed that the interaction prompt appeared even when a wall was between my player and the object.

At first, I had set the Interaction Manager's layer mask to include only interactable objects. Later, I changed it to include everything. This allowed the raycast to detect other objects as well, so walls could block the ray before it reached an interactable object.

**Solution:**

I configured the raycast to detect objects in the scene, including walls, while checking whether the hit object implements the `IInteractable` interface before allowing interaction. This prevents the player from interacting with objects through walls, provided the blocking geometry has appropriate colliders and is included in the raycast's layer mask.

### Problem 2: The Player Was in the Way of Moving Objects

**Explanation:**

I noticed that my player was sometimes in the path of moving objects, such as drawers, bookshelves, and the safe door. When a moving object collided with the player, they interfered with each other and started jittering.

**Solution:**

I created a child GameObject for each moving object—the drawer, bookshelf, and safe door—and attached a `BoxCollider` shaped to cover the relevant area. I extended the collider along the direction in which the object moves.

I also created a script and attached it to an empty GameObject called `PlayerZone`. It checks whether the player would be in the way of the object's next movement. If the player is in the way, the object stops moving. Otherwise, it continues moving and checks again before its next movement.

I attached this script to the drawer, bookshelf, and safe door. The same script can be reused by multiple objects, which helps avoid duplicating code.

### Problem 3: Duplicate Code in Every UI Panel Script

**Explanation:**

At first, each UI panel—such as the Note Panel, Safe Keypad Panel, and Ending Screen Panel—had its own script to implement fade-in and fade-out animations.

This resulted in duplicate code across multiple UI panel scripts.

**Solution:**

I decided to create a reusable `UIPanel` script that could be attached to all three panels. It handles fade-in, fade-out, and scale animations using methods such as `Show()`, `Hide()`, and `HideImmediate()`.

If I need to add a new panel in the future, I can reuse the same `UIPanel` script instead of implementing the same animations again.

I also added a scale-up animation to make the UI panels look more interesting.

### Problem 4: Z-Flipping

**Explanation:**

To be Honest I didn't find any solution of it, I'll search about it more i just increase one Object Scale to prevent. but it's not good Practice. I want you to guys share the solution, And Tell me How to fix it.

### Third-Party Tool Used in This Project

I used a tool called **Fast Play**. It helped me improve my workflow and save time while working on my projects.

After importing it into your Unity project, its button appears alongside the Play Mode button in the Unity Editor. When you click it, it enters Play Mode in around 5–10 seconds, depending on your project and system. This makes testing your game faster and helps you save time during development.

**Shortcut to Toggle Fast Play:** `Shift + Alt + P`

**Note:** Be careful when using Fast Play because it disables Domain Reload. This means static variables may keep their previous values when you enter Play Mode again.

To prevent this, you can use Unity's `[RuntimeInitializeOnLoadMethod]` attribute with `RuntimeInitializeLoadType.SubsystemRegistration`.

- **`[RuntimeInitializeOnLoadMethod]`** tells Unity to call a method automatically during runtime initialization.
- **`RuntimeInitializeLoadType.SubsystemRegistration`** specifies that the method should run during the subsystem registration stage, which happens early when entering Play Mode.
- **The reset method** contains the code that resets your static variables to their default values.

For example:

```csharp
[RuntimeInitializeOnLoadMethod(
    RuntimeInitializeLoadType.SubsystemRegistration)]
private static void ResetStaticVariables()
{
    Score = 0;
}
```

In this example, Unity automatically calls `ResetStaticVariables()` when entering Play Mode, resetting `Score` to `0`. This helps prevent static variables from retaining values from a previous Play Mode session when Domain Reload is disabled.

**Link:** [Fast Play - Unity Asset Store](https://assetstore.unity.com/packages/tools/utilities/fast-play-353186)

### Performance Optimization Techniques Used in This Project

I used two techniques to optimize the performance of my Interactive Mansion project: **Static Objects** and **Light Baking**.

#### 1. Static Objects

I marked non-movable objects, such as walls, floors, ceilings, and stationary furniture, as Static in Unity. This allows Unity to use relevant static optimizations, such as static batching and baked occlusion culling, when properly configured.

#### 2. Light Baking

I selected lights that never turn off during gameplay and changed their Mode to `Baked`. Then, I opened **Window → Rendering → Lighting**, selected the **Scene** tab, and clicked **Generate Lighting** to bake the scene's lighting.

Light baking allows Unity to calculate lighting in advance and store the results in lightmaps, reducing the need for certain lighting calculations during gameplay.

**Result:**

After applying these techniques, I noticed a small improvement in the game's FPS. This helped me understand how static objects and baked lighting can contribute to performance optimization in Unity.

How to Download and Play on Windows

Follow these steps to play the game on your Windows PC.

**Step 1: Download the Game**

Download the Windows build ZIP file from the link provided below.

[Download Game: Interactive Mansion - Windows Build](https://github.com/mohammad-anash/Interactive_Gameplay/releases/download/v1.0.0/InteractiveMansion-Windows.zip)

**Step 2: Extract the ZIP File**

Locate the downloaded ZIP file in your Downloads folder.

Right-click the ZIP file.

Select Extract All....

Choose a destination folder where you want to extract the game.

Click Extract and wait for the process to finish.

**Step 3: Launch the Game**

Open the extracted folder.

Find the game executable (.exe file).

Double-click the .exe file to launch the game.

Wait for the game to load, then enjoy exploring the mansion!

## Final Thoughts

This is not a complete game; it's just an interactive house gameplay project I made to improve my skills and explore new things. I know it still has some issues, there is still room for improvement. However, I learned so many new things from this project.

I hope you'll like it! Please share your opinion on it or let me know if there is anything I can improve. It would be great to hear your feedback.

Maybe your feedback will help me improve my project and learn something new.
