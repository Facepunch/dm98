


partial class DeathmatchGame : Game
{
	public static GameStates CurrentState => (Current as DeathmatchGame)?.GameState ?? GameStates.WaitingForPlayers;

	[Net]
	public RealTimeUntil StateTimer { get; set; } = 0f;

	[Net]
	public GameStates GameState { get; set; } = GameStates.WaitingForPlayers;
	[Net]
	public string NextMap { get; set; } = "facepunch.datacore";

	[AdminCmd]
	public static void SkipStage()
	{
		if ( Current is not DeathmatchGame dmg ) return;

		dmg.StateTimer = 1;
	}

	private async Task WaitStateTimer()
	{
		while ( StateTimer > 0 )
		{
			await GameTask.DelayRealtimeSeconds( 1.0f );
		}

		// extra second for fun
		await GameTask.DelayRealtimeSeconds( 1.0f );
	}

	private async Task GameLoopAsync()
	{
		GameState = GameStates.WaitingForPlayers;

		while ( !HasEnoughPlayers() )
		{
			await GameTask.DelayRealtimeSeconds( 1.0f );
		}

		GameState = GameStates.Warmup;
		StateTimer = 10;
		await WaitStateTimer();

		GameState = GameStates.Live;
		StateTimer = 10 * 60;
		FreshStart();
		await WaitStateTimer();

		GameState = GameStates.GameEnd;
		StateTimer = 20000f;
		await WaitStateTimer();

		GameState = GameStates.MapVote;
		StateTimer = 10f;
		await WaitStateTimer();

		Global.ChangeLevel( NextMap );
	}

	private bool HasEnoughPlayers()
	{
		if ( All.OfType<DeathmatchPlayer>().Count() < 2 )
			return false;

		return true;
	}

	private void FreshStart()
	{
		foreach ( var cl in Client.All )
		{
			cl.SetInt( "kills", 0 );
			cl.SetInt( "deaths", 0 );
		}

		All.OfType<DeathmatchPlayer>().ToList().ForEach( x => x.Respawn() );
	}

	public enum GameStates
	{
		WaitingForPlayers,
		Warmup,
		Live,
		GameEnd,
		MapVote
	}

}
