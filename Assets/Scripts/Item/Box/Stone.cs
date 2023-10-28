using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : EntityBase
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public virtual void Init(Sprite sprite, int sortOrder)
    {
        isDie = true;

        if (sprite != null) {
            
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = Color.white;
            if (sprite.name.Contains("water_set") || sprite.name.Contains("ground_set_2"))
            {
                spriteRenderer.color = Color.clear;
            }
        }

        var pos = transform.position;
        if (pos.x != 0) pos.x = (int)pos.x + (pos.x < 0 ? -0.5f : 0.5f);
        if (pos.y != 0) pos.y = (int)pos.y + (pos.y < 0 ? -0.5f : 0.5f);

        pos.z = pos.y / 1000;
        //pos.z = -sortOrder;

        transform.position = pos;
    }

    protected override void LateUpdate()
    {
    }
}
