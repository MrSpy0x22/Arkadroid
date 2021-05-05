using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    #region Singleton
    private static Platform _instance = null;
    public static Platform Instance => _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public static float SizeNormal = 1.5902f;
    public static float SizeWide = 3f;
    public static float SizeNarrow = 0.9f;

    public const float SizeHeight = 0.4078406f;
    public const int AnimationTime = 10;

    public bool IsTransforming { get; set; }

    SpriteRenderer platformSprite;
    BoxCollider2D platformCollider;

    #region Private fields
    private Vector3 _touchVector;
    #endregion

    private void Start()
    {
        _touchVector = Vector3.zero;
        platformSprite = gameObject.GetComponent<SpriteRenderer>();
        platformCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Touch movement handling for mobile devices
        if (Input.touchCount > 0)
        {
            Touch _touch = Input.GetTouch(0);
            _touchVector = Camera.main.ScreenToWorldPoint(_touch.position);
            _touchVector.y = -4.5f;
            _touchVector.z = 0;
            transform.position = new Vector2(_touchVector.x, transform.position.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D _ballRigidBody = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector2 _platformPosition = this.gameObject.transform.position;
            Vector2 _platformCenter = new Vector2(_platformPosition.x, _platformPosition.y);
            Vector2 _hitPosition = collision.contacts[0].point;

            _ballRigidBody.velocity = Vector2.zero;

            float _difference = _platformCenter.x - _hitPosition.x;

            if (_hitPosition.x < _platformCenter.x)
            {
                _ballRigidBody.AddForce(new Vector2(-Mathf.Abs(_difference * 200f), BallsManager.Instance.movingForce));
            }
            else
            {
                _ballRigidBody.AddForce(new Vector2(Mathf.Abs(_difference * 200f), BallsManager.Instance.movingForce));
            }
        }
    }

    public void Resize(float size)
    {
        StartCoroutine(ResizeWithAnimation(size));
    }

    public IEnumerator ResizeWithAnimation(float size)
    {
        this.IsTransforming = true;
        StartCoroutine(ResizeToNormal(AnimationTime));

        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        BoxCollider2D bc = gameObject.GetComponent<BoxCollider2D>();
        float currSize = sr.size.x;
        if (size > currSize)
        {
            while (currSize < size)
            {
                currSize += Time.deltaTime * 2;
                platformSprite.size = new Vector2(currSize , SizeHeight);
                platformCollider.size = new Vector2(currSize , SizeHeight);
                yield return null;
            }
        }
        else
        {
            while (currSize > size)
            {
                currSize -= Time.deltaTime * 2;
                platformSprite.size = new Vector2(currSize, SizeHeight);
                platformCollider.size = new Vector2(currSize, SizeHeight);
                yield return null;
            }
        }

        IsTransforming = false;
    }

    public IEnumerator ResizeToNormal(int animationTime)
    {
        yield return new WaitForSeconds(animationTime);
        Resize(SizeNormal);
    }
}
