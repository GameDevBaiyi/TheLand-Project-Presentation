using System.Collections;
using System.Collections.Generic;

using SuPro;

using UnityEngine;

using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class MasterAi : MonoBehaviour
{
    private Master _master;

    private const float _staticDuration = 5f;
    private IEnumerator RandomWalkAsync()
    {
        this._master = this.GetComponent<Master>();
        if (this._master.Data.WorkInfo.WorkKind != WorkKind.Farm) yield break;
        float timer = 0f;
        while (true)
        {
            if (this._master.GetAnimState() != CharacterAnimState.Idle) goto NextFrame;
            if (!this._master.gameObject.activeInHierarchy) goto NextFrame;
            timer += Time.deltaTime;
            if (timer < _staticDuration) goto NextFrame;
            timer = 0f;
            Vector3 centerPos = this._master.transform.position;
            Ray ray = new Ray(centerPos + Vector3.up * 50f,Vector3.down);
            bool hasTouchable = Physics.Raycast(ray,out RaycastHit hit,100f,LayerMask.GetMask("Touchable"));
            if (hasTouchable)
            {
                centerPos = hit.point;
            }
            else
            {
                GridObjectController gridObjectController = MasterData.FindCtrl(this._master.Data.WorkInfo.WorkKind,this._master.Data.WorkInfo.LastPosId);
                Vector2 workPos = this._master.GetWorkPosition(WorkKind.Farm,gridObjectController);
                centerPos = new Vector3(workPos.x,0f,workPos.y);
            }
            List<Transform> adjacentFarmObjects = new List<Transform>(8);
            for (int xOffset = -1; xOffset <= 1; xOffset++)
            {
                for (int zOffset = -1; zOffset <= 1; zOffset++)
                {
                    if (xOffset == 0
                     && zOffset == 0) continue;
                    Vector3 adjacentPos = centerPos + new Vector3(xOffset * 2f,0f,zOffset * 2f);
                    ray = new Ray(adjacentPos + Vector3.up * 50f,Vector3.down);
                    bool hasAdjacentTouchable = Physics.Raycast(ray,out hit,100f,LayerMask.GetMask("Touchable"));
                    if (!hasAdjacentTouchable) continue;
                    if (!hit.transform.GetComponent<FarmCollider>()) continue;
                    adjacentFarmObjects.Add(hit.transform.parent);
                }
            }
            int adjacentCount = adjacentFarmObjects.Count;
            if (adjacentCount == 0) goto NextFrame;
            int randomIndex = UnityEngine.Random.Range(0,adjacentCount);
            this._master.MoveTo(adjacentFarmObjects[randomIndex].position);

        NextFrame:
            yield return null;
        }
    }

    public void StartRandomWalk()
    {
        this.StartCoroutine(this.RandomWalkAsync());
    }
}