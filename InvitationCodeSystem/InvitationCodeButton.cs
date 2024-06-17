using System;
using System.Collections;
using System.Threading;

using SuPro;

using UnityEngine;

namespace ScriptsSortedByFunctions.InvitationCodeSystem
{
[Serializable]
public class InvitationCodeButton
{
    public const int DurationInHours = 72;

    [SerializeField]
    private GameObject _go;
    [SerializeField]
    private UILabel _timerLabel;

    /// <summary>
    /// 当 主 UI 打开的时候, 默认那个按钮是关闭的. 会从服务器请求时间, 然后判断是否显示.
    /// </summary>
    public void CheckAndShowButton()
    {
        if (!DataManager.Instance.IsInMyTown())
        {
            this._go.SetActive(false);
            return;
        }
        bool canInvite = InvitationCodeWidget.CanInvite();
        if (!canInvite)
        {
            this._go.SetActive(false);
            return;
        }
        this._go.SetActive(true);
        this.StopPreviousAsyncAndCountDownAgain();
        this._go.GetComponentInParent<UIGrid>().Reposition();
    }

    private CancellationTokenSource _countDownCts;
    private IEnumerator CountDownAsync(CancellationToken token)
    {
        WaitForSeconds wait1Sec = new WaitForSeconds(1f);
        TimeSpan timeSpan = InvitationCodeLogic.ActivityStartTime.AddHours(DurationInHours) - NetworkManager.Instance.Now;
        while (true)
        {
            if (token.IsCancellationRequested) yield break;
            this._timerLabel.text = $"{Mathf.FloorToInt((float)timeSpan.TotalHours)}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";

            yield return wait1Sec;
            timeSpan = timeSpan.Subtract(TimeSpan.FromSeconds(1d));
            if (timeSpan.TotalSeconds <= 0d) this._go.SetActive(false);
            if (!this._go.activeSelf) break;
        }
    }
    private void StopPreviousAsyncAndCountDownAgain()
    {
        this._countDownCts?.Cancel();
        this._countDownCts?.Dispose();
        this._countDownCts = new CancellationTokenSource();
        UIJobManager.Instance.runner.StartCoroutine(this.CountDownAsync(this._countDownCts.Token));
    }
}
}