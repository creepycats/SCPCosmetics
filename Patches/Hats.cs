namespace SCPCosmetics.Patches
{
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using MapGeneration;
    using Mirror;
    using NorthwoodLib.Pools;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl.Thirdperson;
    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerRoles.PlayableScps.Scp939.Ripples;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using UnityEngine;

    //[HarmonyPatch(typeof(PickupStandardPhysics), nameof(PickupStandardPhysics.UpdateClient))]
    //internal static class UpdateClient
    //{
    //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    //    {
    //        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

    //        Label skip = generator.DefineLabel();

    //        // Insert instructions to skip when NPC to the skip label
    //        //newInstructions.Add(new CodeInstruction(OpCodes.Ret));
    //        newInstructions[38].labels.Add(skip);

    //        newInstructions.InsertRange(17, new List<CodeInstruction>()
    //        {
    //            new CodeInstruction(OpCodes.Ldarg_0),
    //            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Hats), nameof(Hats.IsHat), new[] { typeof(PickupStandardPhysics) })),
    //            new CodeInstruction(OpCodes.Brtrue_S, skip),
    //        });

    //        foreach (CodeInstruction instruction in newInstructions)
    //            yield return instruction;

    //        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    //    }
    //}

    //[HarmonyPatch(typeof(PickupStandardPhysics), nameof(PickupStandardPhysics.UpdateServer))]
    //internal static class UpdateServer
    //{
    //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    //    {
    //        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

    //        Label skip = generator.DefineLabel();

    //        // Insert instructions to skip when Hat Item to the skip label
    //        newInstructions[19].labels.Add(skip);

    //        newInstructions.InsertRange(0, new List<CodeInstruction>()
    //        {
    //            new CodeInstruction(OpCodes.Ldarg_0),
    //            new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Hats), nameof(Hats.IsHat), new[] { typeof(PickupStandardPhysics) })),
    //            new CodeInstruction(OpCodes.Brtrue_S, skip),
    //        });

    //        foreach (CodeInstruction instruction in newInstructions)
    //            yield return instruction;

    //        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    //    }
    //}

    //[HarmonyPatch(typeof(PickupStandardPhysics), nameof(PickupStandardPhysics.UpdateClient))]
    //internal static class UpdateClient
    //{
    //    [HarmonyPrefix]
    //    private static bool Prefix(ref PickupStandardPhysics __instance)
    //    {
    //        if (Hats.IsHat(__instance))
    //        {
    //            float sqrMagnitude = __instance.Rb.velocity.sqrMagnitude;
    //            if (sqrMagnitude < __instance._serverPrevVelSqr)
    //            {
    //                __instance._serverEverDecelerated = true;
    //            }

    //            __instance._serverPrevVelSqr = sqrMagnitude;
    //            if (!__instance._serverPrevSleeping && __instance._serverNextUpdateTime > NetworkTime.time)
    //            {
    //                return false;
    //            }

    //            __instance.ServerSendRpc(__instance.ServerWriteRigidbody);
    //            __instance._serverNextUpdateTime = NetworkTime.time + 0.05;
    //            return false;
    //        }

    //        return true;
    //    }
    //}

    //[HarmonyPatch(typeof(PickupStandardPhysics), nameof(PickupStandardPhysics.UpdateClient))]
    //internal static class UpdateClient
    //{
    //    [HarmonyPrefix]
    //    private static bool Prefix(ref PickupStandardPhysics __instance)
    //    {
    //        if (__instance.ClientFrozen && !(__instance._freezeProgress > 1f) && SeedSynchronizer.MapGenerated)
    //        {
    //            Vector3 position = __instance.Rb.position;
    //            Vector3 lastWorldPos = __instance.LastWorldPos;
    //            if ((position - lastWorldPos).sqrMagnitude > 25f / 64f && !Hats.IsHat(__instance))
    //            {
    //                __instance._freezeProgress = 0f;
    //                __instance.Rb.position = lastWorldPos;
    //                __instance.Rb.rotation = __instance.LastWorldRot;
    //            }
    //            else
    //            {
    //                __instance._freezeProgress += Time.deltaTime * 1.2f;
    //                float t = Mathf.Lerp(20f * Time.deltaTime, 1f, Mathf.Pow(__instance._freezeProgress, 5f));
    //                __instance.Rb.position = Vector3.Lerp(position, lastWorldPos, t);
    //                __instance.Rb.rotation = Quaternion.Lerp(__instance.Rb.rotation, __instance.LastWorldRot, t);
    //            }
    //        }

    //        return false;
    //    }
    //}
}