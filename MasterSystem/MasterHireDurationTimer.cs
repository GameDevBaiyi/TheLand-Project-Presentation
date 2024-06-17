using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DefaultNamespace.Common;

using UnityEngine;

public class MasterHireDurationTimer : SingletonMonoBehaviour<MasterHireDurationTimer>
{
    public void CheckExpiredMasterAndSendRequest()
    {
        this.StopAllCoroutines();
        this.StartCoroutine(this.CheckExpiredMasterAndSendRequestAsync());
    }

    private IEnumerator CheckExpiredMasterAndSendRequestAsync()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(3f);
        while (true)
        {
            List<MasterInfo> masterInfos = DataManager.Instance.MyAvatar.MasterData.List;
            bool hasHiredMaster = masterInfos.Any(t => t.Data.masterType == SuPro.MasterType.MasterHire);
            if (!hasHiredMaster) yield break;
            MasterInfo firstExpiredMaster = masterInfos.Where(t => t.Data.masterType == SuPro.MasterType.MasterHire)
                                                       .OrderBy(t => t.Data.expireTimer)
                                                       .First();
            DateTime expireTime = firstExpiredMaster.Data.expireTimer;
            if (expireTime > NetworkManager.Instance.Now)
            {
                yield return waitForSeconds;
                continue;
            }

            void AckHandler(SuPro.MasterRecoveryAck masteHireInfoAck,NetMessageContext context)
            {
                if (masteHireInfoAck.Result != SuPro.MasterBaseResult.Success) return;
                DataManager.Instance.MyAvatar.MasterData.UpdateList(masteHireInfoAck.AvatarMasterList);
                MasterHireDurationTimer.Instance.CheckExpiredMasterAndSendRequest();
            }
            NetworkManager.Instance.SendToTS(new SuPro.MasterRecoveryReq()).Ack<SuPro.MasterRecoveryAck>(AckHandler);
            yield return waitForSeconds;
        }
    }

    private void OnDestroy()
    {
        this.StopAllCoroutines();
    }
}