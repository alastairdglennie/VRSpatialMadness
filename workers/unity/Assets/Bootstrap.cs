using Improbable;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Worker;
using System;
using Improbable.Server;
using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Configuration;

// Placed on a gameobject in client scene to execute connection logic on client startup
public class Bootstrap : MonoBehaviour
{
    public WorkerConfigurationData Configuration = new WorkerConfigurationData();

    int TargetClientFramerate = 60;
    int TargetServerFramerate = 60;
    int FixedFramerate = 20;

    public static string WorkerId;

    public void Start()
    {
        SpatialOS.ApplyConfiguration(Configuration);

        Time.fixedDeltaTime = 1.0f / FixedFramerate;

        switch (SpatialOS.Configuration.WorkerPlatform)
        {
            case WorkerPlatform.UnityWorker:
                Application.targetFrameRate = TargetServerFramerate;
                SpatialOS.OnDisconnected += reason => Application.Quit();
                break;
            case WorkerPlatform.UnityClient:
                Application.targetFrameRate = TargetClientFramerate;
                SpatialOS.OnConnected += CreatePlayer;
                break;
        }

        SpatialOS.Connect(gameObject);
    }

    public static void CreatePlayer()
    {
        FindPlayerCreatorEntity(RequestPlayerCreation);
    }

    private static void FindPlayerCreatorEntity(Action<EntityId> createRequestCallback)
    {
        var playerCreatorQuery = Query.HasComponent<GameManager>().ReturnOnlyEntityIds();
        SpatialOS.WorkerCommands.SendQuery(playerCreatorQuery, result => OnQueryResult(createRequestCallback, result));
    }

    private static void OnQueryResult(Action<EntityId> requestPlayerCreationCallback, ICommandCallbackResponse<EntityQueryResult> queryResult)
    {
        if (!queryResult.Response.HasValue || queryResult.StatusCode != StatusCode.Success)
        {
            Debug.LogError(queryResult.ErrorMessage);
            Debug.LogError("PlayerCreator query failed. SpatialOS workers probably haven't started yet. Try again in a few seconds.");
            return;
        }

        var queriedEntities = queryResult.Response.Value;
        if (queriedEntities.EntityCount < 1)
        {
            Debug.LogError("Failed to find PlayerCreator. SpatialOS probably hadn't finished creating the initial snapshot. Try again in a few seconds.");
            return;
        }

        var playerCreatorEntityId = queriedEntities.Entities.First.Value.Key;
        requestPlayerCreationCallback(playerCreatorEntityId);
    }

    private static void RequestPlayerCreation(EntityId playerCreatorEntityId)
    {
        Debug.LogWarning("Sending RequestPlayerCreation");
        SpatialOS.WorkerCommands.SendCommand(GameManager.Commands.SpawnPlayer.Descriptor, new SpawnPlayerRequest(), playerCreatorEntityId)
            .OnSuccess(result => { WorkerId = result.workerid; })
            .OnFailure(result => OnCreatePlayerFailure(result, playerCreatorEntityId));
    }

    private static void OnCreatePlayerFailure(ICommandErrorDetails error, EntityId playerCreatorEntityId)
    {
        Debug.LogWarning("CreatePlayer command failed - you probably tried to connect too soon. Try again in a few seconds.");
        Debug.LogWarning(error.ErrorMessage);
    }
}