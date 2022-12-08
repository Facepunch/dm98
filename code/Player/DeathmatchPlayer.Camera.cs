public partial class DeathmatchPlayer
{
	private void UpdateCamera()
	{
		Camera.Rotation = ViewAngles.ToRotation();
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Local.UserPreference.FieldOfView );

		if ( ThirdPerson )
		{
			Camera.FirstPersonViewer = null;

			Vector3 targetPos;
			var center = Position + Vector3.Up * 64;

			var pos = center;
			var rot = Rotation.FromAxis( Vector3.Up, -16 ) * Camera.Rotation;

			float distance = 130.0f * Scale;
			targetPos = pos + rot.Right * ((CollisionBounds.Mins.x + 32) * Scale);
			targetPos += rot.Forward * -distance;

			var tr = Trace.Ray( pos, targetPos )
				.WithAnyTags( "solid" )
				.Ignore( this )
				.Radius( 8 )
				.Run();

			Camera.Position = tr.EndPosition;
		}
		else
		{
			Camera.Position = EyePosition;
			Camera.FirstPersonViewer = this;

			if ( ActiveChild is DeathmatchWeapon weapon )
			{
				weapon.UpdateViewmodelCamera();
				weapon.UpdateCamera();
			}

			Camera.Main.SetViewModelCamera( Camera.FieldOfView );

			AddCameraEffects();
		}
	}

	float walkBob = 0;
	float lean = 0;
	float fov = 0;

	private void AddCameraEffects()
	{
		var speed = Velocity.Length.LerpInverse( 0, 320 );
		var forwardspeed = Velocity.Normal.Dot( Camera.Rotation.Forward );

		var left = Camera.Rotation.Left;
		var up = Camera.Rotation.Up;

		if ( GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		Camera.Position += up * MathF.Sin( walkBob ) * speed * 2;
		Camera.Position += left * MathF.Sin( walkBob * 0.6f ) * speed * 1;

		// Camera lean
		lean = lean.LerpTo( Velocity.Dot( Camera.Rotation.Right ) * 0.01f, Time.Delta * 15.0f );

		var appliedLean = lean;
		appliedLean += MathF.Sin( walkBob ) * speed * 0.3f;
		Camera.Rotation *= Rotation.From( 0, 0, appliedLean );

		speed = (speed - 0.7f).Clamp( 0, 1 ) * 3.0f;

		fov = fov.LerpTo( speed * 20 * MathF.Abs( forwardspeed ), Time.Delta * 4.0f );

		Camera.FieldOfView += fov;
	}
}
