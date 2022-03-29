
partial class DeathmatchGame : Game
{

	[Net]
	public float StateTimer { get; set; } = 1f;
	[Net]
	public GameStates GameState { get; set; } = GameStates.WaitingForPlayers;
	[Net]
	public string NextMap { get; set; } = "facepunch.datacore";

	[Event.Tick.Server]
	private void OnTick()
	{
		if ( StateTimer > 0 )
		{
			StateTimer -= Time.Delta;

			if( StateTimer <= 0 )
			{
				MoveToNextState();
			}
		}

		if( GameState == GameStates.Live && !HasEnoughPlayers() )
		{
			StartMapVote();
		}
	}

	private void MoveToNextState()
	{
		switch ( GameState )
		{
			case GameStates.WaitingForPlayers:
				if ( !HasEnoughPlayers() )
				{
					StateTimer = 1f;
					break;
				}
				StartIntro();
				break;
			case GameStates.Warmup:
				if ( !HasEnoughPlayers() )
				{
					RestartGame();
					break;
				}
				StartGame();
				break;
			case GameStates.Live:
				EndGame();
				break;
			case GameStates.GameEnd:
				StartMapVote();
				break;
			case GameStates.MapVote:
				GotoNextMap();
				break;
		}
	}

	private void RestartGame()
	{
		Host.AssertServer();

		GameState = GameStates.WaitingForPlayers;
		StateTimer = 1f;

		FreshStart();
	}

	private void StartIntro()
	{
		Host.AssertServer();

		GameState = GameStates.Warmup;
		StateTimer = 30f;
	}

	private void StartGame()
	{
		Host.AssertServer();

		GameState = GameStates.Live;
		StateTimer = 15 * 60f;

		FreshStart();
	}

	private void EndGame()
	{
		Host.AssertServer();

		GameState = GameStates.GameEnd;
		StateTimer = 60f;
	}

	private void StartMapVote()
	{
		Host.AssertServer();

		GameState = GameStates.MapVote;
		StateTimer = 60f;
	}

	private void GotoNextMap()
	{
		Host.AssertServer();

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
