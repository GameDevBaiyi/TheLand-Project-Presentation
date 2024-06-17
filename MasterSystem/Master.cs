using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(TraitTouchable))]
public class Master : CharacterBase
{
    public bool LookAtPlayer;
    public bool StepAwayFromPlayer;

    public MasterInfo Data { get; set; }
    
    PlayerCharacter playerCharacter;
    
    GameObject widgetObj;

    public void SetData(MasterInfo data)
    {
        this.Data = data;
    }
    
    public MasterInfo GetData()
    {
        return this.Data;
    }

    protected override void Awake()
    {
        base.Awake();
        
        InitBones("human", SuPro.Blueprint.Gender.Both);
        InitAnimation("human");

        WalkSpeed = 1.2f;
        RunSpeed = 5.5f;
    }

    protected override void Start()
    {
        base.Start();

        playerCharacter = AvatarManager.Instance.GetPlayerCharacter();
    }

    public override void OnFreeResource()
    {
        base.OnFreeResource();
        
        if (widgetObj != null)
        {
            GameObject.Destroy(widgetObj);
        }
    }

    public override void ResetMove()
    {
        if (onRiding) return;
        
        StandInPlace();
        
        followTarget = null;
    }
    
    void CheckForFollow(PhysicalObject target)
    {
        const float distanceToPlayer = 1.8f;
        const float distanceToSee = 10;
        const float distanceToStepAside = 1f;

        if (GetEmotionState() != E_EMOTION_STATE.DEFAULT)
        {
            return;
        }

        var state = GetCurrentCharacterState();

        switch (state)
        {
            case E_CHARACTER_STATE.IDLE:
            case E_CHARACTER_STATE.WALK:
            case E_CHARACTER_STATE.RUN:
            case E_CHARACTER_STATE.FOLLOW:
                break;
            default:
                return;
        }

        if (target == null) return;

        if (moveType == CharacterBase.MoveType.Follow)
        {
            var distance = (target.currentPosition - currentPosition).magnitude;

            if (distance >= distanceToPlayer && distance < distanceToSee)
            {
                ChangeState(new CharacterStateFollow(target, 1.5f, 2f), false);
            }
            else if (distance < distanceToStepAside)
            {
                ChangeState(new CharacterStateFollow(target, 1.5f, 2f), false);
            }
        }
        else
        {
            if (waypoints == null || waypoints.Count == 0) return;

            var distFromOrigin = Vector2.Distance(target.currentPosition, waypoints[0]);

            if (ignoreFollowRange || distFromOrigin < followRange)
            {
                if (GetCurrentCharacterState() == E_CHARACTER_STATE.IDLE ||
                    GetCurrentCharacterState() == E_CHARACTER_STATE.WALK ||
                    GetCurrentCharacterState() == E_CHARACTER_STATE.RUN)
                {
                    var distance = (target.currentPosition - currentPosition).magnitude;

                    if (distance >= distanceToPlayer && distance < distanceToSee)
                    {
                        ChangeState(new CharacterStateFollow(target, 1.5f, 2), false);
                    }
                    else if (distance < distanceToStepAside)
                    {
                        ChangeState(new CharacterStateFollow(target, 1.5f, 2), false);
                    }
                }
            }
            else
            {
                if (DistanceToTarget <= ApproachDistance.Epsilon || GetCurrentCharacterState() == E_CHARACTER_STATE.FOLLOW)
                {
                    var targetPos = MathFunctions.GetWorldPhysicPosByLogicPos(isOutdoor, waypoints[0], waypoints[0].z);
                    UpdateTargetPosition(targetPos);
                    ChangeState(new CharacterStateIdle(), false);
                }
            }
        }
    }
    
    void CheckForIdleLookAt(PhysicalObject target)
    {
        const float distanceToPlayer = 4f;
        const float distanceToRelease = 5f;

        if (target == null) return;

        if (GetEmotionState() != E_EMOTION_STATE.DEFAULT)
        {
            return;
        }

        switch (GetCurrentCharacterState())
        {
            case E_CHARACTER_STATE.IDLE:
            case E_CHARACTER_STATE.WALK:
            case E_CHARACTER_STATE.RUN:
            case E_CHARACTER_STATE.IDLE_LOOK_AT:
                break;
            default:
                return;
        }

        var distance = (target.currentPosition - currentPosition).magnitude;

        if (GetCurrentCharacterState() != E_CHARACTER_STATE.IDLE_LOOK_AT)
        {
            if (distance < distanceToPlayer)
            {
                if (moveType != MoveType.Fixed || distanceToStop <= Mathf.Epsilon)
                {
                    StopMove(false);
                    ChangeState(new CharacterStateIdleLookAt(target), false);
                }
            }
        }
        else
        {
            if (distance > distanceToRelease)
            {
                ChangeState(new CharacterStateIdle(), false);
            }
        }
    }

    void CheckForStepAwayFrom(PlayerCharacter target)
    {
        const float distanceToAvoid = 1f;
        const float distanceToRelease = 5f;
        const float distanceToReturn = 6f;

        if (target == null) return;

        if (GetEmotionState() != E_EMOTION_STATE.DEFAULT)
        {
            return;
        }

        switch (GetCurrentCharacterState())
        {
            case E_CHARACTER_STATE.IDLE:
            case E_CHARACTER_STATE.WALK:
            case E_CHARACTER_STATE.RUN:
            case E_CHARACTER_STATE.IDLE_LOOK_AT:
            case E_CHARACTER_STATE.STEP_AWAY:
                break;
            default:
                return;
        }

        var distFromStandPos = Vector2.Distance(standPos, currentPosition);
        var distance = Vector2.Distance(target.currentPosition, currentPosition);

        if (GetCurrentCharacterState() != E_CHARACTER_STATE.STEP_AWAY)
        {
            if (distance < distanceToAvoid && (standRange <= ApproachDistance.Epsilon || distFromStandPos < standRange))
            {
                ChangeState(new CharacterStateStepAway(target, 1.5f, 2), false);
                return;
            }

            // Anne - i don't know!!!!!
            if (UIManager.Instance.IsTutorialMode())
                return;

            // Get out of over-range status
            if (distanceToTarget <= ApproachDistance.Epsilon && distance < distanceToAvoid && standRange > ApproachDistance.Epsilon && distFromStandPos > standRange)
            {
                var targetPos = currentPosition + (standPos - currentPosition).normalized * (distFromStandPos - standRange + 0.3f);
                UpdateTargetPosition(targetPos);

                if (GetCurrentCharacterState() != E_CHARACTER_STATE.IDLE)
                {
                    ChangeState(new CharacterStateIdle(), false);
                }
                return;
            }
        }
        else
        {
            if (distanceToStop < ApproachDistance.Epsilon || (distance >= distanceToRelease && (standRange <= ApproachDistance.Epsilon || distFromStandPos > standRange)))
            {
                StopMove(false);
                ChangeState(new CharacterStateIdle(), false);
                return;
            }
        }

        if (distance >= distanceToReturn)
        {
            if (targetPosition != standPos)
            {
                UpdateTargetPosition(standPos);

                if (GetCurrentCharacterState() != E_CHARACTER_STATE.IDLE)
                {
                    ChangeState(new CharacterStateIdle(), false);
                }
            }
        }
    }

    protected sealed override void Update()
    {
        base.Update();

        if (playerCharacter)
        {
            DistanceToPlayer = Vector2.Distance(playerCharacter.currentPosition, currentPosition);
        }

        switch (moveType)
        {
            case MoveType.Follow:
                //			if(npcData.npcRef.ID == 4000)
                //				CheckForFollowEbichu(followTarget);
                //			else
                CheckForFollow(followTarget);
                break;
            case MoveType.Fixed:
                if (onRiding == false)
                {
                    if (LookAtPlayer) CheckForIdleLookAt(playerCharacter);
                    if (StepAwayFromPlayer) CheckForStepAwayFrom(playerCharacter);
                }
                break;
            case MoveType.Waypoint:
                if (onRiding == false)
                {
                    if (LookAtPlayer) CheckForIdleLookAt(playerCharacter);
                    UpdateWaypoint(Time.deltaTime);
                }
                break;
            case MoveType.Path:
                FollowPath(Time.deltaTime);
                break;
            case MoveType.UI:
                if (emotionManager != null)
                {
                    emotionManager.Update();
                }

                stateManager.Update();
                return;
        }

        if (GetEmotionState() == E_EMOTION_STATE.DEFAULT && GetCurrentCharacterState() == E_CHARACTER_STATE.IDLE_FOR_EMOTION)
        {
            // Emotion -> Idle
            ChangeState(new CharacterStateIdle(), false);
        }

        if (onRiding)
        {
            SetPosition(MathFunctions.GetLogicPosByPhysicPos(true, transform.position));
            SetHeight(MathFunctions.GetWorldHeightByPhysicPos(true, transform.position));
            SetAngle(transform.eulerAngles.y);
        }

        PlayAnim();

    }

    public override bool HasWaypoints()
    {
        return (waypoints != null);
    }

    // Event Listener Interface

    public override bool OnMessage(EventBase msg)
    {
        base.OnMessage(msg);

        if (msg is WorldEvent)
        {
            bool resetState = false;

            switch ((E_WORLD_EVENT)msg.MsgType)
            {
                case E_WORLD_EVENT.SCENE_PREPARING:
                    if (isOutdoor)
                    {
                        var eventArgs = msg.GetParam(0) as OutdoorSceneChangedEventArgs;
                        resetState = eventArgs != null && eventArgs.prevBuildingID == InstanceID.InvalidID;
                    }
                    else
                    {
                        resetState = msg.GetParam(0) is IndoorSceneChangedEventArgs;
                    }

                    if (resetState)
                    {
                        if (identity != null)
                        {
                            if (!onRiding)
                            {
                                ResetPosition();
                            }
                        }
                    }
                    break;
                case E_WORLD_EVENT.CHARACTER_APPROACH_TO_GIVE:
                    if ((GameObject)msg.GetParam(0) == gameObject)
                    {
                        ChangeState(new CharacterStateReceive((GameObject)msg.GetParam(1)), false);
                        return true;
                    }
                    break;
                case E_WORLD_EVENT.REQ_NPC_GIVE:
                    if ((GameObject)msg.GetParam(0) == gameObject)
                    {
                        var target = playerCharacter.gameObject;
                        MoveTo(target.transform.position, ApproachDistance.Talk, new CharacterStateGive(target));
                        return true;
                    }
                    break;
                case E_WORLD_EVENT.REQ_NPC_RECEIVE:
                    // see Player's
                    break;
            }
        }

        return false;
    }
    
    public bool UpdatePositionFromSpawnLoc()
    {
        var spawnLoc = Data.SpawnLoc;

        if (spawnLoc.targetPos == null) return false;
        
        switch (spawnLoc.targetPos.targetType)
        {
            case Reactive.Targets.Pos:
                return UpdateSpawnPosFromTargetPos(spawnLoc.targetPos as Reactive.TargetPos);
            case Reactive.Targets.Grid:
                return UpdateSpawnPosFromTargetGrid(spawnLoc.targetPos as Reactive.TargetGrid);
            case Reactive.Targets.SpawnPoint:
                return UpdateSpawnPosFromTargetSpawnPoint(spawnLoc.targetPos as Reactive.TargetSpawnPoint);
            case Reactive.Targets.Player:
                return UpdateSpawnPosFromTargetPlayer(spawnLoc.targetPos as Reactive.TargetPlayer);
            case Reactive.Targets.Npc:
                break;
            case Reactive.Targets.Object:
                return UpdateSpawnPosFromTargetObject(spawnLoc.targetPos as Reactive.TargetObject);
            case Reactive.Targets.Building:
                break;
            case Reactive.Targets.Room:
                return UpdateSpawnPosFromTargetRoom(spawnLoc.targetPos as Reactive.TargetRoom);
            case Reactive.Targets.Door:
                break;
            case Reactive.Targets.MasterHouse:
                return UpdateSpawnPosFromTargetMasterHouse(spawnLoc.targetPos as Reactive.TargetMasterHouse);
        }

        return false;
    }

    bool UpdateSpawnPosFromTargetPos(Reactive.TargetPos target)
    {
        var physicPos = MathFunctions.GetRayCastPhysicPosByLogicPos(DataManager.Instance.IsOutdoor(), new Vector2(target.x, target.y), LayerDef.maskForGroundHeight);

        var logicPos = MathFunctions.GetLogicPosByPhysicPos(DataManager.Instance.IsOutdoor(), physicPos);

        Data.Position.x = logicPos.x;
        Data.Position.y = logicPos.y;
        Data.Position.height = physicPos.y;
        Data.Position.interiorID = DataManager.Instance.GetWorldScene().InteriorID;
        Data.Position.direction = SuPro.Blueprint.Direction.Undefined;

        return true;
    }

    bool UpdateSpawnPosFromTargetGrid(Reactive.TargetGrid target)
    {
        var physicPos = MathFunctions.GetRayCastPhysicPosByLogicPos(DataManager.Instance.IsOutdoor(), MathFunctions.GetLogicPosByGridPos(new GridPos(target.x, target.y)), LayerDef.maskForGroundHeight);

        var logicPos = MathFunctions.GetLogicPosByPhysicPos(DataManager.Instance.IsOutdoor(), physicPos);

        Data.Position.x = logicPos.x;
        Data.Position.y = logicPos.y;
        Data.Position.height = physicPos.y;
        Data.Position.interiorID = DataManager.Instance.GetWorldScene().InteriorID;
        Data.Position.direction = SuPro.Blueprint.Direction.Undefined;

        return true;
    }

    bool UpdateSpawnPosFromTargetSpawnPoint(Reactive.TargetSpawnPoint target)
    {
        Vector3 physicPos, eulerAngle;

        if (!SceneHelper.FindSpawnPoint(target.name, out physicPos, out eulerAngle)) return false;

        var logicPos = MathFunctions.GetLogicPosByPhysicPos(DataManager.Instance.IsOutdoor(), physicPos);

        Data.Position.x = logicPos.x;
        Data.Position.y = logicPos.y;
        Data.Position.height = physicPos.y;
        Data.Position.interiorID = DataManager.Instance.GetWorldScene().InteriorID;
        Data.Position.direction = SuPro.Blueprint.Direction.Undefined;

        return true;
    }

    bool UpdateSpawnPosFromTargetPlayer(Reactive.TargetPlayer target)
    {
        Vector2 logicPos;
        float height, angle;

        var playerCharacter = AvatarManager.Instance.GetPlayerCharacter();
        SceneHelper.FindEmptyPositionAroundActor(playerCharacter, AroundDistance.Character, 4, out logicPos, out height, out angle);

        Data.Position.interiorID = DataManager.Instance.GetWorldScene().InteriorID;
        Data.Position.direction = SuPro.Blueprint.Direction.Undefined;

        Data.Position.x = logicPos.x;
        Data.Position.y = logicPos.y;
        Data.Position.height = height;
        Data.Position.angle = angle;

        return true;
    }

    bool UpdateSpawnPosFromTargetObject(Reactive.TargetObject target)
    {
        Vector2 logicPos;
        float height;

        float angle = 0;

        var physicPos = GetPhysicPosFromTargetObject(target);

        logicPos = MathFunctions.GetLogicPosByPhysicPos(DataManager.Instance.IsOutdoor(), physicPos);
        height = MathFunctions.GetWorldHeightByPhysicPos(DataManager.Instance.IsOutdoor(), physicPos);

        Data.Position.interiorID = DataManager.Instance.GetWorldScene().InteriorID;
        Data.Position.direction = SuPro.Blueprint.Direction.Undefined;

        Data.Position.x = logicPos.x;
        Data.Position.y = logicPos.y;
        Data.Position.height = height;
        Data.Position.angle = angle;

        return true;
    }

    bool UpdateSpawnPosFromTargetRoom(Reactive.TargetRoom target)
    {
        Vector2 logicPos;
        float height = 0;

        float angle = 0;

        logicPos = Vector3.zero;

        Data.Position.interiorID = DataManager.Instance.GetWorldScene().InteriorID;
        Data.Position.direction = SuPro.Blueprint.Direction.Undefined;

        Data.Position.x = logicPos.x;
        Data.Position.y = logicPos.y;
        Data.Position.height = height;
        Data.Position.angle = angle;

        return true;
    }
    
    Vector3 GetPhysicPosFromTargetObject(Reactive.TargetObject target)
    {
        Vector3 targetPos = Vector3.zero;
        GameObject go = null;

        if (target.objectRef != null)
        {
            var goVar = target.objectRef.GetReference() as GameObjectVariable;
            go = goVar.GameObject;
        }

        if (go != null)
        {
            if (target.distance > 0)
            {
                targetPos = SceneHelper.GetApproachPos(this.gameObject, go, target.distance);
            }
            else
            {
                targetPos = go.transform.position;
            }

            if (target.offsetX != 0 || target.offsetY != 0)
            {
                var logicPos = MathFunctions.GetLogicPosByPhysicPos(DataManager.Instance.IsOutdoor(), targetPos);

                logicPos.x += target.offsetX;
                logicPos.y += target.offsetY;

                targetPos = MathFunctions.GetRayCastPhysicPosByLogicPos(DataManager.Instance.IsOutdoor(), logicPos, LayerDef.maskForGroundHeight);
            }
        }

        return targetPos;
    }

    bool UpdateSpawnPosFromTargetMasterHouse(Reactive.TargetMasterHouse target)
    {
        var spawnPoint = GameManager.Scene.World.GetComponentsInChildren<Identity>()
            .FirstOrDefault(o => o.ID.idType == IdentifierType.MasterWorkHouse)?.transform;
        if (spawnPoint != null)
        {
            var physicPos = spawnPoint.position;
            var logicPos = MathFunctions.GetLogicPosByPhysicPos(DataManager.Instance.IsOutdoor(), physicPos);
            var height = MathFunctions.GetWorldHeightByPhysicPos(DataManager.Instance.IsOutdoor(), physicPos);

            Data.Position.interiorID = DataManager.Instance.GetWorldScene().InteriorID;
            Data.Position.direction = SuPro.Blueprint.Direction.Undefined;

            Data.Position.x = logicPos.x;
            Data.Position.y = logicPos.y;
            Data.Position.height = height;

            return true;
        }

        return false;
    }
    
    public void MoveTo(Reactive.TargetBase target)
    {
        Vector3 targetPhysicPos = Vector3.zero;

        switch (target.targetType)
        {
            case Reactive.Targets.Pos:
                var targetPos = target as Reactive.TargetPos;
                targetPhysicPos = MathFunctions.GetRayCastPhysicPosByLogicPos(DataManager.Instance.IsOutdoor(), new Vector2(targetPos.x, targetPos.y), LayerDef.maskForGroundHeight);
                break;
            case Reactive.Targets.Grid:
                var targetGrid = target as Reactive.TargetGrid;
                targetPhysicPos = MathFunctions.GetRayCastPhysicPosByLogicPos(DataManager.Instance.IsOutdoor(), MathFunctions.GetLogicPosByGridPos(new GridPos(targetGrid.x, targetGrid.y)), LayerDef.maskForGroundHeight);
                break;
            case Reactive.Targets.Object:
                targetPhysicPos = GetPhysicPosFromTargetObject(target as Reactive.TargetObject);
                break;
            case Reactive.Targets.Player:
                // Find pos front of player, but avoid some place that can't walking.
                targetPhysicPos = NTAgentHelper.GetPosNearPlayer(1.5f);
                //			targetPhysicPos =  MathFunctions.GetRayCastPhysicPosByLogicPos(DataManager.Instance.IsOutdoor(), PlayerHelper.GetPlayer().currentPosition, LayerDef.maskForGroundHeight);
                break;
            case Reactive.Targets.Waypoint:
                //var targetWaypoint = target as Reactive.TargetWaypoint;
                //targetPhysicPos = NPCManager.Instance.GetOutdoorSpawnLoc(targetWaypoint.id);
                break;
            default:
                DCon.LogError("Not implemented!");
                return;
        }

        moveType = MoveType.Fixed;

        MoveTo(targetPhysicPos);
    }
    
    public void OnWorkActionChange(MasterWorkActionChangeArgs args)
    {
        var ctrl = MasterData.FindCtrl(args.workKind, args.id);

        if (ctrl == null)
        {
            return;
        }

        switch(args.actionType)
        {
            case MasterWorkActionChangeArgs.ActionType.MoveTo:
                var pos = GetWorkPosition(args.workKind, ctrl);
                MoveTo(new Reactive.TargetPos(pos.x, pos.y));
                break;

            case MasterWorkActionChangeArgs.ActionType.Make:

                //强行设置位置
                if (!IsArrived(false))
                {
                    SetMaster2WorkTargetPostion(args.workKind, ctrl);
                }

                //根据类型执行相应表现
                switch (args.workKind)
                {
                    case SuPro.WorkKind.Tree:
                        var treeCtrl = ctrl as TreeController;
                        ChangeState(new CharacterStateShakeTree(treeCtrl, () =>
                        {
                            //JobHelper.Run(Jobs.ApplyTreeChange(new List<SuPro.TreeChangeBase>() { change }));
                        }), false);
                        break;
                    case SuPro.WorkKind.Farm:
                        break;
                    case SuPro.WorkKind.Ranch:
                        break;
                    case SuPro.WorkKind.Factory:
                        break;
                }
                break;

            case MasterWorkActionChangeArgs.ActionType.SetPos:
                SetMaster2WorkTargetPostion(args.workKind, ctrl);
                break;
            
            case MasterWorkActionChangeArgs.ActionType.WaitCollect:
                SetMaster2WorkTargetPostion(args.workKind, ctrl);
                if (widgetObj == null)
                {
                    widgetObj = WidgetManager.Instance.Spawn(WidgetID.MasterWaitCollectWidget);
                }
                var follow = widgetObj.GetComponent<TFollowObject>();
                follow.Target = this.transform;
                follow.selfTarget = widgetObj.transform;
                follow.height = 2.5f;
                break;
        }
    }

   public Vector2 GetWorkPosition(SuPro.WorkKind workKind, GridObjectController ctrl)
    {
        switch (workKind)
        {
            case SuPro.WorkKind.Tree:
                var treePos = ctrl.Placement.GetLogicPos();
                return treePos - (treePos - currentPosition).normalized * ApproachDistance.Tree;
            case SuPro.WorkKind.Farm:
                return ctrl.Placement.GetLogicPos();
            case SuPro.WorkKind.Ranch:
                Vector2 ranch_pos;
                float ranch_height;
                float ranch_angle;
                int ranch_radius = Mathf.Max(ctrl.Placement.AreaSize.Height, ctrl.Placement.AreaSize.Width) / 2;
                SceneHelper.FindEmptyPositionAroundActor(((RanchController)ctrl).Object, 5, ranch_radius, out ranch_pos, out ranch_height, out ranch_angle);
                return ranch_pos;
            case SuPro.WorkKind.Factory:
                Vector2 factory_pos;
                float factory_height;
                float factory_angle;
                int factory_radius = Mathf.Max(ctrl.Placement.AreaSize.Height, ctrl.Placement.AreaSize.Width) / 2;
                SceneHelper.FindEmptyPositionAroundActor(((FactoryController)ctrl).Object, 5, factory_radius, out factory_pos, out factory_height, out factory_angle);
                return factory_pos;
            default:
                return default;
        }
    }

    void SetMaster2WorkTargetPostion(SuPro.WorkKind workKind, GridObjectController ctrl)
    {
        var logicPos = GetWorkPosition(workKind, ctrl);
        SetPosition(logicPos);
        SetHeight(MathFunctions.GetRaycastHeightByLogicPos(true, logicPos, LayerDef.maskForGroundHeight));
        ForceUpdatePhysics();
    }
    
    protected override void OnTargetReached()
    {
        base.OnTargetReached();

        if (!gameObject.activeSelf) return;
        
        //sync location data
        Data.Position.x = currentPosition.x;
        Data.Position.y = currentPosition.y;
        Data.Position.height = currentHeight;
        Data.Position.angle = currentAngle;
    }

    public void ApplyOutfit()
    {
        CharacterHelper.ApplyOutfit(Data.Appearence, this, EquipmentSocket.Undefined);
    }

    private void OnEnable()
    {
        if (widgetObj != null)
        {
            widgetObj.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (widgetObj != null)
        {
            widgetObj.SetActive(false);
        }
    }
}
