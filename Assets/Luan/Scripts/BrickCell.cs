using UnityEngine;

public class BrickCell : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private SpriteRenderer _icon;

    private void OnEnable()
    {
        /*    var pos = transform.position;
            pos.z = pos.y / 1000;
            transform.position = pos;*/
        var pos = transform.position;
        _icon.sortingOrder = (int) (pos.y * -10);
    }

    private void OnValidate()
    {
        _collider = GetComponent<Collider2D>();
        _icon = GetComponent<SpriteRenderer>();
    }
}