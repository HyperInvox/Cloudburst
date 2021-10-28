using UnityEngine;

namespace Cloudburst.Cores.States.Bombardier
{
    interface IFireRocket
    {
        void FireProjectile();
        GameObject GetProjectile();
    }
}