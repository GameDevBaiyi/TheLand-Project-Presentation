using System;

using SuPro;

using UnityEngine;

public class VrModeController : MonoBehaviour
{
    [SerializeField]
    private CharacterBase _characterBase;
    [SerializeField]
    private CharacterNetSync _characterNetSync;

    private void Awake()
    {
        this.TryGetComponent(out this._characterBase);
        this.TryGetComponent(out this._characterNetSync);
    }

    public bool IsVrMode
    {
        get => this._isVrMode;
        set
        {
            this._isVrMode = value;
            this._timer = 0f;
            if (!value) this.SendRouteTo();
        }
    }

    private const float _moveSyncInterval = 3f;
    private float _timer;
    [SerializeField]
    private bool _isVrMode;
    private void FixedUpdate()
    {
        if (!this.IsVrMode) return;

        this._timer += Time.fixedDeltaTime;
        if (this._timer < _moveSyncInterval) return;
        this._timer -= _moveSyncInterval;

        this.SendRouteTo();
    }

    private void SendRouteTo()
    {
        if (this._characterBase == null
         || this._characterNetSync == null)
        {
            this.TryGetComponent(out this._characterBase);
            this.TryGetComponent(out this._characterNetSync);
        }
        if (this._characterBase == null
         || this._characterNetSync == null)
        {
            Debug.LogError(" _characterBase or _characterNetSync is null.");
            return;
        }
        PositionMoveRoute positionMoveRoute = new PositionMoveRoute()
        {
            ActorId = DataManager.Instance.MyAvatarID,SyncId = this._characterNetSync.NextSyncID(),
            TarPos = new Vector2f() { X = this._characterBase.currentPosition.x,Y = this._characterBase.currentPosition.y },
            TarAngle = this._characterBase.currentAngle,StopDistance = 0f,FastMove = true,
        };
        NetworkManager.Instance.SendRouteTo(TSocketType.TS,positionMoveRoute,P2POption.UnreliableSend);
    }
}