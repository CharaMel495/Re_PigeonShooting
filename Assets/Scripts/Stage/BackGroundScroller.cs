using UnityEngine;

public class BackGroundScroller : MonoBehaviour
{
    [SerializeField]
    private Material _bgMaterial;

    [SerializeField] 
    private float _scrollSpeed = 0.1f;

    private Vector3 _offset;

    public void UpdateOffset(Vector3 move)
    {
        // プレイヤーの移動量に応じてオフセット加算
        _offset += move * _scrollSpeed * Time.deltaTime;

        _bgMaterial.mainTextureOffset = (Vector2)_offset;
    }
}
