using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MagicPigGames.ProjectileFactory
{
    [CreateAssetMenu(fileName = "New Raycast Hit Detector",
        menuName = "Projectile Factory/Generic Behavior/Raycast Hit Detector")]
    public class ProjectileRaycastHitDetector : ProjectileBehavior
    {
        [Header("Raycast Settings")]
        [Tooltip(
            "The length of the raycast. This is multiplied by the distance the projectile has traveled since the last frame.")]
        [ShowInProjectileEditor("Raycast Length Factor")]
        public float raycastLengthFactor = 1.0f;

        [Tooltip("Set true for moving projectiles. False for static projectiles, generally attached somewhere.")]
        [ShowInProjectileEditor("Length Based On Distance")]
        public bool lengthBasedOnDistance = true;

        [Tooltip(
            "The maximum number of hits the raycast can have, starting with the closest to the Projectile Spawner. -1 for infinite.")]
        [ShowInProjectileEditor("Max Hits")]
        public int maxHits = 1;

        [Tooltip("When true, the raycast will use a sphere cast instead of a raycast.")]
        [ShowInProjectileEditor("Use Sphere Cast")]
        public bool useSphereCast;

        [Tooltip("The radius of the sphere cast.")] [ShowInProjectileEditor("Sphere Cast Radius")]
        public float radius = 1f;

        [FormerlySerializedAs("disableOnHit")]
        [Header("Options")]
        [Tooltip("When true, the ray will stop being cast after we hit. This is re-enabled OnEnable.")]
        [ShowInProjectileEditor("Disable Ray On Hit")]
        public bool disableRayOnHit = true;

        [Header("Debug Options")] public bool drawDebugLine;

        protected bool _initialized;

        protected Vector3 _lastPosition;

        protected bool _rayDisabled;

        // Internal description is used for the custom inspector to show a description of the behavior.
        public override string InternalDescription =>
            "Raycasts forward each frame looking for a collision. If a collision " +
            "is found, the projectile will trigger a collision with the object.";

        // Internal icon is used for the custom inspector to show an icon of the behavior.
        public override string InternalIcon => "Raycast";

        protected Vector3 Position => Projectile.transform.position;
        protected Vector3 Forward => Projectile.transform.forward;

        // Re-enable the ray when the projectile is enabled
        protected virtual void OnEnable()
        {
            _initialized = false;
            _rayDisabled = false;
        }

        // Start is called before the first frame update
        protected virtual void CacheLastPosition()
        {
            _lastPosition = Position;
        }


        public override void OnReturnToPool(Projectile projectile)
        {
        }

        public override void OnGetFromPool(Projectile projectile)
        {
        }

        public override void Tick()
        {
            if (_rayDisabled) return;

            var distance = Position.DistanceFrom(_lastPosition); // Get the distance delta
            CacheLastPosition(); // Cache the new position

            // If we haven't been initiatlized (first frame), then just return
            if (!_initialized)
            {
                _initialized = true;
                return;
            }

            CheckRaycastHit(distance);
        }

        protected virtual void CheckRaycastHit(float distance)
        {
            if (lengthBasedOnDistance && distance == 0) return;

            var raycastLength = lengthBasedOnDistance ? distance * raycastLengthFactor : raycastLengthFactor;

            if (drawDebugLine)
                Debug.DrawRay(Position, Forward * raycastLength, Color.cyan, 5f);

            // Get all hits forward
            var hits = useSphereCast
                ? Physics.SphereCastAll(Position, radius, Forward, raycastLength, Projectile.CollisionMask)
                : Physics.RaycastAll(Position, Forward, raycastLength, Projectile.CollisionMask);

            Array.Sort(hits, (h1, h2) => h1.distance.CompareTo(h2.distance));

            // If we have a max hits, then limit the hits to that number
            if (maxHits > 0 && hits.Length > maxHits)
                Array.Resize(ref hits, maxHits);

            // Handle each hit
            foreach (var hit in hits)
                HandleRaycastHit(hit);
        }

        protected virtual void HandleRaycastHit(RaycastHit hitInfo)
        {
            Projectile.TriggerCollisionWithObject(hitInfo.collider.gameObject,
                hitInfo.point); // Force a collision to the projectile.
            _rayDisabled = disableRayOnHit; // Disable ray, or keep it active
        }
    }
}