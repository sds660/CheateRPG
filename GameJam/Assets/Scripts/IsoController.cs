using System;
using UnityEngine;
using UnityEngine.Events;

public class IsoController : MonoBehaviour
{
	[SerializeField] private int moveSpeed;
	private AnimationManager _animationManager;
	private bool _facingRight = true;
	private bool _facingDown = true;
	private Vector2 _prevPosition;

	private int baseSpeed;
	private bool _isMoving = false;
		
	private void Awake()
	{
		_prevPosition = transform.position;
		TryGetComponent<AnimationManager>(out _animationManager);
	}
	private void Start()
	{
		baseSpeed = moveSpeed;
		_animationManager.FaceDown(_facingDown);
	}
	public void Move(Vector3 dest, float destOffSet)
	{
		if (Vector3.Distance(transform.position, dest) > destOffSet)
		{
			transform.position = Vector3.MoveTowards(transform.position, dest, moveSpeed * Time.deltaTime);
			_animationManager.Moving(true);
		}
		else
		{
			_animationManager.Moving(false);
		}
		UpdateDirection();
	}
	private void FixedUpdate()
	{
		var transform1 = transform;
		var position = transform1.position;
		
		_isMoving = _prevPosition != (Vector2)position;
		_prevPosition = position;
	}
	private void UpdateDirection()
    {
	    if (_prevPosition.x < transform.position.x && !_facingRight)
		    FlipX();		
	    else if (_prevPosition.x > transform.position.x && _facingRight)
		    FlipX();		
	    
	    if (_prevPosition.y < transform.position.y && !_facingDown)
		    FlipY();		
	    else if (_prevPosition.y > transform.position.y && _facingDown)
		    FlipY();		
	}
	private void FlipX()
	{
		_facingRight = !_facingRight;

		var transform1 = transform;
		var theScale = transform1.localScale;
		theScale.x *= -1;
		transform1.localScale = theScale;
	}
	private void FlipY()
	{
		_animationManager.FaceDown(_facingDown);
		_facingDown = !_facingDown;
	}
	public bool GetIsMoving()
	{
		return _isMoving;
	}
	public int GetBaseSpeed()
	{
		return baseSpeed;
	}

	public int GetMoveSpeed()
	{
		return moveSpeed;
	}
	public void SetSpeed(int speed)
	{
		moveSpeed = speed;
	}
}