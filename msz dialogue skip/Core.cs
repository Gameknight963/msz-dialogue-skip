using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using UnityEngine;
using UnityEngine.Rendering;

[assembly: MelonInfo(typeof(msz_dialogue_skip.Core), "msz dialogue skip", "1.0.0", "Gameknight963", null)]
[assembly: MelonAuthorColor(255, 86, 65, 157)]

namespace msz_dialogue_skip
{
    public class Core : MelonMod
    {
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _tree )
            {
                _tree?.ContinueToNextNode();
            }
        }

        static DialogueTree? _tree;

        [HarmonyPatch(typeof(DialogueTree), nameof(DialogueTree.PlayNode))]
        static class PlayNodePatch
        {
            static void Prefix(DialogueTree __instance, DialogueNode node)
            {
                _tree = __instance;

                // decompiled: DialogueTree.PlayNode
                // DisplayNode seems to have been inlined by the Il2Cpp optimizer,
                // so we call it here for DisplayNodePatch to work.
                if (node == null)
                {
                    __instance.StartCoroutine(__instance.EndDialogueRoutine());
                    return;
                }
                __instance.currentNode = node;
                __instance.manager.DisplayNode(node, __instance);
            }
        }

        [HarmonyPatch(typeof(DialogueManager), nameof(DialogueManager.DisplayNode))]
        static class DisplayNodePatch
        {
            static Coroutine? coroutine;

            static void Prefix(DialogueManager __instance, DialogueNode node, DialogueTree tree)
            {
                MelonLogger.Msg("displaynode");
                if (coroutine != null) __instance.StopCoroutine(coroutine);
                coroutine = __instance.StartCoroutine(__instance.HandleNode(node, tree));
            }
        }
    }
}