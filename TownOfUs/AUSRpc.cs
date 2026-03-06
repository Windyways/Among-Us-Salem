namespace TownOfUs;

public enum AUSRpc : uint
{
    // Roles
    Mayor_Reveal,
    Deputy_HighNoon,
    Prosecutor_Prosecute,
    RpcResetMafiosoCooldown,
    RpcResetGodfatherCooldown,
    RpcDaybreak,
    RpcPerformInteraction,
    RpcPerformDoubleInteraction,
    RpcTryStartProtest,
    RpcEmpowerGrimoire,
    RpcPerformInteractionTracked,
    RpcQuotaLynch,

    // Notifications
    RpcNotifyBodyguard,
    RpcNotifyJinx,
    RpcNotifyPotionMaster,
    RpcNotifySurvivor,
    RpcNotifyCrusader,
    RpcNotifyWar,
    RpcNotifyStarspawn,
    RpcNotifyPestilence,
    RpcNotifyPlaguebearer,
    RpcNotifyLeaveTown,
    RpcNotifyWarlock,
    RpcNotifyDeath,
    RpcTMDNotify,

    // Mechanics
    StartDayOne,
    RpcApplyAttack,
    RpcApplyDefense,

    // Other
    RpcNotifyCoven,
    NecroPassing_PassNecronomicon,
    NecroPassing_AssignNecronomicon,
    RpcAddDeathReason,

    // Misc
    RequestDeathStateValidation,
    SyncDeathState,
    GhostRoleMurder,











    RemoveSpawns,

    UpdateDeathHandler,
    SetMap,
    ChangeRole,
    PlayerExile,
    SetPos,
    SendLoveChat,
    SendJailorChat,
    SendJaileeChat,
    SendImpTeamChat,
    SendVampTeamChat,
    UpdateCelebrityKilled,
    ClericBarrierAttacked,
    Transport,
    SetSwaps,
    CleanBody,
    CatchPlayer,
    MagicMirror,
    ClearMagicMirror,
    MagicMirrorAttacked,
    MirrorcasterUnleash,
    MedicShield,
    ClearMedicShield,
    MedicShieldAttacked,
    EngineerFix,
    EngineerEventFix,
    IgniteSound,
    PlaceVent,
    ShowVent,
    Remember,
    PlantBomb,
    Blackmail,
    Recall,
    MarkLocation,
    Disperse,
    Mediate,
    VampireBite,
    CheckInfected,
    SetGATarget,
    SetOtherLover,
    SetTraitor,
    DragBody,
    DropBody,
    AltruistRevive,
    Prosecute,
    DoomsayerWin,
    SetExeTarget,
    AddInquisTarget,
    Hysteria,
    WardenFortify,
    ClearWardenFortify,
    WardenNotify,
    OracleBlessNotify,
    CatchGhost,
    Guarded,
    PlumberFlush,
    PlumberBlockVent,
    OracleConfess,
    OracleBless,
    InquisitorWin,
    TriggerSixthSense,
    AurialSense,
    ButtonBarry,
    LookoutSeePlayer,
    AnimateNewReveal,
    SheriffMisfire,
    RetrainImpostor,
    AmbushPlayer,
    RetrainConfirm
}