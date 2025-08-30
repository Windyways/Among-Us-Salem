using System.Collections;
using UnityEngine;
using Color = UnityEngine.Color;

namespace ObjectWorkshop.LifeImprovement.Objects;

[RegisterInIl2Cpp]
public class Duel(IntPtr ptr) : MonoBehaviour(ptr)
{
    public GameObject Sword1;
    public GameObject Sword2;
    public PlayerControl player;
    public PlayerControl target;
    public int WinChance;

    public void Start()
    {
        PlayersInDuel.Add(player.PlayerId);
        PlayersInDuel.Add(target.PlayerId);

        Sword1.transform.SetParent(player.transform);
        Sword1.transform.localPosition = new Vector3(SwordLeftRight(1), 0.1f, 0f);

        Sword2.transform.SetParent(target.transform);
        Sword2.transform.localPosition = new Vector3(-SwordLeftRight(2), 0.1f, 0f);

        TouAudio.PlaySound(OWAssets.DuelBegin_SFX);
        Coroutines.Start(DuelSequence(player, target, WinChance));
    }

    public float SwordLeftRight(int sword)
    {
        bool left = player.transform.position.x < target.transform.position.x;

        if (left && sword == 1)
        {
            Sword1.GetComponent<SpriteRenderer>().flipX = true;
        }

        return left ? 0.5f : -0.5f;
    }

    public IEnumerator DuelSequence(PlayerControl player, PlayerControl target, float winChance)
    {
        player.Immobilize();
        target.Immobilize();

        yield return new WaitForSeconds(1f);

        // Example sequence: dash together, show effects, decide winner
        yield return DashTowardsEachOther(player, target, 0.5f);

        CreateSlashArcBetweenPlayers(player.transform.position, target.transform.position);

        yield return new WaitForSeconds(0.2f);

        CreateSparkLines((player.transform.position + target.transform.position) / 2);

        TouAudio.PlaySound(OWAssets.DuelKill_SFX);
        if (UnityEngine.Random.Range(0, 100) <= winChance && (player.AmOwner || Debugger.IsDebuggerActive))
        {
            player.RpcCustomMurder(target); // Duelist wins
        }
        else if (target.AmOwner || Debugger.IsDebuggerActive)
        {
            target.RpcCustomMurder(player); // Target wins
        }

        StopDuel(player, target, false);
    }

    public IEnumerator DashTowardsEachOther(PlayerControl p1, PlayerControl p2, float duration)
    {
        Vector3 startPos1 = p1.transform.position;
        Vector3 startPos2 = p2.transform.position;
        Vector3 midPoint = (startPos1 + startPos2) / 2;

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            p1.transform.position = Vector3.Lerp(startPos1, midPoint, t);
            p2.transform.position = Vector3.Lerp(startPos2, midPoint, t);
            time += Time.deltaTime;
            yield return null;
        }

        p1.transform.position = midPoint;
        p2.transform.position = midPoint;
    }

    public void StopDuel(PlayerControl player, PlayerControl target, bool canceled)
    {
        PlayersInDuel.Remove(player.PlayerId);
        PlayersInDuel.Remove(target.PlayerId);

        player.Mobilize();
        target.Mobilize();

        if (player.IsRole<Duelist>())
        {
            var duelist = player.GetRole<Duelist>();

            if (!canceled)
            {
                if (player.AmOwner || Debugger.IsDebuggerActive)
                {
                    var button = CustomButtonSingleton<Duelist_Duel>.Instance;
                    button.ResetCooldownAndOrEffect();
                }

                Duelist.RpcResetWinChance(duelist.Player);
            }
        }
        /*else if (player.Is(RoleEnum.Claylamity))
        {
            var claylamity = Role.GetRole<Claylamity>(player);
            CleanupSwords();
            player.StopPlayer(false);
            target.StopPlayer(false);

            if (!canceled)
            {
                Utils.ResetCooldowns(player, target, 2, false, false);
                claylamity.WinChance = Duelist.InitialChance;
                claylamity.DuelingPlayers.Clear();
                Utils.Rpc(CustomRPC.ExtraRPCs, "Reset Win Chance", player.PlayerId);
            }
        }*/
        
        CleanupDuel();
    }

    private void CleanupDuel()
    {
        if (Sword1 != null) Destroy(Sword1);
        if (Sword2 != null) Destroy(Sword2);

        Destroy(gameObject);
    }

    public void CreateSlashArcBetweenPlayers(Vector2 origin, Vector2 target, float length = 2f, float arcDegrees = 100f, float duration = 0.4f)
    {
        GameObject slashGO = new GameObject("SlashArc");
        LineRenderer line = slashGO.AddComponent<LineRenderer>();

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.widthMultiplier = 0.2f;
        line.positionCount = 20;
        line.sortingOrder = 150;
        line.useWorldSpace = true;
        line.startColor = new Color(0.6f, 0.9f, 1f, 1f);
        line.endColor = new Color(0.6f, 0.9f, 1f, 0f);

        Vector2 dir = (target - origin).normalized;
        float baseAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float halfArc = arcDegrees / 2f;
        float angleStart = baseAngle - halfArc;
        float angleStep = arcDegrees / (line.positionCount - 1);

        for (int i = 0; i < line.positionCount; i++)
        {
            float angle = (angleStart + i * angleStep) * Mathf.Deg2Rad;
            Vector2 pos = origin + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * length;
            line.SetPosition(i, pos);
        }

        Coroutines.Start(FadeAndDestroyLine(slashGO, duration));
    }

    public IEnumerator FadeAndDestroyLine(GameObject lineGO, float duration)
    {
        var lr = lineGO.GetComponent<LineRenderer>();
        float time = 0f;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            Color c = lr.startColor;
            lr.startColor = new Color(c.r, c.g, c.b, alpha);
            lr.endColor = new Color(c.r, c.g, c.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(lineGO);
    }

    public void CreateSparkLines(Vector2 position, int sparkCount = 10, float length = 0.5f, float duration = 1f)
    {
        GameObject container = new GameObject("SparkLines");
        container.transform.position = position;

        for (int i = 0; i < sparkCount; i++)
        {
            GameObject spark = new GameObject("SparkLine");
            spark.transform.SetParent(container.transform);

            LineRenderer line = spark.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startWidth = 0.03f;
            line.endWidth = 0.01f;
            line.positionCount = 2;
            line.startColor = Color.cyan;
            line.endColor = Color.white;

            Vector2 direction = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 start = position;
            Vector3 end = position + direction * length;

            line.SetPosition(0, start);
            line.SetPosition(1, end);

            Coroutines.Start(FadeAndDestroySparkLine(line, duration));
        }

        Destroy(container, duration + 0.1f);
    }

    private IEnumerator FadeAndDestroySparkLine(LineRenderer line, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            Color startColor = line.startColor;
            Color endColor = line.endColor;
            line.startColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            line.endColor = new Color(endColor.r, endColor.g, endColor.b, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        Destroy(line.gameObject);
    }

    private void Update()
    {
        UpdateColor();
    }

    public void UpdateColor()
    {
        SpriteRenderer sr = Sword1.GetComponent<SpriteRenderer>();
        SpriteRenderer sr2 = Sword2.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = new Color(1f, 1f, 1f, 1f);
        if (sr2 != null) sr2.color = new Color(1f, 1f, 1f, 1f);
        /*if (PlayerControl.LocalPlayer.IsUnderground())
        {
            sr.color = new Color(1f, 1f, 1f, 0f);
        }
        else
        {
            sr.color = new Color(1f, 1f, 1f, 1f);
        }*/
    }

    #region Global
    #endregion
    public static void Begin(PlayerControl player, PlayerControl target, int winChance)
    {
        GameObject obj = new GameObject("DuelController");

        Duel duel = obj.AddComponent<Duel>();
        duel.player = player;
        duel.target = target;
        duel.WinChance = winChance;
        duel.Sword1 = CreateSword();
        duel.Sword2 = CreateSword();
    }

    public static GameObject CreateSword()
    {
        GameObject sword = new GameObject("DuelSword");
        SpriteRenderer sr = sword.AddComponent<SpriteRenderer>();
        sr.sprite = OWAssets.SwordSprite.LoadAsset();
        sr.sortingOrder = 0;
        sr.rendererPriority = 100;
        sr.flipY = true;
        return sword;
    }

    public static List<byte> PlayersInDuel = new List<byte>();

    public static void CleanUp()
    {
        PlayersInDuel.Clear();
    }
}
