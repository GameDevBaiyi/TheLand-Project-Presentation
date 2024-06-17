using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StarCropEfxConfig
{
    [SerializeField]
    private int _growingPhase;
    public int GrowingPhase => this._growingPhase;

    [SerializeField]
    private Vector2Int _luckyPowderRange;
    public Vector2Int LuckyPowderRange => this._luckyPowderRange;

    [SerializeField]
    private GameObject _particleGo;
    public GameObject ParticleGo => this._particleGo;
}

public class StarCropEfx : MonoBehaviour
{
    [SerializeField]
    private List<StarCropEfxConfig> _starCopEfxConfigs;

    private GameObject _currentParticleGo;
    [SerializeField]
    private GameObject[] _baseEf;

    [SerializeField]
    private GameObject[] _baseStarEf;
    public void UpdateParticle(int growingPhase,int luckyPowderCount, int specialLuckyPowderCount, int level)
    {
        if (luckyPowderCount == 0 && specialLuckyPowderCount == 0)
        {
            if (this._currentParticleGo != null)
                this._currentParticleGo.SetActive(false);
            for (int i = 0; i < _baseEf.Length; i++)
                _baseEf[i].gameObject.SetActive(false);
            if(growingPhase < _baseEf.Length)
                _baseEf[growingPhase].gameObject.SetActive(true);
            return;
        }
        if (this._currentParticleGo != null) this._currentParticleGo.SetActive(false);
        /* if (this._currentParticleGo != null) this._currentParticleGo.SetActive(false);
         bool Match(StarCropEfxConfig config)
         {
             if (config.GrowingPhase != growingPhase) return false;
             return luckyPowder >= config.LuckyPowderRange.x && luckyPowder < config.LuckyPowderRange.y;
         }
         StarCropEfxConfig starCropEfxConfig = this._starCopEfxConfigs.Find(Match);
         if (starCropEfxConfig.ParticleGo == null)
         {
             Debug.LogError($"未找到 该星之作物的特效: 生长阶段: {growingPhase}  星辰粉末数量: {luckyPowder}");
             return;
         }*/
        var gobj = _baseStarEf[level-1];
        this._currentParticleGo = gobj;
        this._currentParticleGo.SetActive(true);
        //var effTable = LuckLvData.LuckyPowderEffect;
        //foreach(var item in effTable)
        //{
        //    if(item.Min<= luckyPowderCount)
        //    {
        //        if(item.Max >= luckyPowderCount || item.Max == -1)
        //        {
        //            var index = item.ID - 1;
        //            if (index >= 0 && index < _baseStarEf.Length)
        //            {
        //                var gobj = _baseStarEf[index];
        //                this._currentParticleGo = gobj;
        //                this._currentParticleGo.SetActive(true);
        //            }
        //        }
        //    }
        //}
        //BlueprintStorage.Instance.Tables.LuckyPowderEffect.Records[];
       // this._currentParticleGo = starCropEfxConfig.ParticleGo;
       // this._currentParticleGo.SetActive(true);
    }
    public void UpdateParticle(FarmData farmData)
    {
        DateTime growAt = farmData.GrowAt;
        int growTimeInMinutes = farmData.CropRef.CoolTime;
        DateTime halfGrowAt = growAt - TimeSpan.FromSeconds(growTimeInMinutes * 60d / 2d);
        int growLevel = 0;
        if (halfGrowAt > NetworkManager.Instance.Now)
        {
            growLevel = 0;
        }
        else if (growAt > NetworkManager.Instance.Now)
        {
            growLevel = 1;
        }
        else
        {
            growLevel = 2;
        }

        this.UpdateParticle(growLevel,farmData.LuckyPowderCount, farmData.SpecialLuckyPowderCoun, farmData.PowderEffectLevel);
    }
}