
partial class MapVoteEntity : Entity
{
	static MapVoteEntity Current;
	MapVotePanel Panel;

	[Net]
	public IDictionary<Client, string> Votes { get; set; }

	[Net]
	public string WinningMap { get; set; } = "facepunch.datacore";

	[Net]
	public RealTimeUntil VoteTimeLeft { get; set; } = 30;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
		Current = this;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		Current = this;
		Panel = new MapVotePanel();
		HudRootPanel.Current.AddChild( Panel );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Panel?.Delete();
		Panel = null;

		if ( Current == this )
			Current = null;
	}

	[Event.Client.Frame]
	public void OnFrame()
	{
		if ( Panel != null )
		{
			var seconds = VoteTimeLeft.Relative.FloorToInt().Clamp( 0, 60 );

			Panel.TimeText = $"00:{seconds:00}";
		}
	}

	void CullInvalidClients()
	{
		foreach ( var entry in Votes.Keys.Where( x => !x.IsValid() ).ToArray() )
		{
			Votes.Remove( entry );
		}
	}

	void UpdateWinningMap()
	{
		if ( Votes.Count == 0 )
			return;

		WinningMap = Votes.GroupBy( x => x.Value ).OrderBy( x => x.Count() ).First().Key;
	}

	void SetVote( Client client, string map )
	{
		CullInvalidClients();
		Votes[client] = map;

		UpdateWinningMap();
		RefreshUI();
	}

	[ClientRpc]
	void RefreshUI()
	{
		Panel.UpdateFromVotes( Votes );
	}

	[ConCmd.Server]
	public static void SetVote( string map )
	{
		if ( Current == null || ConsoleSystem.Caller == null )
			return;

		Current.SetVote( ConsoleSystem.Caller, map );
	}

}

