[Library]
public partial class DeathmatchHud : HudEntity<HudRootPanel>
{
	public DeathmatchHud()
	{
	}

	[ClientRpc]
	public void OnPlayerDied( string victim, string attacker = null )
	{
		Host.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
}
