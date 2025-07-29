using UnityEngine;

public class PlayerMover
{
    private float _moveSpeed;

    private Rect _playerArea;

    public PlayerMover(float moveSpeed, Rect area)
    {
        _moveSpeed = moveSpeed;
        _playerArea = area;
    }

    public void Initialize(float moveSpeed, Rect area)
    {
        _moveSpeed = moveSpeed;
        _playerArea = area;
    }

    public void MovePlayer(Player player)
    {
        var inputDir = InputManager.GetInputDirection(InputHandler.Player).normalized;

        var transform = player.transform;
        var pos = transform.position;
        var scale = transform.localScale;
        var halfScale = scale * 0.5f;
        pos += (Vector3)inputDir * _moveSpeed * Time.fixedDeltaTime;

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
