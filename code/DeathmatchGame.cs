global using Sandbox;
global using SandboxEditor;
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;

/// <summary>
/// This is the heart of the gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
partial class DeathmatchGame : Game
{
	[Net]
	DeathmatchHud Hud { get; set; }

	public DeathmatchGame()
	{
		//
		// Create the HUD entity. This is always broadcast to all clients
		// and will create the UI panels clientside.
		//
		if ( IsServer )
		{
			Hud = new DeathmatchHud();

			_ = GameLoopAsync();
		}
	}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();

		ItemRespawn.Init();
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		var player = new DeathmatchPlayer();
		player.UpdateClothes( cl );
		player.Respawn();

		cl.Pawn = player;
	}

	public override void MoveToSpawnpoint( Entity pawn )
	{
		var spawnpoint = Entity.All
								.OfType<SpawnPoint>()
								.OrderByDescending( x => SpawnpointWeight( pawn, x ) )
								.ThenBy( x => Guid.NewGuid() )
								.FirstOrDefault();

		//Log.Info( $"chose {spawnpoint}" );

		if ( spawnpoint == null )
		{
			Log.Warning( $"Couldn't find spawnpoint for {pawn}!" );
			return;
		}

		pawn.Transform = spawnpoint.Transform;
	}

	/// <summary>
	/// The higher the better
	/// </summary>
	public float SpawnpointWeight( Entity pawn, Entity spawnpoint )
	{
		float distance = 0;

		foreach ( var client in Client.All )
		{
			if ( client.Pawn == null ) continue;
			if ( client.Pawn == pawn ) continue;
			if ( client.Pawn.LifeState != LifeState.Alive ) continue;

			var spawnDist = (spawnpoint.Position - client.Pawn.Position).Length;
			distance = MathF.Max( distance, spawnDist );
		}

		//Log.Info( $"{spawnpoint} is {distance} away from any player" );

		return distance;
	}

	public override void OnKilled( Client client, Entity pawn )
	{
		base.OnKilled( client, pawn );

		Hud.OnPlayerDied( To.Everyone, pawn as DeathmatchPlayer );
	}


	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		\var postProcess = Map.Camera.FindOrCreateHook<Sandbox.Effects.ScreenEffects>();

		postProcess.Sharpen = 0.5f;

		postProcess.FilmGrain.Intensity = 0.2f;
		postProcess.FilmGrain.Response = 1;

		postProcess.Vignette.Intensity = 1.0f;
		postProcess.Vignette.Roundness = 1.5f;
		postProcess.Vignette.Smoothness = 0.5f;
		postProcess.Vignette.Color = Color.Black;

		postProcess.Saturation = 1;

		postProcess.MotionBlur.Scale = 0;

		Audio.SetEffect( "core.player.death.muffle1", 0 );

		if ( Local.Pawn is DeathmatchPlayer localPlayer )
		{
			var timeSinceDamage = localPlayer.TimeSinceDamage.Relative;
			var damageUi = timeSinceDamage.LerpInverse( 0.25f, 0.0f, true ) * 0.3f;
			if ( damageUi > 0 )
			{
				postProcess.Saturation -= damageUi;
				postProcess.Vignette.Color = Color.Lerp( postProcess.Vignette.Color, Color.Red, damageUi );
				postProcess.Vignette.Intensity += damageUi;
				postProcess.Vignette.Smoothness += damageUi;
				postProcess.Vignette.Roundness += damageUi;

				postProcess.MotionBlur.Scale = damageUi * 0.5f;
			}


			var healthDelta = localPlayer.Health.LerpInverse( 0, 100.0f, true );

			healthDelta = MathF.Pow( healthDelta, 0.5f );

			postProcess.Vignette.Color = Color.Lerp( postProcess.Vignette.Color, Color.Red, 1 - healthDelta );
			postProcess.Vignette.Intensity += (1 - healthDelta) * 0.5f;
			postProcess.Vignette.Smoothness += (1 - healthDelta);
			postProcess.Vignette.Roundness += (1 - healthDelta) * 0.5f;
			postProcess.Saturation *= healthDelta;
			postProcess.FilmGrain.Intensity += (1 - healthDelta) * 0.5f;

			Audio.SetEffect( "core.player.death.muffle1", 1 - healthDelta, velocity: 2.0f );

		}


		if ( CurrentState == GameStates.Warmup )
		{
			postProcess.FilmGrain.Intensity = 0.4f;
			postProcess.FilmGrain.Response = 0.5f;

			postProcess.Saturation = 0;
		}
	}

	public static void Explosion( Entity weapon, Entity owner, Vector3 position, float radius, float damage, float forceScale )
	{
		// Effects
		Sound.FromWorld( "rust_pumpshotgun.shootdouble", position );
		Particles.Create( "particles/explosion/barrel_explosion/explosion_barrel.vpcf", position );

		// Damage, etc
		var overlaps = Entity.FindInSphere( position, radius );

		foreach ( var overlap in overlaps )
		{
			if ( overlap is not ModelEntity ent || !ent.IsValid() )
				continue;

			if ( ent.LifeState != LifeState.Alive )
				continue;

			if ( !ent.PhysicsBody.IsValid() )
				continue;

			if ( ent.IsWorld )
				continue;

			var targetPos = ent.PhysicsBody.MassCenter;

			var dist = Vector3.DistanceBetween( position, targetPos );
			if ( dist > radius )
				continue;

			var tr = Trace.Ray( position, targetPos )
				.Ignore( weapon )
				.WorldOnly()
				.Run();

			if ( tr.Fraction < 0.98f )
				continue;

			var distanceMul = 1.0f - Math.Clamp( dist / radius, 0.0f, 1.0f );
			var dmg = damage * distanceMul;
			var force = (forceScale * distanceMul) * ent.PhysicsBody.Mass;
			var forceDir = (targetPos - position).Normal;

			var damageInfo = DamageInfo.Explosion( position, forceDir * force, dmg )
				.WithWeapon( weapon )
				.WithAttacker( owner );

			ent.TakeDamage( damageInfo );
		}
	}

	[ClientRpc]
	public override void OnKilledMessage( long leftid, string left, long rightid, string right, string method )
	{
		Sandbox.UI.KillFeed.Current?.AddEntry( leftid, left, rightid, right, method );
	}

	public override void RenderHud()
	{
		var localPawn = Local.Pawn as DeathmatchPlayer;
		if ( localPawn == null ) return;


		localPawn.RenderHud( Screen.Size );
	}

}
