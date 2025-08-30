using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

[RegisterInIl2Cpp]
public class Sanctuary(IntPtr ptr) : MonoBehaviour(ptr)
{
    public byte id;
    public PlayerControl Owner;
    public SpriteRenderer myRend;

    public void Start()
    {
        myRend = gameObject.GetComponent<SpriteRenderer>();
        id = GetAvailableId();

        AllSanctuarys.Add(this);
        Destroy(gameObject, OptionGroupSingleton<Oasis_Options>.Instance.Duration);
    }

    private void Update()
    {
        UpdateColor();
    }

    private void OnDestroy()
    {
        AllSanctuarys.Remove(this);
    }

    public void UpdateColor()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();

        if (Owner == PlayerControl.LocalPlayer) sr.color = new Color(1f, 1f, 0f, 0.3f);
        else sr.color = ObjectExtentions.noColor(); // Completely invisible
    }

    public bool IsInRange(PlayerControl target)
    {
        if (target == null || gameObject == null) return false;
        if (target.HasDied() || target.inVent/* || target.IsUnderground() || target.IsSpider() || target.IsPeacock()*/) return false;
        
        float dist = Vector2.Distance(gameObject.transform.position, target.transform.position);
        return dist <= OptionGroupSingleton<Oasis_Options>.Instance.Radius;
    }

    public static void Begin(PlayerControl player)
    {
        GameObject gameObject = new GameObject("Sanctuary");
        gameObject.AddSpriteRenderer(OWAssets.Bubble.LoadAsset(), 30, 100, player.transform.position, ObjectExtentions.One3rdColor(), Vector3.one * OptionGroupSingleton<Oasis_Options>.Instance.Radius);

        Sanctuary sanctuary = gameObject.AddComponent<Sanctuary>();
        sanctuary.Owner = player;
    }

    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllSanctuarys.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static void DestroyAll()
    {
        foreach (Sanctuary sanctuarys in AllSanctuarys)
        {
            Destroy(sanctuarys.gameObject);
        }
        AllSanctuarys.Clear();
    }

    public static Sanctuary GetObjectByPlayer(PlayerControl player)
    {
        foreach (Sanctuary sanctuary in AllSanctuarys)
        {
            if (sanctuary.Owner == player) return sanctuary;
        }
        return null;
    }

    public static void CleanUp()
    {
        AllSanctuarys.Clear();
    }
    public static List<Sanctuary> AllSanctuarys = new List<Sanctuary>();
}
