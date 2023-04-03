using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineForce : MonoBehaviour
{
	[SerializeField] private float shotPower = 150;
	[SerializeField] private float stopVelocity = .05f;

	[SerializeField] private LineRenderer lineRenderer;
	private bool isIdle;
	private bool isAiming;

	private Rigidbody ball;

	private void Awake(){
		ball = GetComponent<Rigidbody>();

		isAiming = false;
		lineRenderer.enabled = false;
	}
	
	private void Update(){
		if(ball.velocity.magnitude < stopVelocity){
			Stop();
		}
		ProcessAim();
	}

	private void OnMouseDown() {
		if(isIdle){
			isAiming = true;
		}
	}
	
	private void ProcessAim(){
		if(!isAiming || !isIdle){
			return;
		}

		Vector3? worldPoint = CastMouseClickRay();
		
		if(!worldPoint.HasValue){
			return;
		}
		DrawLine(worldPoint.Value);

		if(Input.GetMouseButtonUp(0)){
			Shoot(worldPoint.Value);
		}
	}

	public bool IsAiming(){
		return isAiming;
	}

	private void Shoot(Vector3 worldPoint){
		isAiming = false;
		lineRenderer.enabled = false;

		Vector3 horizontalWorldPoint = new Vector3(worldPoint.x, worldPoint.y, worldPoint.z);
		Vector3 direction = (horizontalWorldPoint - transform.position).normalized;
		float strength = Vector3.Distance(transform.position, horizontalWorldPoint);
		ball.AddForce(direction * strength * shotPower);
	}

	private void DrawLine(Vector3 worldPoint){
		Vector3[] positions = {
			transform.position,
			worldPoint};
		lineRenderer.SetPositions(positions);
		lineRenderer.enabled = true;
	}

	private void Stop(){
		ball.velocity = Vector3.zero;
		ball.angularVelocity = Vector3.zero;
		isIdle = true;
	}
	
	private Vector3? CastMouseClickRay(){
		Vector3 screenMousePosFar = new Vector3(
			Input.mousePosition.x,
			Input.mousePosition.y,
			Camera.main.farClipPlane);
		Vector3 screenMousePosNear = new Vector3(
			Input.mousePosition.x,
			Input.mousePosition.y,
			Camera.main.nearClipPlane);
		Vector3 WorldMousePosFar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
		Vector3 WorldMousePosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
		RaycastHit hit;
		if(Physics.Raycast(WorldMousePosNear, WorldMousePosFar - WorldMousePosNear, out hit, float.PositiveInfinity)){
			return hit.point;
		} else {
			return null;
		}
		
	}
}
