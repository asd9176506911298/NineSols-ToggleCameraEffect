using BepInEx;
using BepInEx.Configuration;
using Cysharp.Threading.Tasks;
using HarmonyLib;
using NineSolsAPI;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ToggleCameraEffect;

[BepInDependency(NineSolsAPICore.PluginGUID)]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ToggleCameraEffect : BaseUnityPlugin {
    private ConfigEntry<bool> disableCameraEffect = null!;
#if DEBUG
    private ConfigEntry<KeyboardShortcut> somethingKeyboardShortcut = null!;
#endif

    private Harmony harmony = null!;

    private void Awake() {
        Log.Init(Logger);
        RCGLifeCycle.DontDestroyForever(gameObject);

        harmony = Harmony.CreateAndPatchAll(typeof(ToggleCameraEffect).Assembly);

        disableCameraEffect = Config.Bind("", "Disable Camera Effect", false, "");
#if DEBUG
        somethingKeyboardShortcut = Config.Bind("General.Something", "Shortcut",
            new KeyboardShortcut(KeyCode.H, KeyCode.LeftControl), "Shortcut to execute");
#endif

        KeybindManager.Add(this, TestMethod, () => somethingKeyboardShortcut.Value);

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        disableCameraEffect.SettingChanged += DisableCameraEffect_SettingChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void DisableCameraEffect_SettingChanged(object sender, System.EventArgs e) {
        bool disable = disableCameraEffect.Value;

        foreach (var x in GameObject.FindObjectsOfType<CameraHUDEffect>()) {
            var go = x.gameObject;

            // Animator 處理（無論開或關都要）
            if (go.TryGetComponent(out Animator animator))
                animator.enabled = !disable;

            if (!disable) continue; // 如果不關閉，就跳過特效禁用

            // 以下只在 disable = true 時執行
            if (go.TryGetComponent(out CameraFilterPack_Vision_Blood_Fast bloodFast))
                bloodFast.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_Lut_Mask lutMask))
                lutMask.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_NewGlitch4 glitch))
                glitch.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_FX_Screens screens))
                screens.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_Blur_Focus blurFocus))
                blurFocus.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_Color_Adjust_Levels levels))
                levels.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_FX_EarthQuake earthquake))
                earthquake.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_3D_Computer computer))
                computer.enabled = false;
        }
    }

#if DEBUG
    private void TestMethod() {
        ToastManager.Toast("Shortcut activated");
        
    }
#endif

    async UniTask checkMove() {
        // 每 1000 毫秒檢查一次 Player 是否存在且已開始移動
        while (Player.i == null || Player.i.moveVec.x == 0f) {
            await UniTask.Delay(1000); // 1 秒檢查一次
        }
    }

    private async void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (!disableCameraEffect.Value) return;

        await checkMove();

        foreach (var x in GameObject.FindObjectsOfType<CameraHUDEffect>()) {
            var go = x.gameObject;

            if (go.TryGetComponent(out Animator animator))
                animator.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_Vision_Blood_Fast bloodFast))
                bloodFast.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_Lut_Mask lutMask))
                lutMask.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_NewGlitch4 glitch))
                glitch.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_FX_Screens screens))
                screens.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_Blur_Focus blurFocus))
                blurFocus.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_Color_Adjust_Levels levels))
                levels.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_FX_EarthQuake earthquake))
                earthquake.enabled = false;

            if (go.TryGetComponent(out CameraFilterPack_3D_Computer computer))
                computer.enabled = false;
        }
    }




    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        harmony.UnpatchSelf();
    }
}