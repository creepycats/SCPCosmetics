using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using MapEditorReborn.API.Features;
using MEC;
using Mirror;
using PlayerRoles;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SCPCosmetics
{
    public class Pets
    {
        public static void SpawnDum(string Name, RoleTypeId Role, Player target)
        {
            Log.Info("Spawning Dummy");
            GameObject newPlayer = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
            Player NewPlayer = new Player(newPlayer);
            Log.Info("Got Player from object");
            ReferenceHub hubPlayer = NewPlayer.ReferenceHub;
            Log.Info("Got ReferenceHub from Player");
            SCPCosmetics.Instance.PetDummies.Add(hubPlayer);
            Log.Info("Added Dummy to List");
            hubPlayer.GetComponent<CharacterClassManager>().UserId = $"DevDummyAmongus@server";
            hubPlayer.nicknameSync.Network_myNickSync = $"{Name}-BOT";
            hubPlayer.serverRoles.DoNotTrack = true;
            Log.Info("Set Network Info");
            hubPlayer.characterClassManager.GodMode = true;
            NewPlayer.RemoteAdminPermissions = PlayerPermissions.AFKImmunity;
            NewPlayer.Role.Set(Role);
            Log.Info("Gave permissions and role");
            NewPlayer.GameObject.transform.position = target.GameObject.transform.position;
            //NewPlayer.SessionVariables.Add("npc", true);
            Log.Info("Final");
            NetworkServer.Spawn(newPlayer);
            Log.Info($"Spawned {hubPlayer.nicknameSync.Network_myNickSync}");
        }
    }
}

