using Object = UnityEngine.Object;
using UnityEngine;
using System.Collections;

// Special thanks to Pix for this code!
// And also for the basis to make this mod compatible with more than one role at a time! [yeah Pyre i'm looking at you]
[RegisterInIl2Cpp(typeof(IUsable))]
public class Crate(IntPtr ptr) : MonoBehaviour(ptr)
{
    public float UsableDistance => 90f;
    public float PercentCool => 1f;
    public ImageNames UseIcon => ImageNames.FreeplayButton;
    
    public SpriteRenderer myRend;
    public byte id;
    public PlayerControl Owner;

    public void Start()
    {
        // Lockdown.AvailableObjects.Add(gameObject);

        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.Bounce(gameObject.transform, 0.7f, 0.45f));
        DestroyableSingleton<HudManager>.Instance.StartCoroutine(Effects.ColorFade(myRend, new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 0.4f));
        
        myRend = gameObject.GetComponent<SpriteRenderer>();
        myRend.SetMaterial(ShipStatus.Instance.EmergencyButton.GetComponent<SpriteRenderer>().material);
        id = GetAvailableId();
        AllCrates.Add(this);
    }

    private void OnDestoy()
    {
        AllCrates.Remove(this);
    }

    public void Use()
    {
        GiftWeaver.RpcOpenCrate(Owner, PlayerControl.LocalPlayer);
    }

    public void SetOutline(bool on, bool mainTarget)
    {
        if (on) myRend.SetOutline(new Color?(Color.yellow));
        else if (!GameObjectExtensions.IsDestroyedOrNull(this)) myRend.SetOutline(new Color?(Color.white));
    }

    public float CanUse(NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        float num = float.MaxValue;
        PlayerControl @object = pc.Object;
        couldUse = !pc.IsDead;
        canUse = couldUse;
        
        if (canUse)
        {
            Vector2 truePosition = @object.GetTruePosition();
            Vector3 position = base.transform.position;
            num = Vector2.Distance(truePosition, position);
            canUse &= (num <= UsableDistance);
        }

        return num;
    }
    
    public static List<Crate> AllCrates = new List<Crate>();
    public static void Begin(PlayerControl player)
    {
        GameObject obj = new GameObject("BrownCrate");
        obj.AddSpriteRenderer(OWAssets.CrateSprite.LoadAsset(), 0, 100, player.transform.position, ObjectExtentions.fullColor(), Vector3.one);
        obj.AddBoxCollider2D(false, true);
        obj.AddRigidBody2D(0f, RigidbodyConstraints2D.FreezeAll);

        Crate crate = obj.AddComponent<Crate>();
        crate.Owner = player;
    }

    public static Crate GetObjectByOwner(PlayerControl player)
    {
        foreach (var crate in AllCrates)
        {
            if (crate.Owner == player) return crate;
        }
        return null;
    }

    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllCrates.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static void CleanUp()
    {
        AllCrates.Clear();
    }
}