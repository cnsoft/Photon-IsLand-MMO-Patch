#pragma strict
#pragma implicit
#pragma downcast 

var sounds : AudioClip[] = new AudioClip[0];
var soundFrequency = 1.00;

var minSpeed = 0.00;
var turnSpeed = 0.00;
var randomFreq = 0.00;
randomFreq = 1.0 / randomFreq;

var randomForce = 0.00;
var toOriginForce = 0.00;
var toOriginRange = 0.00;

var damping = 0.00;

var gravity = 0.00;

var avoidanceRadius = 0.00;
var avoidanceForce = 0.00;

var followVelocity = 0.00;
var followRadius = 0.00;

var bankTurn = 0.00;

var raycast = false;
var bounce = 0.80;

private var target : SeagullFlightPath;
private var origin : Transform;
private var velocity : Vector3;
private var normalizedVelocity : Vector3;
private var randomPush : Vector3;
private var originPush : Vector3;
private var gravPush : Vector3;
private var hit : RaycastHit;
private var objects : Transform[];
private var otherSeagulls : Seagull[];
private var animationComponent : Animation;
private var transformComponent : Transform;
private var gliding = false;
private var bank = 0.00;
private var glide : AnimationState;

function Start ()
{
	gameObject.tag = transform.parent.gameObject.tag;

	animationComponent = GetComponentInChildren(Animation);
	animationComponent.Blend("fly");
	animationComponent["fly"].normalizedTime = Random.value;
	glide = animationComponent["glide"];

	origin = transform.parent;
	target = origin.GetComponent(SeagullFlightPath);
	transform.parent = null;
	transformComponent = transform;
	
	var tempSeagulls = new Component[0];
	if (transform.parent)
		tempSeagulls = transform.parent.GetComponentsInChildren(Seagull);
	objects = new Transform[tempSeagulls.length];
	otherSeagulls = new Seagull[tempSeagulls.length];
	for(var i=0;i<tempSeagulls.Length;i++)
	{
		objects[i] = tempSeagulls[i].transform;
		otherSeagulls[i] = tempSeagulls[i];
	}
			
	UpdateRandom();
}

function UpdateRandom ()
{
	while(true)
	{
		randomPush = Random.insideUnitSphere * randomForce;
		yield WaitForSeconds(randomFreq + Random.Range(-randomFreq / 2, randomFreq / 2));
	}	
}

function Update ()
{ 
	var speed = velocity.magnitude;
	var avoidPush = Vector3.zero;
	var avgPoint = Vector3.zero;
	var count = 0;
	var f = 0.0;
	var myPosition = transformComponent.position;
	
	for(var i=0;i<objects.Length;i++)
	{
		var o = objects[i];
		if(o != transformComponent)
		{
			var otherPosition = o.position;
			avgPoint += otherPosition;
			count++;
			
			var forceV = myPosition - otherPosition;
			var d = forceV.magnitude;
			if (d < followRadius)
			{
				if(d < avoidanceRadius)
				{
					f = 1.0 - (d / avoidanceRadius);
					if(d > 0) avoidPush += (forceV / d) * f * avoidanceForce;
				}
				
				f = d / followRadius;
				var otherSealgull : Seagull = otherSeagulls[i];
				avoidPush += otherSealgull.normalizedVelocity * f * followVelocity;	
			}
		}	
	}
	
	if(count > 0)
	{
		avoidPush /= count;
		toAvg = (avgPoint / count) - myPosition;	
	}	
	else
	{
		toAvg = Vector3.zero;		
	}
	
	forceV = origin.position + target.offset - myPosition;
	d = forceV.magnitude;
	f = d / toOriginRange;
	if(d > 0) originPush = (forceV / d) * f * toOriginForce;
	
	if(speed < minSpeed && speed > 0)
	{
		velocity = (velocity / speed) * minSpeed;
	}
	
	wantedVel = velocity;
	wantedVel -= wantedVel * damping * Time.deltaTime;	
	wantedVel += randomPush * Time.deltaTime;
	wantedVel += originPush * Time.deltaTime;
	wantedVel += avoidPush * Time.deltaTime;
	wantedVel += toAvg.normalized * gravity * Time.deltaTime;
	diff = transformComponent.InverseTransformDirection(wantedVel - velocity).normalized;
	bank = Mathf.Lerp(bank, diff.x, Time.deltaTime * 0.8);
	velocity = Vector3.RotateTowards(velocity, wantedVel, turnSpeed * Time.deltaTime, 100.00);
	
	transformComponent.rotation = Quaternion.LookRotation(velocity);
	transformComponent.Rotate(0, 0, -bank * bankTurn);
	
	// Raycast
	distance = speed * Time.deltaTime;
	if(raycast && distance > 0.00 && Physics.Raycast(myPosition, velocity, hit, distance))
	{
		velocity = Vector3.Reflect(velocity, hit.normal) * bounce;
	}
	else
	{
		transformComponent.Translate(velocity * Time.deltaTime, Space.World);
	}
	
	// Animation Controls
	if (speed > 0 )
	{
		up = (velocity / speed).y;
		if(gliding && up > 0)
		{
			gliding = false;
			animationComponent.Blend("glide", 0.00, 0.2);	
			animationComponent.Blend("fly", 1.00, 0.2);	
		}
		if(!gliding && up < -0.20)
		{
			gliding = true;
			animationComponent.Blend("glide", 1.00, 0.2);	
			animationComponent.Blend("fly", 0.00, 0.2);
			glide.speed = 0;
		}
	}
	
	// Sounds
	if(SeagullSoundHeat.heat < Mathf.Pow(Random.value, 1 / soundFrequency / Time.deltaTime))
	{
		AudioSource.PlayClipAtPoint(sounds[Random.Range(0, sounds.length)], myPosition, 0.90);	
		SeagullSoundHeat.heat += (1 / soundFrequency) / 10;
	}
	
	normalizedVelocity = velocity.normalized;
}