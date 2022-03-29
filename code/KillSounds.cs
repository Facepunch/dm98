/// <summary>
/// A system for classic deathmatch killstreak sounds
/// </summary>
public static partial class KillSounds
{
	[ServerVar( "dm_killsounds_enabled" )]
	public static bool EnableKillSounds { get; set; } = true;

	[ServerVar( "dm_killsounds_combo_time")]
	public static float ComboWaitTime { get; set; } = 4.5f;

	public static readonly SoundEvent HeadshotSound = new( "dm.announcer_headshot" );
	// public static readonly SoundEvent CrowbarKillSound = new( "dm.announcer_humiliation" );
	// public static readonly SoundEvent SelfKillSound = new( "dm.announcer_humiliation" );
	// public static readonly SoundEvent FirstBloodSound = new( "dm.announcer_firstblood" );

	public static readonly Dictionary<int, SoundEvent> ComboSounds = new()
	{
		{ 2, new( "dm.announcer_doublekill" ) },
		{ 3, new( "dm.announcer_triplekill" ) },
		{ 4, new( "dm.announcer_multikill" ) },
		{ 5, new( "dm.announcer_combowhore" ) },
	};

	public static void PlayerKilled( DeathmatchPlayer player, DeathmatchPlayer attacker, DamageInfo info )
	{
		if ( !EnableKillSounds ) return;

		// Headshot kill
		if ( player.GetHitboxGroup( info.HitboxIndex ) == 1 )
		{
			PlayKillSound( To.Single( attacker ), HeadshotSound.Sounds.FirstOrDefault() );
		}

		if ( attacker.TimeSinceLastKill < ComboWaitTime )
		{
			attacker.ComboKillCount++;

			if ( ComboSounds.TryGetValue( attacker.ComboKillCount, out var soundEvent ) )
				PlayKillSound( To.Single( attacker ), soundEvent.Sounds.FirstOrDefault() );
		}
		else
		{
			attacker.ComboKillCount = 1;
		}

		attacker.TimeSinceLastKill = 0;
	}

	[ClientRpc]
	public static void PlayKillSound( string sound )
	{
		Sound.FromScreen( sound );
	}
}
