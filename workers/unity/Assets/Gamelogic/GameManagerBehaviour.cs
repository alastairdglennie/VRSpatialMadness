using Assets.EntityTemplates;
using Improbable.Entity.Component;
using Improbable.Server;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic
{
    class GameManagerBehaviour : MonoBehaviour
    {
        [Require]
        private GameManager.Writer GameManagerWriter;

        private void OnEnable()
        {
            GameManagerWriter.CommandReceiver.OnSpawnPlayer.RegisterResponse(HandleSpawnPlayer);
        }

        private SpawnPlayerResponse HandleSpawnPlayer(SpawnPlayerRequest request, ICommandCallerInfo callerinfo)
        {
            int newTeamId = GameManagerWriter.Data.currentTeamId + 1;
            GameManagerWriter.Send(new GameManager.Update().SetCurrentTeamId(newTeamId));

            SpatialOS.WorkerCommands.CreateEntity("Player", EntityTemplateFactory.Player(new Improbable.Math.Coordinates(Random.Range(-20, 15), 0, Random.Range(-20, 20)), callerinfo.CallerWorkerId, newTeamId), callback =>
            {
                if (callback.StatusCode != StatusCode.Success)
                {
                    Debug.LogError("Spawning player failed on fsim " + callback.ErrorMessage);
                }
                else
                {
                    Debug.LogError("SUCCESSFULLY SPAWNED PLAYER FOR " + callerinfo.CallerWorkerId);
                }
            });
            return new SpawnPlayerResponse(callerinfo.CallerWorkerId);
        }

        private void OnDisable()
        {
            GameManagerWriter.CommandReceiver.OnSpawnPlayer.DeregisterResponse();
        }
    }
}
