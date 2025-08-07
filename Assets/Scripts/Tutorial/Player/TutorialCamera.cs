using UnityEngine;

public class TutorialCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _followTarget;

    private float _halfWidth;
    private float _halfHeight;

    private Vector3 _shakeOffset;
    private float _shakeTime;
    private float _shakePower;

    private void Start()
    {
        var cam = GetComponent<Camera>();
        _halfHeight = cam.orthographicSize;
        _halfWidth = _halfHeight * cam.aspect;
    }

    private void Update()
    {
        Follow();
        UpdateShake();

        if (Input.GetKey(KeyCode.V))
            Shake(0.5f, 2.0f);
    }

    private void Follow()
    {
        Vector3 pos = _followTarget.position;
        float posZ = transform.position.z;

        Correct(ref pos); // プレイヤー位置をステージ内でClamp
        pos.z = posZ;

        // シェイクオフセットを最後に適用
        transform.position = pos + _shakeOffset;
    }

    private void Correct(ref Vector3 pos)
    {
        var playArea = TutorialStageManager.Instance.PlayArea;

        pos.x = Mathf.Clamp(pos.x,
            playArea.xMin + _halfWidth,
            playArea.xMax - _halfWidth);

        pos.y = Mathf.Clamp(pos.y,
            playArea.yMin + _halfHeight,
            playArea.yMax - _halfHeight);
    }

    /// <summary>
    /// カメラを揺らす（duration: 持続時間, power: 揺れ幅）
    /// </summary>
    public void Shake(float duration, float power)
    {
        _shakeTime = duration;
        _shakePower = power;
    }

    private void UpdateShake()
    {
        if (_shakeTime > 0f)
        {
            _shakeTime -= Time.deltaTime;
            _shakeOffset = (Vector3)Random.insideUnitCircle * _shakePower;
        }
        else
        {
            _shakeOffset = Vector3.zero;
        }
    }
}
