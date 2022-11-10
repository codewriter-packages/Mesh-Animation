# Mesh Animation [![Github license](https://img.shields.io/github/license/codewriter-packages/Mesh-Animation.svg)](#)
Mesh Animation is lightweight library for rendering hundreds of meshes in one draw call with GPU instancing.

#### NOTE: To use MeshAnimation library you need to install [Tri Inspector](https://github.com/codewriter-packages/Tri-Inspector) - Free and open-source library that improves unity inspector.

![preview](https://user-images.githubusercontent.com/26966368/201170891-99093ad9-6bd6-4ed3-a81e-1cd0f1becb55.png)

## How it works?
Mesh Animation bakes vertex positions for each frame of animation to texture. Custom shader then move mesh vertexes to desired positions on GPU. This allows draw the same original mesh multiple times with GPU Instancing. Unique animation parameters are overridden for each instance with Material Property Block.

## Limitations
* Supported up to 2048 vertices per mesh.
* Bakes one SkinnedMeshRenderer animation per prefab.
* Requires special shader for vertex animations.
* Animations can only be baked in editor mode.
* Possibly low animation quality on some GPUs.
* Vertex animation may be not supported on some old devices.

## How to use?

1. Create Mesh Animation Asset (in `Assets/Create/Mesh Animation` menu).
2. Assign skin, shader and animation clips fields in inspector.
3. Click `Bake` button.
4. Assign generated material to gameObject.
5. Add `MeshAnimator` component to gameObject.
6. Play animation from code.
```c#
gameObject.GetComponent<MeshAnimator>().Play("Zombie Walking");
```
<br>

[![Mesh Animation](https://user-images.githubusercontent.com/26966368/92770369-90559200-f3a2-11ea-9f1f-37719a0637c7.png)](#)

## FAQ

##### Which Rig AnimationType are supported?
Works with Humanoid. Not works with legacy. Other not tested.
