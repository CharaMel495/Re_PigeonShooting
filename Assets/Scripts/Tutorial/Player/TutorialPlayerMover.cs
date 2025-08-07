using UnityEngine;

public class TutorialPlayerMover
{
    private float _moveSpeed;

    private Rect _playerArea;

    private Vector3 _inputDir;

    private bool _inputLock = false;

    private float _dashMag = 5.0f;

    public TutorialPlayerMover(float moveSpeed, Rect area)
    {
        _moveSpeed = moveSpeed;
        _playerArea = area;
    }

    public void Initialize(float moveSpeed, Rect area)
    {
        _moveSpeed = moveSpeed;
        _playerArea = area;
    }

    public void MovePlayer(TutorialPlayer player)
    {
        if (!_inputLock)
            _inputDir = InputManager.GetInputDirection(InputHandler.Player).normalized;

        _inputLock = player.IsDash;

        var transform = player.transform;
        var pos = transform.position;
        var scale = transform.localScale;
        var halfScale = scale * 0.5f;
        pos += _inputDir * _moveSpeed * Time.fixedDeltaTime * (player.IsDash ? _dashMag : 1);

        if (CheckArea(pos, halfScale))
            CrrectInArea(ref pos, halfScale);

        transform.position = pos;
    }

    private bool CheckArea(Vector3 pos, Vector3 padding)
    {
        return
            pos.x < _playerArea.min.x + padding.x ||
            pos.y < _playerArea.min.y + padding.y ||
            pos.x > _playerArea.max.x - padding.x ||
            pos.y > _playerArea.max.y - padding.y;
    }

    private void CrrectInArea(ref Vector3 pos, Vector3 padding)
    {
        if (pos.x < _playerArea.min.x + padding.x)
            pos.x = _playerArea.min.x + padding.x;
        if (pos.y < _playerArea.min.y + padding.y)
            pos.y = _playerArea.min.y + padding.y;
        if (pos.x > _playerArea.max.x - padding.x)
            pos.x = _playerArea.max.x - padding.x;
        if (pos.y > _playerArea.max.y - padding.y)
            pos.y = _playerArea.max.y - padding.y;
    }
}
