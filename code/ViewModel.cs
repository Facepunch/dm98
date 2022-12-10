partial class DmViewModel : BaseViewModel
{
	float walkBob = 0;

	public override void PlaceViewmodel()
	{
		// nothing
	}

	public void UpdateCamera()
	{
		var rotationDistance = Rotation.Distance( Camera.Rotation );

		Position = Camera.Position;
		Rotation = Rotation.Lerp( Rotation, Camera.Rotation, RealTime.Delta * rotationDistance * 1.1f );

		if ( Game.LocalPawn.LifeState == LifeState.Dead )
			return;

		if ( DeathmatchGame.CurrentState == DeathmatchGame.GameStates.GameEnd )
			return;

		//
		// Bob up and down based on our walk movement
		//
		var speed = Game.LocalPawn.Velocity.Length.LerpInverse( 0, 400 );
		var left = Camera.Rotation.Left;
		var up = Camera.Rotation.Up;

		if ( Game.LocalPawn.GroundEntity != null )
		{
			walkBob += Time.Delta * 2.0f * speed;
		}

		Position += up * MathF.Sin( walkBob ) * speed * -0.6f;
		Position += left * MathF.Sin( walkBob * 0.5f ) * speed * -0.3f;

		var uitx = new Sandbox.UI.PanelTransform();
		uitx.AddTranslateY( MathF.Sin( walkBob * 1.0f ) * speed * -4.0f );
		uitx.AddTranslateX( MathF.Sin( walkBob * 0.5f ) * speed * -3.0f );

		HudRootPanel.Current.Style.Transform = uitx;
	}
}
