using UnityEngine;

namespace Source.Scripts.ConfigSystem
{
    public class Config : ConfigsAbstraction
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float range;
        [SerializeField] private float waitTime;
        [SerializeField] private float delay;

        public float MoveSpeed => moveSpeed;
        public float Range => range;
        public float WaitTime => waitTime;
        
        private const string nameMoveSpeed = "moveSpeed";
        private const string nameRange = "range";
        private const string nameWaitTime = "waitTime";
        private const string nameDelay = "delay";
        
        private const string descriptionMoveSpeed = "Скорость персонажа";
        private const string descriptionRange = "Радиус атаки";
        private const string descriptionWaitTime = "Ожидание после атаки";
        private const string descriptionDelay = "Мы что-то ожидаем";

        protected override void InitFields()
        {
            moveSpeed = GetValueOrSetDefault(nameMoveSpeed, descriptionMoveSpeed, moveSpeed);
            range = GetValueOrSetDefault(nameRange, descriptionRange, range);
            waitTime = GetValueOrSetDefault(nameWaitTime, descriptionWaitTime, waitTime);
            delay = GetValueOrSetDefault(nameDelay, descriptionDelay, delay);
        }
    }
}