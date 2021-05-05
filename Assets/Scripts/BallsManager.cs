using Assets.Scripts.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     BallManager class to manage multiball function.
/// </summary>
/// <seealso cref="Ball"/>
public class BallsManager : MonoBehaviour
{
    #region Singleton
    private static BallsManager _instance = null;
    public static BallsManager Instance => _instance;

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

    private Ball _initialBall;
    private Rigidbody2D _initialBallRigidBody;

    public Ball _prefabBall;
    public List<Ball> BallsCollection { get; set; }
    public float movingForce = 300f;

    /// <summary>
    ///     Start method by Unity Engine
    /// </summary>
    private void Start()
    {
        InitializeBalls();
    }

    /// <summary>
    ///     Update method by Unity Engine
    /// </summary>
    private void Update()
    {
        var _gameState = GameManager.Instance.GetState();

        if (_gameState == GameState.GS_STOPPED)
        {
            Vector2 _platformPosition = Platform.Instance.gameObject.transform.position;
            Vector2 _ballPosition = new Vector3(_platformPosition.x , _platformPosition.y + 0.4f , 0);
            _initialBall.transform.position = _ballPosition;

            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    _initialBallRigidBody.isKinematic = false;
                    _initialBallRigidBody.AddForce(new Vector2(0 , movingForce));
                    GameManager.Instance.SetState(GameState.GS_STARTED);
                }
            }
        }
    }

    /// <summary>
    ///     Initialize one starting ball.
    /// </summary>
    private void InitializeBalls()
    {
        Vector2 _platformPosition = Platform.Instance.gameObject.transform.position;
        Vector2 _startingPosition = new Vector2(_platformPosition.x , _platformPosition.y + 0.4f);

        _initialBall = Instantiate(_prefabBall , _startingPosition , Quaternion.identity);
        _initialBallRigidBody = _initialBall.GetComponent<Rigidbody2D>();

        this.BallsCollection = new List<Ball> { _initialBall };
    }

    /// <summary>
    ///     Destroy balls and create one initial.
    /// </summary>
    /// <seealso cref="InitializeBalls"/>
    public void ResetBalls()
    {
        foreach (var ball in this.BallsCollection.ToList())
        {
            Destroy(ball.gameObject);
        }

        InitializeBalls();
    }

    public void AddBalls(Vector2 position , byte count)
    {
        for (int i = 0; i < count; i++)
        {
            // Max 3 balls
            if (this.BallsCollection.Count > 3)
            {
                break;
            }

            Ball ball = Instantiate(_prefabBall, position, Quaternion.identity);
            Rigidbody2D ballRigidBody = ball.GetComponent<Rigidbody2D>();

            ballRigidBody.isKinematic = false;
            ballRigidBody.AddForce(new Vector2(0 , movingForce));
            this.BallsCollection.Add(ball);
        }
    }
}
