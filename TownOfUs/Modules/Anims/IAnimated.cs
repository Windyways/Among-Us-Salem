namespace TownOfUs.Modules.Anims;

public interface IAnimated
{
    public bool IsVisible { get; set; }
    public void SetVisible();
}