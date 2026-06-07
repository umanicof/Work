using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UniRx;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace NkfLib.Unity
{
    // 一定間隔ごとに１文字ずつ追加して出力されるメッセージ用文字列
    // ※TextMeshPro表示用。タグを使用している。
    public class MessageString
    {
        public float MessageSpeed { get; set; } = 5; // メッセージ速度（1秒に表示する文字数）
        public bool Fixed { get; set; } = true; // 最終的な出力文字数および行数を保ったメッセージを出力する（文字がずれない）
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        public IObservable<string> OnChanged { get { return _onChanged; } }
        public IObservable<string> OnFinish { get { return _onFinish; } }

        int _pos;
        Subject<string> _onChanged = new Subject<string>();
        Subject<string> _onFinish = new Subject<string>();
        IDisposable _dispasable;

        string _originalMessage;

        ReactiveProperty<string> _outputMessage = new ReactiveProperty<string>("");

        SimpleLocalizedString Space { get; } = new SimpleLocalizedString("SystemText", "TEXTID_SPACE");
        public MessageString()
        {
            _outputMessage.Subscribe(x =>
            {
                _onChanged?.OnNext(x);
            });
        }

        public void SetMessage(string original)
        {
            _originalMessage = original;
            _outputMessage.Value = "";
            _pos = 0;
            StopTimer();
        }
        public string GetOutputMessage()
        {
            return _outputMessage.Value;
        }

        public void StartTimer()
        {
            StopTimer();
            Debug.Assert(MessageSpeed > 0.0f);
            _outputMessage.Value = "";
            _pos = 0;
            if (_originalMessage.Length <= 0)
            {
                _onFinish?.OnNext(_originalMessage);
                return;
            }
            IsRunning = true;

            // TODO: MessageSpeedが早すぎると多分追いつかない
            _dispasable = Observable.Interval(TimeSpan.FromSeconds(1.0f / MessageSpeed)).Subscribe(_ =>
            {
                if (IsPaused)
                {
                    return;
                }

                ++_pos;
                string message = "";

                if (Fixed)
                {
                    // 参考：TextMeshProで文末にスペース等が入力できない問題。新しいバージョンでは解決されていそう
                    //       https://forum.unity.com/threads/why-there-is-no-setting-for-textmesh-pro-ugui-to-count-whitespace-at-the-end.676897/
                    const string AlphaChar = "<alpha=#00>.<alpha=#FF>"; // 文字列の両端に透明の文字を付与することで、スペース等が切り取られないよう調整している
                    message = _originalMessage.Substring(0, _pos);
                    string postMessage = _originalMessage.Substring(_pos);

                    // 改行以外をスペースに置換（正規表現では単語を除くマッチングは面倒そう）
                    // TODO: 英語だと少しずれる（等幅フォントにすればずれないかも）
                    var splits = postMessage.Split(Environment.NewLine);
                    int row = 0;
                    foreach (var s in splits)
                    {
                        ++row;
                        string nl = row == splits.Length ? "" : Environment.NewLine; // 改行文字列
                        var s2 = Regex.Replace(s, ".", Space.ToString()); // スペースに置換
                        message += s2 + nl;
                    }

                    // 各行を透明文字で囲む
                    splits = message.Split(Environment.NewLine);
                    row = 0;
                    message = "";
                    foreach (var s in splits)
                    {
                        ++row;
                        string nl = row == splits.Length ? "" : Environment.NewLine; // 改行文字列
                        message += AlphaChar + s + AlphaChar + nl;
                    }
                }
                else
                {
                    message = _originalMessage.Substring(0, _pos);
                }

                _outputMessage.Value = message;
                if (_pos >= _originalMessage.Length)
                {
                    StopTimer(true);
                }
            });
        }
 
        public void StopTimer(bool isFinish = false)
        {
            _dispasable?.Dispose();
            _dispasable = null;
            IsRunning = false;

            if (isFinish)
            { 
                // この呼び出しで再度StartTimerが呼ばれる可能性に注意
                _onFinish?.OnNext(_originalMessage);
            }
        }

        public override string ToString()
        {
            return _outputMessage.Value;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }
    }
}
