using UnityEngine;
using System;

/// <summary>
/// シングルトンでMonoBehaviorを継承する際の基底クラス
/// </summary>
/// <typeparam name="T">クラステンプレート</typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    //インスタンス
    private static T instance;

    public static T Instance
    {
        get
        {
            //もしインスタンスがnullだったら
            if (instance == null)
            {
                //自身の型を取得
                Type t = typeof(T);

                //同じ型を持つオブジェクトを探して、インスタンス登録
                instance = (T)FindObjectOfType(t);

                //下はアタッチされてない時にエラーログを出せる
                //(全てのシーンに全てのスクリプトをいれたくないので今はコメント化してる)
                //if (instance == null)
                //{
                //    Debug.LogError(t + " をアタッチしているGameObjectはありません");
                //}

                //return null;
            }

            //インスタンスを返す
            return instance;
        }
    }

    virtual protected void Awake()
    {
        //複数にアタッチされてないか調べる
        CheckInstance();
    }

    /// <summary>
    /// 他のゲームオブジェクトにアタッチされているか調べ
    /// アタッチされている場合は破棄する。
    /// </summary>
    protected bool CheckInstance()
    {
        //インスタンスがnull(自分だけがこのクラスを持っていれば)
        if (instance == null)
        {
            //インスタンスの型を自身の型にキャスト
            instance = this as T;

            return true;
        }
        else if (Instance == this)
        {
            return true;
        }

        //他にインスタンスが見つかった場合は自身を削除する
        Destroy(this);

        return false;
    }


}
