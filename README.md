# Mig Model Manager

`com.mig.model` is the model management package for MigSpace. It provides the runtime layer responsible for importing, organizing, selecting, and saving 3D model content inside the MigSpace workflow.

## Package Info

- Package name: `com.mig.model`
- Display name: `Mig Model Manager`
- Current version: `0.0.1`
- Unity version in `package.json`: `2019.4`

## What This Package Provides

This package focuses on model-centric workflows, including:

- loading models into the scene
- managing the current model root and selection state
- importing GLB content
- saving model data to local files or remote storage
- utility helpers for model post-processing and mesh extraction

## Main Modules

### `Runtime/ModelManager`

`ModelManager` is the central entry point of the package.

It is responsible for:

- tracking the active model root
- managing the currently selected object
- coordinating model loaders and savers
- dispatching model-related workflow events
- exposing high-level load and save methods

Representative operations include:

- loading models from a file picker
- loading `.glb` files directly
- saving models to local files
- saving models or project assets to the web through FTP-backed flows

### `Runtime/MeshLoader`

Contains loader implementations for bringing model content into the Unity scene.

Key classes include:

- `GlbFileLoader`
- `ModelFilePickAndLoad`
- `ModelPostProcess`

`GlbFileLoader` uses UnityGLTF to import `.glb` files and attach the loaded objects under the configured parent transform.

### `Runtime/ModelSaver`

Contains saver implementations for exporting model content.

Key classes include:

- `ModelSaveToFile`
- `ModelSaveToWeb`

These classes support exporting model data to local storage or uploading generated output to a remote destination.

### `Runtime/ModelUtils`

Contains geometry and mesh processing helpers used during import and export workflows.

Key classes include:

- `ExtractSubmeshes`
- `ArcenSubmeshSplitter`
- `ModelSaveUtils`

## Folder Structure

```text
Packages/mig.model
|- package.json
|- README.md
|- Runtime
   |- MeshLoader
   |- ModelSaver
   |- ModelUtils
   |- ModelManager.cs
   |- com.mig.modelmanager.runtime.asmdef
```

## Assembly

This package currently includes one runtime assembly:

- `Mig.Model`

## How To Use

### Install through `manifest.json`

Add the package to your Unity project's `Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.mig.model": "git@github.com:HanochZhu/mig.model.git"
  }
}
```

### Use in code

After Unity resolves the package, you can reference it in your scripts:

```csharp
using Mig.Model;
using Mig.Model.ModelLoader;
using Mig.Model.ModelSaver;
```

### Typical responsibilities

This package is usually used together with other Mig packages:

- `com.mig.core` provides shared abstractions such as materials, FTP helpers, and wrapper infrastructure.
- `com.mig.presentation` handles project-level serialization and presentation workflows built on top of loaded model data.

## Development Notes

- The package is designed as runtime infrastructure, not as a standalone application.
- Some save workflows depend on FTP and shared services from `com.mig.core`.
- Import and export behaviors may rely on external packages such as UnityGLTF.

## License

This package follows the license terms used by the main MigSpace repository.
