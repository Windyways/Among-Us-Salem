namespace ObjectWorkshop.Extensions;

public interface ITraitorIgnore
{
    // Used incase a role shouldn't be pickable by the Traitor
    // No need to use this if it's not an impostor role!
    bool IsIgnored { get; }
}