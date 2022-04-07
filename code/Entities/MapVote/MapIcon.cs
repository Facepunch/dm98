
using Sandbox.UI;

[UseTemplate]
class MapIcon : Panel
{
	public string VoteCount { get; set; } = "0";
	public string Title { get; set; } = "...";
	public string Org { get; set; } = "...";
	public string Ident { get; internal set; }
	public Panel OrgAvatar { get; set; }

	public MapIcon( string fullIdent )
	{
		Ident = fullIdent;

		_ = FetchMapInformation();
	}

	async Task FetchMapInformation()
	{
		var package = await Package.Fetch( Ident, true );
		if ( package == null ) return;
		if ( package.PackageType != Package.Type.Map ) return;

		Title = package.Title;
		Org = package.Org.Title;

		await Style.SetBackgroundImageAsync( package.Thumb );
		await OrgAvatar.Style.SetBackgroundImageAsync( package.Org.Thumb );
	}
}

