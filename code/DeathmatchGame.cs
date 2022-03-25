global using Sandbox;
global using System;
global using System.Collections.Generic;
global using System.Linq;

/// <summary>
/// This is the heart of the gamemode. It's responsible
/// for creating the player and stuff.
/// </summary>
[Library( "dm98", Title = "DM98" )]
partial class DeathmatchGame : Game
{
	public DeathmatchGame()
	{
		//
		// Create the HUD entity. This is always broadcast to all clients
		// and will create the UI panels clientside. It's accessible 
		// globally via Hud.Current, so we don't need to store it.
		//
		if ( IsServer )
		{
			new DeathmatchHud();
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
}
