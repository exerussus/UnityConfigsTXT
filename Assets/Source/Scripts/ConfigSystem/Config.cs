using UnityEngine;

namespace Source.Scripts.ConfigSystem
{
    public class Config : ConfigsAbstraction
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float range;
        [SerializeField] private float waitTime;

        public float MoveSpeed => moveSpeed;
        public float Range => range;
        public float WaitTime => waitTime;
        
        private const string nameMoveSpeed = "moveSpeed";
        private const string nameRange = "range";
        private const string nameWaitTimer = "waitTimer";

        protected override void InitFields()
        {
            Debug.Log($"waitTime = {waitTime}");
            moveSpeed = GetValueOrSetDefault(nameMoveSpeed, moveSpeed);
            range = GetValueOrSetDefault(nameRange, range);
            waitTime = GetValueOrSetDefault(nameWaitTimer, waitTime);
        }

        protected override void UpdateFields()
        {
            moveSpeed = GetValue(nameMoveSpeed);
            range = GetValue(nameRange);
            waitTime = GetValue(nameWaitTimer);
            Debug.Log("fields updated");
        }
    }
}