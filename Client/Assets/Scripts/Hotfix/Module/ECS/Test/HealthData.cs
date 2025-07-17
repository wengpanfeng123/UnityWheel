using ECS;

namespace Hotfix.ECS.Test
{
    public class HealthData : BaseDataComponent
    {
        public float currentHealth = 100f;
        public float maxHealth = 100f;
        public bool isInvulnerable = false;
        public bool isDead = false;
    }
}