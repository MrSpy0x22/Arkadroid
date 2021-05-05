using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Block : MonoBehaviour
{
    public int _hitPoints = 1;

    public SpriteRenderer _blockGraphic;
    public ParticleSystem _destroyEffect;
    public static event Action<Block> OnBlockDestroy;

    private void Awake()
    {
        this._blockGraphic = this.gameObject.GetComponent<SpriteRenderer>();
        //this._blockGraphic.sprite = BlocksManager.Instance._spriteTextures[this._hitPoints - 1];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball _ball = collision.gameObject.GetComponent<Ball>();
        ApplyCollision();
    }

    private void ApplyCollision()
    {
        this._hitPoints--;

        if (this._hitPoints <= 0)
        {
            BlocksManager.Instance.LevelBlocks.Remove(this);
            OnBlockDestroy?.Invoke(this);
            this.AddDrop();
            SpawnDeathEffect();
            Destroy(this.gameObject);
        }
        else
        {
            this._blockGraphic.sprite = BlocksManager.Instance._spriteTextures[this._hitPoints - 1];
        }
    }

    private void AddDrop()
    {
        float buffChance = UnityEngine.Random.Range(0f, 100f);
        float debuffChance = UnityEngine.Random.Range(0f, 100f);
        bool isSpawned = false;

        if (buffChance <= DropsManager.Instance.BuffChance)
        {
            isSpawned = true;
            Drop drop = this.SpawnDrop(true);
        }

        if (debuffChance <= DropsManager.Instance.DebuffChance && !isSpawned)
        {
            Drop drop = this.SpawnDrop(false);
        }
    }

    private Drop SpawnDrop(bool buff)
    {
        List<Drop> list;

        if (buff)
        {
            list = DropsManager.Instance.AvailableBuffs;
        }
        else
        {
            list = DropsManager.Instance.AvailableDebuffs;
        }

        int buffIndex = UnityEngine.Random.Range(0, list.Count);
        Drop prefab= list[buffIndex];
        Drop drop = Instantiate(prefab, this.transform.position, Quaternion.identity);

        return drop;
    }

    private void SpawnDeathEffect()
    {
        Vector2 _blockPosition = this.gameObject.transform.position;
        Vector3 _effectPosition = new Vector3(_blockPosition.x , _blockPosition.y , -0.2f);

        GameObject _effect = Instantiate(_destroyEffect.gameObject , _effectPosition , Quaternion.identity);

        MainModule _mainModule = _effect.GetComponent<ParticleSystem>().main;
        _mainModule.startColor = this._blockGraphic.color;
        Destroy(_effect , _destroyEffect.main.startLifetime.constant);

    }

    public void Prepare(Transform parentTransform, Sprite sprite, Color color, int blockType)
    {
        this.transform.SetParent(parentTransform);
        this._blockGraphic.sprite = sprite;
        this._blockGraphic.color = color;
        this._hitPoints = blockType;
    }
}
