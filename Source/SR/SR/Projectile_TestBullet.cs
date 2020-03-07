using RimWorld;
using Verse;
namespace SR
{
    public class Projectile_TestBullet : Bullet
    {
        #region data
        public ThingDef_TestBullet ThingDef_TestBullet
        {
            get
            {
                //底层通过名字读取了我们定义的ThingDef_TestBullet这个xml格式的新数据，并存放到了this.def中，我们将this.def拆箱拿到我们定义好的ThingDef_TestBullet格式数据
                return this.def as ThingDef_TestBullet;
            }
        }
        #endregion
        protected override void Impact(Thing hitThing)
        {
            //子弹的影响，底层实现了伤害 击杀之类的方法，感兴趣的话可以用dnspy反编译Assembly-Csharp.dll研究里面到底写了什么
            base.Impact(hitThing);
            //绝大多数mod报错都是因为没判断好非空，写注释和判断非空是好习惯
            //大佬在这里用了一个语法糖hitThing is Pawn hitPawn
            //如果hitThing可以被拆箱为Pawn的话 这个值返回true并且会声明一个变量hitPawn=hitThing as Pawn
            //否则返回false hitPawn是null
            if (ThingDef_TestBullet != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                var rand = Rand.Value; //这个方法封装了一个返回0%-100%随机数的函数
                //触发瘟疫
                if (rand <= ThingDef_TestBullet.addHediffChance)
                {
                    //在屏幕左上角显示提示,translate方法用于翻译不同语言之后再说，MessageTypeDefOf要设置一种事件
                    Messages.Message("SR_Message_TestBullet_Success".Translate(this.launcher.Label,hitPawn.Label),MessageTypeDefOf.NeutralEvent);
                    //判断一下目标是否已经触发了瘟疫效果
                    var plagueOnPawn = hitPawn.health?.hediffSet?.GetFirstHediffOfDef(ThingDef_TestBullet.hediffToAdd);
                    //我们为本次触发的瘟疫随机生成一个严重程度
                    var randomSeverity = Rand.Range(0.15f, 0.30f);
                    //已经触发瘟疫
                    if (plagueOnPawn != null)
                    {
                        //严重程度叠加，超过100%会即死
                        plagueOnPawn.Severity += randomSeverity;
                    }
                    else
                    {
                        //我们调用HediffMaker.MakeHediff生成一个新的hediff状态，类型就是我们之前设置过的HediffDefOf.Plague瘟疫类型
                        Hediff hediff = HediffMaker.MakeHediff(ThingDef_TestBullet.hediffToAdd, hitPawn);
                        //设置这个状态的严重程度
                        hediff.Severity = randomSeverity;
                        //把状态添加到被击中的目标身上
                        hitPawn.health.AddHediff(hediff);
                    }
                }
                //本次没有触发
                else
                {
                    //这个方法可以在某个位置(这里是被击中目标的身旁)弹出一小行字，比如未击中，击中头部之类的，也是可以
                    MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "SR_Mote_TestBullet_Fail".Translate(hitPawn.Label), 12f);
                }
            }
        }
    }
}
