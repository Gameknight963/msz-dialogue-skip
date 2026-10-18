using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;

[assembly: MelonInfo(typeof(msz_dialogue_skip.Core), "msz dialogue skip", "1.0.0", "Gameknight963", null)]
[assembly: MelonAuthorColor(255, 86, 65, 157)]

namespace msz_dialogue_skip
{
    public class Core : MelonMod
    {
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _tree)
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