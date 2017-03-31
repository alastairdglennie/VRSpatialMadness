using Improbable.General;
using Improbable.Math;
using Improbable.Player;
using Improbable.Server;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;

namespace Assets.EntityTemplates
{
    public class EntityTemplateFactory
    {
        public static SnapshotEntity Player(Coordinates position, string workerid, int teamId)
        {
            var entity = new SnapshotEntity { Prefab = "Player" };

            entity.Add(new Position.Data(position));
            
            entity.Add(new Colour.Data(new Vector3f(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value)));
            entity.Add(new VivePlayer.Data(new ViveTransform(), new ViveTransform(), new ViveTransform()));
            entity.Add(new PlayerInfo.Data(10, 0));
            entity.Add(new Team.Data(teamId));

            var acl = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<Position>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<VivePlayer>(CommonRequirementSets.SpecificClientOnly(workerid))
                .SetWriteAccess<Colour>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<PlayerInfo>(CommonRequirementSets.SpecificClientOnly(workerid))
                .SetWriteAccess<Team>(CommonRequirementSets.SpecificClientOnly(workerid));

            entity.SetAcl(acl);

            return entity;
        }

        public static SnapshotEntity Ball(Coordinates position, Vector3f velocity, Vector3f colour, string workerid, int teamId)
        {
            var entity = new SnapshotEntity { Prefab = "Ball" };

            entity.Add(new Position.Data(position));
            entity.Add(new Velocity.Data(velocity));
            entity.Add(new Colour.Data(colour));
            entity.Add(new Team.Data(teamId));

            var acl = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<Position>(CommonRequirementSets.SpecificClientOnly(workerid))
                .SetWriteAccess<Velocity>(CommonRequirementSets.SpecificClientOnly(workerid))
                .SetWriteAccess<Colour>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Team>(CommonRequirementSets.SpecificClientOnly(workerid));

            entity.SetAcl(acl);

            return entity;
        }

        public static SnapshotEntity FloorTile(Coordinates position)
        {
            var entity = new SnapshotEntity { Prefab = "FloorTile" };

            entity.Add(new Position.Data(position));
            entity.Add(new Colour.Data(new Vector3f(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value)));
            entity.Add(new Team.Data(-1));

            var acl = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOrVisual)
                .SetWriteAccess<Position>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Colour>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Team>(CommonRequirementSets.PhysicsOnly);

            entity.SetAcl(acl);

            return entity;
        }

        public static SnapshotEntity GameManager()
        {
            var entity = new SnapshotEntity { Prefab = "GameManager" };

            entity.Add(new Position.Data(new Coordinates(0, 0, 50)));
            entity.Add(new GameManager.Data(0));

            var acl = Acl.Build()
                .SetReadAccess(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<Position>(CommonRequirementSets.PhysicsOnly)
                .SetWriteAccess<GameManager>(CommonRequirementSets.PhysicsOnly);

            entity.SetAcl(acl);

            return entity;
        }
    }
}
