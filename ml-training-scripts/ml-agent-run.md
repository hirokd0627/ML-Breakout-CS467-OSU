# How to train and run ML-Agents

This document explains how to train the paddle agent and how to use the trained model.

## Training model

1. Copy `./example.yaml` to a new file with your preferred name, e.g., `test1.yaml`
2. Edit `./test1.yaml` according to your training preferences.
3. Follow the steps for the method you prefer: either in the Unity Editor or with a built app.

> **Note:** Using a built app training is faster than training in the Unity Editor.

### Training in the Unity Editor

1. Open the Unity Editor and ML-Breakout project.
2. Run ML-Agents in a terminal.

```sh
mlagents-learn ./ml-training-scripts/test1.yaml --run-id=test1 --force
```

3. Click the **Play** button at the top of the Unity Editor. The training will start.

### Training with a built app

1. In Unity Editor, click `File` -> `Open Scene`.
2. Go to `<repository root>/Assets/Scenes/GameplayScene.unity` and open it.
3. In Unity Editor, click `File` -> `Build Profiles`.
4. In the `Build Profiles` screen, select your platform (`Windows` or `mac`) from the `Platforms` list on the left. Make sure it matches the OS of your Unity Editor.
5. Click `Build` in the top-right corner to execute the build.  
Specify `./Builds/<your OS>` as the output folder for the build.
6. Open Anaconda Prompt and execute `mlagents-learn`.

```sh
# Go to repository root
cd <repository root>

# For Windows
mlagents-learn .\ml-training-scripts/test1.yaml --force --run-id=test1 --no-graphics --env=.\Builds\windows\<app name>

# For macOS
mlagents-learn ./ml-training-scripts/test1.yaml --force --run-id=test1 --no-graphics --env=./Builds/mac/<app name>

# Note: Replace <app name> with the name you specified in Step 5.

# About the parameters: 
# --force       : Overwrites existing results.
# --run-id      : ID for the training run. Results are saved under `results/<run-id>/`.
# --no-graphics : Run training without a game window to speed up the process.
# --env         : Path to the built Unity app (e.g., game-name.exe, game-name.app)
```

### ml-agents command examples

The following examples are for macOS. Windows users will use these commands by changing yaml and env paths.

**With graphics and specified window size**
```sh
mlagents-learn ./ml-training-scripts/test1.yaml \
    --env=./Builds/<app name> \
    --run-id=test1 \
    --force \
    --env-args "--screen-width=400" "--screen-height=200"
```

**No graphics**
```sh
mlagents-learn ./ml-training-scripts/test1.yaml \
    --env=./Builds/<app name> \
    --no-graphics \
    --run-id=test1 \
    --force
```

**Parallel learning**
```sh
mlagents-learn ./ml-training-scripts/test1.yaml \
    --env=./Builds/<app name> \
    --no-graphics \
    --run-id=test1 \
    --num-envs=9 \
    --force
```

## Run the trained model

1. Select the `Assets` folder in the **Project** window.
2. Right-click inside the `Models` folder and choose `Import New Asset...`.
3. Select the `./results/<run-id>/PaddleAgent.onnx` file.
4. Click **Import** to add the model to the `Models` folder.
5. In the **Hierarchy** window, select `MLAgent`, then drag and drop `PaddleAgent.onnx` into **Model** field under **Behavior Parameters** in the **Inspector** window.
6. Click the **Play** button at the top of the Unity Editor.  
The trained model will play the game.

## Related Documentation
Please also refer to *Test ML-Agents Training* section in [environment_setup.md](../Docs/environment_setup.md).
