using System.Collections;
using UnityEngine;
using Color = UnityEngine.Color;
using Object = UnityEngine.Object;

namespace ObjectWorkshop.LifeImprovement.Objects;

[RegisterInIl2Cpp]
public class ReaperVisual(IntPtr ptr) : MonoBehaviour(ptr)
{
    public PlayerControl Owner;
    public PlayerControl ReaperOwner;
    public byte id;
    private Vector3 basePos;

    public void Start()
    {
        id = GetAvailableId();
        AllUndeadReapers.Add(this);

        Owner.SetTransparency(0, true);
        basePos = transform.localPosition;
    }

    private void Update()
    {
        if (!UndeadReaper.ReapersAlive())
        {
            // For Peacock!
            foreach (var role in GameHistory.AllRoles)
            {
                if (!role || role is not IOWRole touRole)
                {
                    continue;
                }

                touRole.OnTargetDeath(Owner, DeathReason.Kill);
            }

            CleanUp();
            Owner.SetTransparency(1, false);

            Destroy(gameObject);
        }
        else 
        {
            ChangeDirection();
            UpdateColor();
            IdleFloat();
        }
    }

    public void ChangeDirection()
    {
        UndeadReaper.RpcFlipReaperVisual(Owner);
    }

    public void UpdateColor()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        float alpha = 0.5f + Mathf.Sin(Time.time * 2f) * 0.25f;
        sr.color = new Color(1f, 1f, 1f, alpha); // pulsing transparency
        
        /*if (PlayerControl.LocalPlayer.IsUnderground())
        {
            sr.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            sr.color = new Color(1f, 1f, 1f, 1f);
        }*/
    }

    public void IdleFloat()
    {
        float offset = Mathf.Sin(Time.time * 2f) * 0.05f;
        transform.localPosition = basePos + new Vector3(0, offset, 0);
    }

    #region Global
    #endregion
    public static void Begin(PlayerControl reaper, PlayerControl player)
    {
        var obj = new GameObject("ReaperVisual");
        obj.transform.SetParent(player.transform);
        obj.AddSpriteRenderer(OWAssets.ReaperSprite.LoadAsset(), 100, 100, player.transform.position, ObjectExtentions.fullColor(), Vector3.one);

        var undeadreaper = obj.AddComponent<ReaperVisual>();
        undeadreaper.Owner = player;
        undeadreaper.ReaperOwner = reaper;
    }

    public static void DestroyAll()
    {
        foreach (var undeadreaper in AllUndeadReapers)
        {
            Destroy(undeadreaper.gameObject);
        }
        AllUndeadReapers.Clear();
    }

    public static void CleanUp()
    {
        AllUndeadReapers.Clear();
    }

    public static List<ReaperVisual> AllUndeadReapers = new List<ReaperVisual>();
    public static byte GetAvailableId()
    {
        byte id = 0;
        while (AllUndeadReapers.Any(x => x.id == id))
        {
            id++;
        }
        return id;
    }

    public static ReaperVisual GetObjectByOwner(PlayerControl player)
    {
        foreach (var undeadReaper in AllUndeadReapers)
        {
            if (undeadReaper.Owner == player) return undeadReaper;
        }
        return null;
    }
}