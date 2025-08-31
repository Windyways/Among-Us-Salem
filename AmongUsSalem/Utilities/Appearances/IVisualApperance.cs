namespace AmongUsSalem.Utilities.Appearances;

public interface IVisualAppearance
{
    bool VisualPriority => false;
    VisualAppearance? GetVisualAppearance();
}