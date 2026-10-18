using MelonLoader;
using MelonLoader.Utils;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;

[assembly: MelonInfo(typeof(msz_dialogue_skip.Core), "msz dialogue skip", "1.1.0", "Gameknight963", null)]
[assembly: MelonAuthorColor(255, 86, 65, 157)]

namespace msz_dialogue_skip
{
    public class Core : MelonMod
    {
        KeyCode skipKeyCode = KeyCode.Space;

        string? configPath;
        FileSystemWatcher? configWatcher;

        public override void OnInitializeMelon()
        {
            configPath = Path.Combine(MelonEnvironment.ModsDirectory, "msz dialogue skip", "skip_key.cfg");
            Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
            if (!File.Exists(configPath)) File.WriteAllText(configPath, skipKeyCode.ToString());
            else ApplyConfig();
            configWatcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(configPath)!,
                Filter = Path.GetFileName(configPath),
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };
            configWatcher.Changed += ConfigWatcher_Changed;
        }

        private void ConfigWatcher_Changed(object sender, FileSystemEventArgs e) => ApplyConfig();

        void ApplyConfig()
        {
            // never null otherwise the filesystemwatcher wouldn't work
            string text = File.ReadAllText(configPath!);
            if (!Enum.TryParse(text, out KeyCode result))
            {
                LoggerInstance.Error($"skip_key.cfg: '{text}' is not a valid key");
                return;
            }
            LoggerInstance.Msg($"Skip key changed to '{text}'");
            skipKeyCode = result;
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(skipKeyCode) && _tree)
            {
                _tree?.ContinueToNextNode();
            }
        }

        static DialogueTree? _tree;

        [HarmonyPatch(typeof(DialogueTree), nameof(DialogueTree.PlayNode))]
        static class PlayNodePatch
        {
            static bool Prefix(DialogueTree __instance, DialogueNode node)
            {
                _tree = __instance;

                // decompiled: DialogueTree.PlayNode
                // DisplayNode seems to have been inlined by the Il2Cpp optimizer,
                // so we call it here for DisplayNodePatch to work.
                if (node == null)
                {
                    __instance.StartCoroutine(__instance.EndDialogueRoutine());
                    return false;
                }
                __instance.currentNode = node;
                __instance.manager.DisplayNode(node, __instance);
                return false;
            }
        }

        [HarmonyPatch(typeof(DialogueManager), nameof(DialogueManager.DisplayNode))]
        static class DisplayNodePatch
        {
            static Coroutine? handleNodeCoroutine;

            static void Prefix(DialogueManager __instance, DialogueNode node, DialogueTree tree)
            {
                if (handleNodeCoroutine != null) __instance.StopCoroutine(handleNodeCoroutine);
                handleNodeCoroutine = __instance.StartCoroutine(__instance.HandleNode(node, tree));
                if (__instance.dialogueCanvas.enabled && __instance.dialogueCanvas.alpha != 1f)
                {
                    // If this is reached, a skip skipped the fade in animation, fix it manually
                    __instance.dialogueCanvas.alpha = 1f;
                }
            }
        }
    }
}