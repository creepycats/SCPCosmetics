namespace SCPCosmetics.Patches
{
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using Mirror;
    using RelativePositioning;
    using SCPCosmetics.Cosmetics.Extensions;
    using SCPCosmetics.Cosmetics.Hats;
    using System;
    using System.Linq;
    using UnityEngine;

    //public static class NetworkHelper {
    //    public static void SendTargetCustomRPCInternal(this NetworkBehaviour nb, NetworkConnection conn, string functionFullName, int functionHashCode, NetworkWriter writer, int channelId)
    //    {
    //        if (!NetworkServer.active)
    //        {
    //            Debug.LogError("TargetRPC " + functionFullName + " was called on " + nb.name + " when server not active.", nb.gameObject);
    //            return;
    //        }

    //        if (!nb.isServer)
    //        {
    //            Debug.LogWarning("TargetRpc " + functionFullName + " called on " + nb.name + " but that object has not been spawned or has been unspawned.", nb.gameObject);
    //            return;
    //        }

    //        if (conn == null)
    //        {
    //            conn = nb.connectionToClient;
    //        }

    //        if (conn == null)
    //        {
    //            Debug.LogError("TargetRPC " + functionFullName + " can't be sent because it was given a null connection. Make sure " + nb.name + " is owned by a connection, or if you pass a connection manually then make sure it's not null. For example, TargetRpcs can be called on Player/Pet which are owned by a connection. However, they can not be called on Monsters/Npcs which don't have an owner connection.", nb.gameObject);
    //        }
    //        else if (!(conn is NetworkConnectionToClient))
    //        {
    //            Debug.LogError("TargetRPC " + functionFullName + " called on " + nb.name + " requires a NetworkConnectionToClient but was given " + conn.GetType().Name, nb.gameObject);
    //        }
    //        else
    //        {
    //            RpcMessage rpcMessage = default(RpcMessage);
    //            rpcMessage.netId = nb.netId;
    //            rpcMessage.componentIndex = nb.ComponentIndex;
    //            rpcMessage.functionHash = (ushort)functionHashCode;
    //            rpcMessage.payload = writer.ToArraySegment();
    //            RpcMessage message = rpcMessage;
    //            conn.Send(message, channelId);
    //        }
    //    }

    //    public static void SendCustomPhysicsModuleRpc(this ItemPickupBase ipb, ArraySegment<byte> arrSegReal, ArraySegment<byte> arrSegFake)
    //    {
    //        if (!HatsHandler.TryGetHat(ipb, out HatComponent hic)) return;

    //        NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
    //        NetworkWriterExtensions.WriteArraySegmentAndSize((NetworkWriter)networkWriterPooled, arrSegFake);
    //        foreach (Player plr in Player.List.Where(play => !hic.IsVisibleToPlayer(play) || Player.Get(hic.gameObject).ReferenceHub == play.ReferenceHub))
    //        {
    //            ipb.SendTargetCustomRPCInternal(plr.Connection, "System.Void InventorySystem.Items.Pickups.ItemPickupBase::SendPhysicsModuleRpc(System.ArraySegment`1<System.Byte>)", 254399230, networkWriterPooled, 0);
    //        }
    //        NetworkWriterPool.Return(networkWriterPooled);

    //        networkWriterPooled = NetworkWriterPool.Get();
    //        NetworkWriterExtensions.WriteArraySegmentAndSize((NetworkWriter)networkWriterPooled, arrSegReal);
    //        foreach (Player plr in Player.List.Where(play => hic.IsVisibleToPlayer(play) && Player.Get(hic.gameObject).ReferenceHub != play.ReferenceHub))
    //        {
    //            ipb.SendTargetCustomRPCInternal(plr.Connection, "System.Void InventorySystem.Items.Pickups.ItemPickupBase::SendPhysicsModuleRpc(System.ArraySegment`1<System.Byte>)", 254399230, networkWriterPooled, 0);
    //        }
    //        NetworkWriterPool.Return(networkWriterPooled);
    //    }

    //    public static void CustomWriteRigidbody(this PickupStandardPhysics __instance, NetworkWriter writer)
    //    {
    //        writer.WriteByte(__instance._serverOrderClock++);
    //        RelativePosition msg = new RelativePosition(__instance.Rb.position);
    //        Quaternion relativeRotation = WaypointBase.GetRelativeRotation(msg.WaypointId, __instance.Rb.rotation);
    //        writer.WriteRelativePosition(msg);
    //        writer.WriteLowPrecisionQuaternion(new LowPrecisionQuaternion(relativeRotation));
    //        if (!__instance.ServerSendFreeze)
    //        {
    //            NetworkWriterExtensions.WriteVector3(writer, __instance.Rb.velocity);
    //            NetworkWriterExtensions.WriteVector3(writer, __instance.Rb.angularVelocity);
    //        }
    //    }
    //}

    //[HarmonyPatch(typeof(PickupPhysicsModule), nameof(PickupPhysicsModule.ServerSendRpc))]
    //internal static class ServerSendRpc
    //{
    //    [HarmonyPrefix]
    //    private static bool Prefix(ref PickupPhysicsModule __instance, Action<NetworkWriter> writerMethod)
    //    {
    //        return true;
    //        if (!HatsHandler.IsHat(__instance.Pickup)) return true;

    //        if (!NetworkServer.active)
    //        {
    //            Debug.LogWarning((object)"[Server] function 'System.Void InventorySystem.Items.Pickups.PickupPhysicsModule::ServerSendRpc(System.Action`1<Mirror.NetworkWriter>)' called when server was not active");
    //        }
    //        else if (__instance.IsSpawned)
    //        {
    //            ArraySegment<byte> realData;
    //            ArraySegment<byte> fakeData;

    //            PickupStandardPhysics standardPhys = __instance as PickupStandardPhysics;

    //            Vector3 oldpos = __instance.Pickup.Position;
    //            using NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
    //            standardPhys.CustomWriteRigidbody(networkWriterPooled);
    //            realData = networkWriterPooled.ToArraySegment();

    //            __instance.Pickup.Position = new Vector3(6000f, 6000f, 6000f);
    //            using NetworkWriterPooled networkWriterPooledAlt = NetworkWriterPool.Get();
    //            standardPhys.CustomWriteRigidbody(networkWriterPooledAlt);
    //            fakeData = networkWriterPooledAlt.ToArraySegment();
    //            __instance.Pickup.Position = oldpos;

    //            __instance.Pickup.SendCustomPhysicsModuleRpc(realData, fakeData);
    //        }

    //        return false;
    //    }
    //}

    [HarmonyPatch(typeof(PickupPhysicsModule), nameof(PickupPhysicsModule.ServerSetSyncData))]
    internal static class ServerSetSyncData
    {
        [HarmonyPrefix]
        private static bool Prefix(ref PickupPhysicsModule __instance, Action<NetworkWriter> writerMethod)
        {
            if (HatsHandler.IsHat(__instance.Pickup) && HatsHandler.TryGetHat(__instance.Pickup, out HatComponent hatComp))
            {
                if (hatComp._hasSyncedPickup)
                    return false;
                else
                {
                    hatComp._hasSyncedPickup = true;
                }
            }
            return true;
        }
    }
}