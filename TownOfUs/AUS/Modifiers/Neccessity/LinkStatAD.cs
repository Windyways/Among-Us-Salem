namespace AmongUsSalem.Modifiers;

public class LinkStatAD(int a, int d, int ed) : BaseModifier
{
    public override string ModifierName => "Link Attack/Defense";

    public Attack attack => (Attack)a;
    public Defense defense => (Defense)d;
    public EtherealDefense etherealDefense => (EtherealDefense)ed;
}