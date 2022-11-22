using System;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class EnemyAttackData
    {
        public int Anim;
        public EnemyAttackTypes AttackTypes;
        public MoveAttackData[] MoveAttackData;

        public EnemyAttackData(int anim, EnemyAttackTypes attackTypes, MoveAttackData[] moveAttackData)
        {
            Anim = anim;
            AttackTypes = attackTypes;
            MoveAttackData = moveAttackData;
        }
        
        public EnemyAttackData(int anim, EnemyAttackTypes attackTypes)
        {
            Anim = anim;
            AttackTypes = attackTypes;
        }
    }
}