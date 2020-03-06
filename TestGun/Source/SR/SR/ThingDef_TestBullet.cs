using RimWorld;
using Verse;

namespace SR
{
    public class ThingDef_TestBullet:ThingDef
    {
        public float addHediffChance; //默认值会被xml覆盖
        public HediffDef hediffToAdd;
    }
}